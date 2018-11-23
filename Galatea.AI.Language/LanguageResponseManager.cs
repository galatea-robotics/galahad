using System;
using System.Collections.Generic;
using System.Globalization;
using Galatea.AI.Abstract;
using Galatea.AI.Characterization;
using Galatea.Globalization;
using Galatea.IO;
using Galatea.Runtime;

namespace Galatea.AI.Language
{
    /// <summary>
    /// Contains all the good algorithms for Language Response.
    /// </summary>
    internal sealed class LanguageResponseManager : RuntimeComponent, IResponseManager
    {
        private static ILanguageAnalyzer _languageModel;

        public LanguageResponseManager(ILanguageAnalyzer languageModel)
        {
            _languageModel = languageModel;
            _languageModel.Add(this);
        }

        /// <summary>
        /// For <see cref="InputTokenManager"/> reference.
        /// </summary>
        internal static ILanguageAnalyzer Default { get { return _languageModel; } }

        internal event EventHandler<RespondingEventArgs> Responding;

        #region IResponseManager

        /// <summary>
        /// Gets the system name of the AI Self-Awareness.
        /// </summary>
        string ITemplate.Name
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.ProviderID; }
        }
        /// <summary>
        /// Gets or sets a string value representing a user-friendly definition 
        /// for the AI.
        /// </summary>
        string ITemplate.FriendlyName
        {
            get { return _languageModel.FriendlyName; }
            set { throw new System.NotImplementedException(); }
        }
        public string GetResponse(IResponseBag responseBag, ContextNode inputContext)
        {
            if (responseBag == null) throw new TeaArgumentNullException("responseBag");
            if (inputContext != null && inputContext.Handled) throw new TeaArgumentException("The input context has already been analyzed.");

            // Split input into separate processing requests using delimiter
            string[] separator = new[] { LanguageResources.Request_Delimiter };
            string[] requests = responseBag.Input.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            string[] results = new string[requests.Length];

            for (int i = 0; i < requests.Length; i++)
            {
                // Get Response
                results[i] = GetResponse2(requests[i], inputContext, responseBag).Output;
            }

            string result = string.Join(LanguageResources.Response_Delimiter, results);
            //IResponse response = responseBag.Push(this, result);

            // Finalize
            if (inputContext != null) inputContext.Handled = true;
            return result;
        }

        /*
        bool IResponseManager.ResponseLogging
        {
            get { return _responseLogging; }
            set { _responseLogging = value; }
        }
         */

        #endregion

        #region Private

        /// <summary>
        /// Analyzes user input text and hardware context, applies comprehension, and 
        /// formulates a response
        /// </summary>
        /// <returns>
        /// The text response to user input.
        /// </returns>
        private IResponse GetResponse2(string input, ContextNode inputContext, IResponseBag responseBag)
        {
            // Initialize 
            string result = LanguageResources.UNDEFINED;
            List<string> inputTokens = InputTokenManager.GetInputTokens(input);
            UserFeedback userFeedback = null;

            IResponse response = responseBag.Push(this, result);
            InputType inputType = inputContext == null ? InputType.Undefined : inputContext.Key.InputType;

            ContextNode responseContext = new ContextNode(ContextType.Response, inputType, ProviderID, null, null);
            TemplateType inputTemplateType = InputTokenManager.GetTemplateType(inputTokens);

            // Is Input asking "WHAT"?
            if (InputTokenManager.CheckInputTypeUserEvaluate(inputTokens))
            {
                if (InputTokenManager.IsEvaluateNameEntity(inputTokens))
                    responseContext.TemplateType = TemplateType.PatternEntity;
                else
                    responseContext.TemplateType = inputTemplateType;

                // Fire pre-Event
                RespondingEventArgs e = new RespondingEventArgs(ContextType.Response, InputType.UserEvaluate);
                if (Responding != null) Responding(this, e);

                response.Output = GetUserEvaluateInputResponse(input, inputTokens, responseContext, result);

                // Create a new Input Context to attach to the new Response
                response.InputContext = new ContextNode(ContextType.Input, InputType.UserEvaluate, ProviderID, null, null);
            }
            else if (inputContext != null && inputContext.Key.InputType == InputType.UserEvaluate)
            {
                responseContext.TemplateType = inputContext.TemplateType;
                if (inputTemplateType != TemplateType.Null) responseContext.TemplateType = inputTemplateType;

                // Does the Input contain Feedback (positive or negative response)?
                bool isInputFeedback = FeedbackTokenManager.CheckInputTokensForUserFeedback(inputTokens, ref userFeedback);

                // Was the previous Input asking WHAT [Template?]
                bool isContextFeedback = InputTokenManager.CheckEvaluateInputFeedback(inputTokens, responseContext);

                if (isInputFeedback || isContextFeedback)
                {
                    response.UserFeedback = userFeedback;

                    if (_languageModel.AI.Engine.Debugger.LogLevel == Diagnostics.DebuggerLogLevel.Diagnostic)
                    {
                        // Respond to Positive or Negative Feedback
                        switch (userFeedback.FeedbackType)
                        {
                            case FeedbackType.Positive:
                                responseBag.Push(this, " [+] Invoking response to POSITIVE input [+]");
                                break;
                            case FeedbackType.Negative:
                                responseBag.Push(this, " [-] Invoking response to NEGATIVE input [-]");
                                break;
                            case FeedbackType.Neutral:
                                responseBag.Push(this, " [-] Invoking response to NEUTRAL input [-]");
                                break;
                        }
                    }
                }

                responseContext.PatternEntity = inputContext.PatternEntity;
                responseContext.NamedEntity = inputContext.NamedEntity;
                responseContext.NamedTemplate = inputContext.NamedTemplate;
                //responseContext.NameFromFeedback = inputContext.NameFromFeedback;

                // Create a new Input Context to attach to the new Response
                response.InputContext = new ContextNode(ContextType.Input, InputType.UserFeedback, ProviderID, null, null);
            }

            // Append Response Context
            response.ResponseContext = responseContext;

            // Update Input context
            if (response.InputContext != null)
            {
                response.InputContext.PatternEntity = response.ResponseContext.PatternEntity;
                response.InputContext.NamedEntity = response.ResponseContext.NamedEntity;
                response.InputContext.NamedTemplate = response.ResponseContext.NamedTemplate;
                response.InputContext.NameFromFeedback = response.ResponseContext.NameFromFeedback;
                response.InputContext.TemplateType = response.ResponseContext.TemplateType;
            }

            return response;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private static string GetUserEvaluateInputResponse(string input, List<string> inputTokens, ContextNode responseContext, string output)
        {
            switch (responseContext.TemplateType)
            {
                case TemplateType.Null:

                    // Use chatbot response instead

                    // NOTE:  Entities and Templates are learned. 
                    // TemplateTypes are programmed, not learned.
                    return output;

                case TemplateType.Name:
                    return GetAiOrUserName(input, inputTokens);

                case TemplateType.PatternEntity:
                    return GetEntityName(responseContext);

                default:
                    return GetTemplateName(responseContext);
            }

            // TODO:  What LETTER?  What NUMBER?
        }

        private static string GetEntityName(ContextNode responseContext)
        {
            string response;

            responseContext.PatternEntity = _languageModel.AI.RecognitionModel.PatternEntity;
            NamedEntity namedEntity = _languageModel.AI.RecognitionModel.NameEntity(responseContext.PatternEntity);

            // Form Response
            response = namedEntity.FriendlyName;
            string article = InputTokenManager.GetArticle(response);

            if(namedEntity.TemplatePattern[namedEntity.NamedTemplateType].FriendlyName == RecognitionResources.Unknown_Template_FriendlyName)
            {
                response = response.Replace(RecognitionResources.Unknown_Template_FriendlyName, "");
                response = response.Replace("  ", " ");
                response = response.Replace(RecognitionResources.Unknown_Entity_Type, InputTokenManager.GetThingToken());
                namedEntity.FriendlyName = response;
            }

            response = string.Format(CultureInfo.CurrentCulture,
                LanguageResources.UserEvaluateInput_NameEntity_Response_Format,
                article, response);

            // Finalize
            responseContext.PatternEntity = _languageModel.AI.RecognitionModel.PatternEntity;
            responseContext.NamedEntity = namedEntity;

            return response;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private static string GetTemplateName(ContextNode responseContext)
        {
            string response;

            // Identify the template instance
            BaseTemplate namedTemplate = null;

            #region // Invoke the Recognition Model

            try
            {
                namedTemplate = _languageModel.AI.RecognitionModel.IdentifyTemplate(responseContext.TemplateType);
            }
            catch (TeaNullRecognitionPatternException ex)
            {
                _languageModel.AI.Engine.Debugger.Log(Diagnostics.DebuggerLogLevel.Message, ex.Message);

                string nullPatternMessage = string.Format(CultureInfo.CurrentCulture,
                    Globalization.RecognitionResources.Recognition_Pattern_Does_Not_Exist,
                    InputTokenManager.GetTemplateTypePluralToken(responseContext.TemplateType));

                return nullPatternMessage;
            }
            catch (TeaException ex)
            {
                _languageModel.AI.Engine.Debugger.HandleTeaException(ex, _languageModel);
            }
            catch (System.Exception ex)
            {
                _languageModel.AI.Engine.Debugger.ThrowSystemException(ex, _languageModel);
            }
            if (_languageModel.AI.Engine.Debugger.Exception != null)
            {
                response = string.Format(CultureInfo.CurrentCulture,
                    _languageModel.AI.Engine.Debugger.ErrorMessage,
                    Globalization.ProviderResources.RecognitionModel_Provider_Name);

                // Restore Debugger Error Status
                _languageModel.AI.Engine.Debugger.ClearError();

                // Send Error Notification to Speech Module
                return response;
            }

            #endregion

            if (namedTemplate == null)
            {
                response = string.Format(CultureInfo.CurrentCulture, LanguageResources.UserEvaluateInput_Response_IDK_Template, responseContext.TemplateType);
            }
            else
            {
                // The template has been identified
                response = namedTemplate.FriendlyName;

                if (namedTemplate.FriendlyName != Globalization.RecognitionResources.Unknown_Template_FriendlyName)
                    response = string.Format(CultureInfo.CurrentCulture,
                        LanguageResources.UserEvaluateInput_NameTemplate_Response_Format,
                        responseContext.TemplateType, namedTemplate.FriendlyName);
            }

            // Update Context
            responseContext.PatternEntity = _languageModel.AI.RecognitionModel.PatternEntity;
            responseContext.NamedEntity = null;

            // Finalize
            responseContext.NamedTemplate = namedTemplate;

            return response;
        }

        private static string GetAiOrUserName(string response, List<string> inputTokens)
        {
            foreach (string it in inputTokens)
            {
                // What is my name?
                if (string.Equals(it, LanguageResources.Tokens_My, StringComparison.CurrentCultureIgnoreCase))
                    return string.Format(CultureInfo.CurrentCulture,
                        LanguageResources.EvaluateName_Response,
                        LanguageResources.Tokens_Your,
                        _languageModel.AI.Engine.User.FriendlyName);    // YOUR name is ________

                // What is my name?
                else if (string.Equals(it, LanguageResources.Tokens_Your, StringComparison.CurrentCultureIgnoreCase))
                    return string.Format(CultureInfo.CurrentCulture,
                        LanguageResources.EvaluateName_Response,
                        LanguageResources.Tokens_My,
                        _languageModel.ChatbotManager.Current.FriendlyName); // MY name is ________
            }

            return response;
        }

        #endregion
    }
}
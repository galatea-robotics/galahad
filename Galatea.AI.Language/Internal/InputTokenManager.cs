using System;
using System.Collections.Generic;
using Galatea.AI.Abstract;
using Galatea.IO;
using System.Globalization;

namespace Galatea.AI.Language
{
    internal static class InputTokenManager
    {
        #region Declarations

        private static readonly string[] evaluateTokens = LanguageResources.Tokens_Evaluate.Split(',');
        private static readonly string[] theTokens = LanguageResources.Tokens_The.Split(',');
        private static readonly string[] toBeTokens = LanguageResources.Tokens_ToBe.Split(',');
        private static readonly string[] itTokens = LanguageResources.Tokens_It.Split(',');

        private static readonly string[] secondPersonTokens = string.Format(
            CultureInfo.InvariantCulture, LanguageResources.Tokens_2nd_Person,
            LanguageResponseManager.Default.ChatbotManager.Current.FriendlyName).Split(',');

        private static readonly Dictionary<string, string[]> articleTokens = GetArticleTokens();

        private static readonly string[] thingTokens = LanguageResources.Tokens_Thing.Split(',');

        #endregion

        #region Internal
        internal static List<string> GetInputTokens(string input)
        {
            List<string> result = new List<string>();

            // Break input from spaces
            string[] sTokens = input.Split(' ');

            // Add punctuation
            foreach (string token in sTokens)
            {
                // TODO : Globalize punctuation as ",.!?"

                int value = 0;
                value += AddPunctuatedInputTokens(result, token, ',');
                value += AddPunctuatedInputTokens(result, token, '.');
                value += AddPunctuatedInputTokens(result, token, '!');
                value += AddPunctuatedInputTokens(result, token, '?');

                // No results - add the whole thing
                if (value == 0 && !string.IsNullOrEmpty(token)) result.Add(token);
            }

            // TODO: check tokens in vocabulary dictionary and add new words
            return result;
        }

        internal static bool CheckInputTypeUserEvaluate(List<string> inputTokens)
        {
            // Is Input asking "WHAT"?
            foreach (string it in inputTokens)
            {
                // Compare to Localized Evaluate Tokens from Resource
                foreach (string et in evaluateTokens)
                {
                    if (string.Equals(et, it, StringComparison.CurrentCultureIgnoreCase))
                    {
                        inputTokens.Remove(it);
                        return true;
                    }
                }
            }

            return false;
        }

        internal static TemplateType GetTemplateType(List<string> inputTokens)
        {
            // Compare to Localized Template Type Tokens from Resource
            foreach (string ttTokenInfo in LanguageResources.Tokens_TemplateType.Split(','))
            {
                TemplateType result = GetTemplateType(inputTokens, ttTokenInfo.Split(':'));
                if (result != TemplateType.Null) return result;
            }

            return TemplateType.Null;
        }

        internal static string GetTemplateTypePluralToken(TemplateType templateType)
        {
            foreach (string ttTokenInfo in LanguageResources.Tokens_TemplateType_Plural.Split(','))
            {
                string[] ttTokens = ttTokenInfo.Split(':');
                if ((int)templateType == int.Parse(ttTokens[1], CultureInfo.InvariantCulture))
                {
                    return ttTokens[0];
                }
            }

            return templateType.ToString();
        }

        internal static bool CheckEvaluateInputFeedback(List<string> inputTokens, ContextNode responseContext)
        {
            bool result = false;

            for (int i = 0; i < inputTokens.Count; i++)
            {
                // Does the Input contain the Context TemplateType?  
                // e.g. What "COLOR"?  What "SHAPE"?
                string[] ttTokens = new[] { responseContext.TemplateType.ToString(), ((int)responseContext.TemplateType).ToString(CultureInfo.InvariantCulture) };
                TemplateType templateTypeFromInput = GetTemplateType(inputTokens, ttTokens);
                if (templateTypeFromInput == responseContext.TemplateType)
                {
                    // The next token is the correct Template label?
                    result = true;
                }
                // Did the Input occur within the context of a User Evaluate request?
                if (responseContext.Key.InputType == InputType.UserEvaluate)
                {
                    // The next token is the correct Template label?
                    result = true;
                }

                // At this point, the input has been evaluated for Feedback
                if (result)
                {
                    // The next token is the correct Template label?
                    SetUnknownNameFromToken(inputTokens, responseContext);
                    break;
                }
                else
                {
                    // Scrub this token as idle chatter and go to the next
                    inputTokens.Remove(inputTokens[i]);
                }
            }

            return result;
        }

        internal static bool IsEvaluateNameEntity(List<string> inputTokens)
        {
            // Is it asking what entity?  ("WHAT IS IT?")
            for (int i = 0; i < inputTokens.Count; i++)
            {
                bool isToBe = IsToBeVerb(inputTokens, inputTokens[i]);

                if (isToBe)
                {
                    bool isIt = IsInList(inputTokens, inputTokens[i], itTokens);
                    if (isIt) return true;
                }
            }

            return false;
        }
        
        internal static string GetArticle(string response)
        {
            // Default Article
            string defaultArticle = "";
            string initial = response.Substring(0, 1);

            foreach (var a in articleTokens)
            {
                string[] aValue = a.Value;

                if (aValue.Length == 1 && aValue[0] == "default")
                    defaultArticle = a.Key;
                else
                    if (IsInList(null, initial, aValue, false))
                        return a.Key;
            }

            return defaultArticle;
        }

        internal static string GetThingToken()
        {
            // TODO:  Initialize THING tokens as NamedEntities, add them to the database, and apply feedback algorithm for retrieval.

            int rand = Galatea.AI.Math.Random.GetRandom(thingTokens.Length);
            return thingTokens[rand];
        }

        #endregion

        #region Private
        private static bool IsToBeVerb(List<string> inputTokens, string token, bool removeToken = true)
        {
            // Analyze Token as grammar - if it's a "TO BE" verb
            return IsInList(inputTokens, token, toBeTokens, removeToken);
        }
        private static bool IsThe(List<string> inputTokens, string token, bool removeToken = true)
        {
            return IsInList(inputTokens, token, theTokens, removeToken);
        }
        private static bool IsArticle(List<string> inputTokens, string token, bool removeToken = true)
        {
            return IsInList(inputTokens, token, articleTokens.Keys, removeToken);
        }
        private static bool Is2ndPerson(List<string> inputTokens, string token, bool removeToken = true)
        {
            return IsInList(inputTokens, token, secondPersonTokens, removeToken);
        }
        private static bool IsInList(List<string> inputTokens, string token, IEnumerable<string> list, bool removeToken = true)
        {
            foreach(string item in list)
            {
                if(string.Equals(item.Trim(), token.Trim(), StringComparison.CurrentCultureIgnoreCase))
                {
                    if (removeToken) inputTokens.Remove(token);
                    return true;
                }
            }

            return false;
        }
        private static int AddPunctuatedInputTokens(List<string> result, string token, params char[] punctuationMark)
        {
            int value = 0;

            if (token.Contains(punctuationMark[0].ToString()))
            {
                // Split the token into a punctuation-delimited array
                int s;
                string[] sPeriodDelimited = token.Split(punctuationMark);
                for (s = 0; s < sPeriodDelimited.Length - 1; s++)
                {
                    result.Add(sPeriodDelimited[s]);
                    result.Add(punctuationMark[0].ToString());
                    value += 2;
                }

                // Add the last token
                if (!string.IsNullOrEmpty(sPeriodDelimited[s]))
                {
                    result.Add(sPeriodDelimited[s]);
                    value++;
                }
            }

            return value;
        }
        private static TemplateType GetTemplateType(List<string> inputTokens, string[] ttTokens)
        {
            foreach (string it in inputTokens)
            {
                if (string.Equals(ttTokens[0], it, StringComparison.CurrentCultureIgnoreCase))
                {
                    inputTokens.Remove(it);
                    return (TemplateType) int.Parse(ttTokens[1], CultureInfo.InvariantCulture);
                }
            }

            return TemplateType.Null;
        }

        private static Dictionary<string, string[]> GetArticleTokens()
        {
            Dictionary<string, string[]> result = new Dictionary<string, string[]>();
            string[] articleTokenInfo = LanguageResources.Tokens_Articles.Split(',');

            foreach (string a in articleTokenInfo)
            {
                string[] articleTokens = a.Split(':');
                string key = articleTokens[0];
                string[] value = articleTokens[1].Split('-');

                result.Add(key, value);
            }

            return result;
        }

        private static void SetUnknownNameFromToken(List<string> inputTokens, ContextNode responseContext)
        {
            int i = 0;
            string nextToken, prevToken = "";

            while (i < inputTokens.Count)
            {
                nextToken = inputTokens[i];

                if (!IsToBeVerb(inputTokens, nextToken))
                {
                    prevToken = nextToken;

                    // Clear the token and go to the next
                    inputTokens.Remove(nextToken);
                }
                else
                {
                    // Ignore if the 2B token is in 2nd person
                    if (Is2ndPerson(inputTokens, prevToken, false))
                    {
                        return;
                    }

                    // The correct answer IS __________________
                    // The remainder is the label 
                    string nameFromFeedback = "";

                    while (i < inputTokens.Count)
                    {
                        nextToken = inputTokens[i];
                        if (IsThe(inputTokens, nextToken))
                        {
                            continue;
                        }
                        nextToken = inputTokens[i];
                        if (IsArticle(inputTokens, nextToken))
                        {
                            continue;
                        }

                        string punkchu = ",.!?";    // TODO : Globalize punctuation as ",.!?"
                        if (!punkchu.Contains(nextToken))
                            nameFromFeedback += " " + nextToken;
                        else
                            break;

                        i++;
                    }

                    responseContext.NameFromFeedback = nameFromFeedback.Trim();
                }
            }
        }

        #endregion
    }
}
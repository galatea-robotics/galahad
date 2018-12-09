using System;
using System.Threading.Tasks;
using Windows.Media.SpeechRecognition;
using Windows.Globalization;

namespace Galahad.Robotics.Speech
{
    using Galatea.Runtime;
    using Galatea.Speech;

    class SpeechRecognition : RuntimeComponent, ISpeechRecognition
    {
        private ISpeechModule _speechModule;

        private Windows.Media.SpeechRecognition.SpeechRecognizer speechRecognizer;

        /// <summary>
        /// This HResult represents the scenario where a user is prompted to allow in-app speech, but 
        /// declines. This should only happen on a Phone device, where speech is enabled for the entire device,
        /// not per-app.
        /// </summary>
        private static uint HResultPrivacyStatementDeclined = 0x80045509;

        /// <summary>
        /// Initialize Speech Recognizer and compile constraints.
        /// </summary>
        /// <param name="recognizerLanguage">Language to use for the speech recognizer</param>
        /// <returns>Awaitable task.</returns>
        private async Task InitializeRecognizer(Language recognizerLanguage)
        {
            if (speechRecognizer != null)
            {
                this.speechRecognizer.Dispose();
                this.speechRecognizer = null;
            }

            try
            {
                // Create an instance of SpeechRecognizer.
                speechRecognizer = new SpeechRecognizer(recognizerLanguage);

                // Compile the dictation topic constraint, which optimizes for dictated speech.
                var dictationConstraint = new SpeechRecognitionTopicConstraint(SpeechRecognitionScenario.Dictation, "dictation");
                speechRecognizer.Constraints.Add(dictationConstraint);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                throw;
            }

            try
            {
                SpeechRecognitionCompilationResult compilationResult = await speechRecognizer.CompileConstraintsAsync();

                //// RecognizeWithUIAsync allows developers to customize the prompts.    
                //speechRecognizer.UIOptions.AudiblePrompt = "Dictate a phrase or sentence...";
                //speechRecognizer.UIOptions.ExampleText = speechResourceMap.GetValue("DictationUIOptionsExampleText", speechContext).ValueAsString;

                // Check to make sure that the constraints were in a proper format and the recognizer was able to compile it.
                if (compilationResult.Status != SpeechRecognitionResultStatus.Success)
                {
                    //// Disable the recognition buttons.
                    //btnRecognizeWithUI.IsEnabled = false;
                    //btnRecognizeWithoutUI.IsEnabled = false;

                    //// Let the user know that the grammar didn't compile properly.
                    //resultTextBlock.Visibility = Visibility.Visible;
                    //resultTextBlock.Text = "Unable to compile grammar.";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public SpeechRecognition(ISpeechModule speechModule)
        {
            // Do Component
            _speechModule = speechModule;
            _speechModule.SpeechRecognition = this;
            _speechModule.Add(this);

            // Initialize recognizer
            Language userLanguage = new Language(Windows.System.UserProfile.GlobalizationPreferences.Languages[0]);
            InitializeRecognizer(userLanguage).Wait();
        }

        public event EventHandler<SpeechRecognizedEventArgs> Recognized;

        async void ISpeechRecognition.StartListening()
        {
            _inactive = false;

            // Start recognition.
            try
            {
                if (_speechModule.TextToSpeech.IsSpeaking)
                {
                    _speechModule.LanguageModel.AI.Engine.Debugger.Log(
                        Galatea.Diagnostics.DebuggerLogLevel.Diagnostic,
                        "TTS is speaking; Listening paused...");
                }
                else
                {
                    //// Get out of this fucking loop
                    //if (_isListening) return;

                    //_isListening = true;

                    // Start Listening
                    int ruleId = -1;
                    SpeechRecognitionStatus status = SpeechRecognitionStatus.Empty;
                    SpeechRecognitionResult speechRecognitionResult = await speechRecognizer.RecognizeAsync();

                    // If successful, display the recognition result.
                    if (speechRecognitionResult.Status == SpeechRecognitionResultStatus.Success)
                    {
                        if (speechRecognitionResult.Text != "")
                        {
                            ruleId = 0;
                            status = speechRecognitionResult.Status.Convert();
                        }
                    }
                    else
                    {
                        //resultTextBlock.Visibility = Visibility.Visible;
                        //resultTextBlock.Text = string.Format("Speech Recognition Failed, Status: {0}", speechRecognitionResult.Status.ToString());
                    }

                    // Fire Event
                    Recognized?.Invoke(this, new SpeechRecognizedEventArgs(ruleId, speechRecognitionResult.Text, null, status));

                    //_isListening = false;
                }
            }
            catch (TaskCanceledException exception)
            {
                // TaskCanceledException will be thrown if you exit the scenario while the recognizer is actively
                // processing speech. Since this happens here when we navigate out of the scenario, don't try to 
                // show a message dialog for this exception.
                System.Diagnostics.Debug.WriteLine("TaskCanceledException caught while recognition in progress (can be ignored):");
                System.Diagnostics.Debug.WriteLine(exception.ToString());
            }
            catch(System.InvalidOperationException exception)
            {
                // No idea why it keeps throwing this Exception
                _speechModule.LanguageModel.AI.Engine.Debugger.Log(Galatea.Diagnostics.DebuggerLogLevel.Error, exception.Message);
                _speechModule.LanguageModel.AI.Engine.Debugger.Log(Galatea.Diagnostics.DebuggerLogLevel.StackTrace, exception.StackTrace);
            }
            catch (Exception exception)
            {
                string msg;
                // Handle the speech privacy policy error.
                if ((uint)exception.HResult == HResultPrivacyStatementDeclined)
                {
                    msg = Galatea.Globalization.RoboticsResources.SpeechRecognition_PrivacySettings_NotAccepted;
                    throw new TeaSpeechException(msg, exception);
                }
                //else
                //{
                //    msg = exception.Message;
                //}
                //var messageDialog = new Windows.UI.Popups.MessageDialog(msg, "Exception");
                //await messageDialog.ShowAsync();

                throw;
            }
        }
        async void ISpeechRecognition.StopListening()
        {
            await Task.Run(() => _inactive = true);
        }

        bool ISpeechRecognition.Inactive
        {
            get { return _inactive; }
        }

        protected override void Dispose(bool disposing)
        {
            speechRecognizer.Dispose();
            base.Dispose(disposing);
        }

        //private bool _isListening;
        protected bool _inactive = true;
    }
}

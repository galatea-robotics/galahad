using System;
using System.Globalization;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Gpio;
using Windows.Media.Capture;
using Windows.UI.Core;

namespace Galahad
{
    using Galatea.IO;
    using Galatea.Globalization;
    using Galahad.Robotics.Speech;
    using Galatea.Speech;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Galahad.UI.BasePage, Galahad.API.INetCommands
    {
        public MainPage() : base()
        {
            _current = this;
        }

        protected override string UserName { get { return App.Engine.User.Name; } }

        bool Galahad.API.INetCommands.CameraOn
        {
            get { return (bool)cameraOnOff.IsChecked; }
            set
            {
                Task.Run(async () => await
                    CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        cameraOnOff.IsChecked = value;
                    }));
            }
        }
        bool Galahad.API.INetCommands.MicrophoneOn
        {
            get { return (bool)microphoneOnOff.IsChecked; }
            set
            {
                Task.Run(async () => await
                    CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        microphoneOnOff.IsChecked = value;
                    }));
            }
        }

        void Galahad.API.INetCommands.SetPinValue(int pinNumber, int value)
        {
            this.SetPinValue(pinNumber, value);
        }

        string Galahad.API.INetCommands.GetResponse(string userName, string input)
        {
            // Show Input first
            DisplayResponse(userName, input);

            // Get Response
            return this.GetResponse(input, userName);
        }

        #region Camera

        protected override void CameraOn()
        {

            // TODO:  https://stackoverflow.com/questions/42849919/live-video-streaming-using-raspberry-pi-and-c-sharp

            Task.Run(async () =>
            {
                try
                {
                    await SendResponse("Initializing camera to capture audio and video...").ConfigureAwait(false);

                    // Use default initialization
                    mediaCapture = new MediaCapture();
                    await mediaCapture.InitializeAsync();

                    // Set callbacks for failure and recording limit exceeded
                    mediaCapture.Failed += new MediaCaptureFailedEventHandler(mediaCapture_Failed);
                    mediaCapture.RecordLimitationExceeded += new Windows.Media.Capture.RecordLimitationExceededEventHandler(mediaCapture_RecordLimitExceeded);

                    // Start Preview                
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        videoCapture.Source = mediaCapture;
                    });

                    await mediaCapture.StartPreviewAsync();
                    await SendResponse("Camera preview succeeded!").ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await SendResponse("Unable to initialize camera for audio/video mode: " + ex.Message).ConfigureAwait(false);
                }
            });
        }
        protected override void CameraOff()
        {
            if (mediaCapture != null)
            {
                try
                {
                    if (isRecording) Task.Run(async () => await mediaCapture.StopPreviewAsync());
                }
                catch (Exception ex)
                {
                    App.HandleException(ex);
                }
                finally
                {
                    mediaCapture.Dispose();
                    mediaCapture = null;
                }

                videoCapture.Source = null;
            }
        }

        /// <summary>
        /// Callback function for any failures in MediaCapture operations
        /// </summary>
        /// <param name="currentCaptureObject"></param>
        /// <param name="currentFailure"></param>
        private async void mediaCapture_Failed(MediaCapture currentCaptureObject, MediaCaptureFailedEventArgs currentFailure)
        {
            await SendResponse("MediaCaptureFailed: " + currentFailure.Message).ConfigureAwait(false);

            if (isRecording)
            {
                await mediaCapture.StopRecordAsync();
                await SendResponse("Recording Stopped").ConfigureAwait(false);
            }
        }
        /// <summary>
        /// Callback function if Recording Limit Exceeded
        /// </summary>
        /// <param name="currentCaptureObject"></param>
        private async void mediaCapture_RecordLimitExceeded(Windows.Media.Capture.MediaCapture currentCaptureObject)
        {
            try
            {
                await SendResponse("Stopping Record on exceeding max record duration").ConfigureAwait(false);
                await mediaCapture.StopRecordAsync();

                isRecording = false;
            }
            catch (Exception e)
            {
                await SendResponse("ERROR: " + e.Message).ConfigureAwait(false);
            }
        }

        private MediaCapture mediaCapture;
        private bool isRecording;
        #endregion

        #region Chatbot

        protected override void InitializeChat()
        {
            base.InitializeChat();

            /*
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(App.Settings.ChatbotDisplayResponseWaitTime)
            };
            timer.Tick += Timer_Tick;
             */

            App.Engine.ExecutiveFunctions.Responding += ExecutiveFunctions_Responding;
            TextToSpeech tts = (TextToSpeech)App.Engine.AI.LanguageModel.SpeechModule.TextToSpeech;
            tts.MediaElement = mediaElement;

            // Meet and Greet
            string responseText = "Well. It's ABOUT, freaking time.";
            App.Engine.AI.LanguageModel.SpeechModule.TextToSpeech.Speak(responseText, this);
        }

        protected override string GetResponse(string inputText, string userName)
        {
            if (inputText.Equals("TEST HTTP LISTENER", StringComparison.CurrentCultureIgnoreCase))
            {
                Galahad.Net.HttpTestClient.TestAPI_Get();
                return null;
            }

            #region // Move to Language Response Manager
            string responseText = null;

            if (inputText.Length > 3 && inputText.Substring(0, 3).Equals("say", StringComparison.CurrentCultureIgnoreCase))
                responseText = inputText.Substring(3, inputText.Length - 3);
            else if (inputText.Length > 5 && inputText.Substring(0, 5).Equals("speak", StringComparison.CurrentCultureIgnoreCase))
                responseText = inputText.Substring(5, inputText.Length - 5);

            if (responseText != null)
            {
                App.Engine.AI.LanguageModel.SpeechModule.TextToSpeech.Speak(responseText, this);
                DisplayResponse(App.Engine.AI.LanguageModel.ChatbotManager.Current.Name, responseText);

                return responseText;
            }
            // TODO:  Move to Language Response Manager
            #endregion

            // Get response to input
            return App.Engine.ExecutiveFunctions.GetResponse(App.Engine.AI.LanguageModel, inputText, userName);  // TODO:  Apply chatbot user logic
        }

        private void ExecutiveFunctions_Responding(object sender, ResponseEventArgs e)
        {
            // Display response
            string responderName = App.Engine.AI.LanguageModel.ChatbotManager.Current.Name;
            string responseText = e.ResponseBag.Output;

            DisplayResponse(responderName, responseText);

            //timer.Start();
        }
        internal async void DisplayResponse(string responderName, string responseText)
        {
            if (responseText.Contains("No match found", StringComparison.CurrentCultureIgnoreCase))
                return;

            string msg = string.Format(CultureInfo.CurrentCulture, ChatbotResources.ChatbotMessageFormat,
                responderName.ToUpper(CultureInfo.CurrentCulture), responseText);

            await SendResponse(msg).ConfigureAwait(false);
        }

        #endregion

        #region Speech Recognition

        [System.Diagnostics.DebuggerNonUserCode]
        protected override void InitializeMicrophone()
        {
            App.Engine.ExecutiveFunctions.SpeechRecognition += ExecutiveFunctions_SpeechRecognition;
            base.InitializeMicrophone();
        }

        private async void ExecutiveFunctions_SpeechRecognition(object sender, SpeechRecognizedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.SpeechText)) return;

            string msg = string.Format(CultureInfo.CurrentCulture, ChatbotResources.ChatbotMessageFormat,
                App.Engine.User.Name.ToUpper(CultureInfo.CurrentCulture), e.SpeechText);

            await SendResponse(msg).ConfigureAwait(false);
        }

        protected override void MicrophoneOn()
        {
            Task.Run(async () =>
            {
                try
                {
                    App.Engine.AI.LanguageModel.SpeechModule.SpeechRecognition.StartListening();
                }
                catch
                {
                    microphoneOnOff.IsChecked = false;
                    throw;
                }

                if (!App.Engine.AI.LanguageModel.SpeechModule.SpeechRecognition.Inactive)
                {
                    await SendResponse("Speech Recognition component is listening...").ConfigureAwait(false);
                }
            });
        }
        protected override void MicrophoneOff()
        {
            try
            {
                if (App.Engine.AI.LanguageModel.SpeechModule == null) return;
                App.Engine.AI.LanguageModel.SpeechModule.SpeechRecognition.StopListening();
            }
            catch (Exception ex)
            {
                App.HandleException(ex);
            }
        }

        #endregion

        #region Robotics

        protected override void SetPinValue(int pinNumber, int value)
        {
            App.Engine.Machine.MotorController.PinCommands[pinNumber].Write(value);
        }

        protected override int GetPinNumber(Button button)
        {
            switch (button.Name)
            {
                case "leftForward": return App.UWPSettings.GpioLeftForwardPin;
                case "leftBack": return App.UWPSettings.GpioLeftReversePin;
                case "rightForward": return App.UWPSettings.GpioRightForwardPin;
                case "rightBack": return App.UWPSettings.GpioRightReversePin;
                default: throw new NotImplementedException();
            }
        }

        #endregion

        public override string ProviderName => "Galahad Main Page";

        internal static MainPage Current { get { return _current; } }

        private static MainPage _current;
    }
}
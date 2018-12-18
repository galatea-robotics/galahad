using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Galahad.UI
{
    using Galatea.Globalization;

    [CLSCompliant(false)]
    public abstract partial class BasePage : Page, IDisposable, Galatea.IProvider
    {
        protected BasePage()
        {
            this.InitializeComponent();
        }

        protected bool StartProcessesOnLoad { get; set; } = true;

        protected virtual void PageLoaded(object sender, RoutedEventArgs e)
        {
            // Initialize Camera
            InitializeVideo();

            // Initialize Chatbot
            InitializeChat();

            // Initialize Speech Recognition
            InitializeMicrophone();

            // Initialize Robotics
            InitializeRobotics();

            // Prepare screen for input
            this.input.Focus(FocusState.Programmatic);
        }

        protected abstract string UserName { get; }

        public string ProviderId { get { return this.GetType().FullName; } }
        public abstract string ProviderName { get; }

        #region Camera
        protected virtual void InitializeVideo()
        {
            cameraOnOff.Checked += CameraOnOff_Checked;
            cameraOnOff.Unchecked += CameraOnOff_Unchecked;

            if (StartProcessesOnLoad) CameraOn();
        }

        protected abstract void CameraOn();

        protected abstract void CameraOff();

        private void CameraOnOff_Checked(object sender, RoutedEventArgs e)
        {
            CameraOn();
        }
        private void CameraOnOff_Unchecked(object sender, RoutedEventArgs e)
        {
            CameraOff();
        }
        #endregion

        #region Chatbot

        protected virtual void InitializeChat()
        {
            response.Text = "";
            input.Text = "";            
        }

        protected async virtual void GetResponse()
        {
            // Why is it firing this event 2x???
            if (responding) return;
            responding = true;

            // Get and display input 
            string inputText = input.Text.Trim();

            // Get Response
            if (!string.IsNullOrEmpty(inputText))
            {
                string msg = string.Format(CultureInfo.CurrentCulture, ChatbotResources.ChatbotMessageFormat, this.UserName, inputText);
                await SendResponse(msg).ConfigureAwait(false);

                // Save input to short term UI History
                history.Add(inputText);
                historyLine = -1;

                // Get response to input
                GetResponse(inputText, this.UserName);
            }

            input.Text = "";
            responding = false;
        }
        protected abstract string GetResponse(string inputText, string userName);

#pragma warning disable CA1801 // Review unused parameters
        private void Input_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if(e.Key == Windows.System.VirtualKey.Enter)
            {
                GetResponse();
                return;
            }

            // Scroll History
            if (history.Count == 0) return;

            // Arrow Up
            if (e.Key == Windows.System.VirtualKey.Up)
            {
                // Go to the last line entered on initial Key Up
                if (historyLine == -1) historyLine = history.Count;

                // Go to the Previous Line
                historyLine--;
            }
            // Arrow Down
            else if (e.Key == Windows.System.VirtualKey.Down)
            {
                // Go to the last line entered on initial Key Up
                if (historyLine == history.Count)
                {
                    input.Text = null;
                    return;
                }

                if (string.IsNullOrEmpty(input.Text))
                {
                    historyLine = -1;
                }

                // Go to the Next Line
                historyLine++;
            }
            else return;

            // Set the input
            input.Text = history[historyLine];
            input.Select(input.Text.Length, 0);
        }
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            GetResponse();
        }
        private void SendButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            GetResponse();
        }
#pragma warning restore CA1801 // Review unused parameters

        private bool responding;
        private List<string> history = new List<string>();
        private int historyLine = -1;
        #endregion

        #region Speech Recognition
        [System.Diagnostics.DebuggerNonUserCode]
        protected virtual void InitializeMicrophone()
        {
            microphoneOnOff.Checked += MicrophoneOnOff_Checked;
            microphoneOnOff.Unchecked += MicrophoneOnOff_Unchecked;

            if (StartProcessesOnLoad) MicrophoneOn();
        }

        protected abstract void MicrophoneOn();
        protected abstract void MicrophoneOff();

        private void MicrophoneOnOff_Checked(object sender, RoutedEventArgs e)
        {
            MicrophoneOn();
        }
        private void MicrophoneOnOff_Unchecked(object sender, RoutedEventArgs e)
        {
            MicrophoneOff();
        }

        #endregion

        #region Robotics

        protected virtual void InitializeRobotics()
        {
            leftForward.Holding += Button_Holding;
            leftBack.Holding += Button_Holding;
            rightForward.Holding += Button_Holding;
            rightBack.Holding += Button_Holding;

            leftForward.PointerPressed += Button_PointerPressed;
            leftBack.PointerPressed += Button_PointerPressed;
            rightForward.PointerPressed += Button_PointerPressed;
            rightBack.PointerPressed += Button_PointerPressed;

            leftForward.PointerReleased += Button_PointerReleased;
            leftBack.PointerReleased += Button_PointerReleased;
            rightForward.PointerReleased += Button_PointerReleased;
            rightBack.PointerReleased += Button_PointerReleased;
        }

        protected abstract void SetPinValue(int pinNumber, int value);

        private void Button_Holding(object sender, HoldingRoutedEventArgs e)
        {
            int pinNumber = GetPinNumber((Button)sender);
            int value = e.HoldingState == Windows.UI.Input.HoldingState.Started ? 1 : 0;
            SetPinValue(pinNumber, value);
        }
        private void Button_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            int pinNumber = GetPinNumber((Button)sender);
            SetPinValue(pinNumber, 0);
        }
        private void Button_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            int pinNumber = GetPinNumber((Button)sender);
            SetPinValue(pinNumber, 1);
        }

        protected virtual int GetPinNumber(Button button)
        {
            switch (button.Name)
            {
                case "leftForward": return 1;
                case "leftBack": return 2;
                case "rightForward": return 3;
                case "rightBack": return 4;
                default: throw new NotImplementedException();
            }
        }

        #endregion

#pragma warning disable CA1801 // Review unused parameters
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }
#pragma warning restore CA1801 // Review unused parameters

        public async Task SendResponse(string message)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // Display response
                response.Text += "\r\n" + message;

                // Scroll all the way down
                response.ScrollToBottom();
            });
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Dispose managed state (managed objects).
                    CameraOff();
                    MicrophoneOff();
                }

                disposedValue = true;
            }

            Disposed?.Invoke(this, EventArgs.Empty);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        public ISite Site { get; set; }

        public event EventHandler Disposed;
    }
}

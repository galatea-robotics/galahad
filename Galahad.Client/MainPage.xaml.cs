using System;
using System.Dynamic;
using System.Globalization;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Galahad.Client
{
    using Galatea.Globalization;
    using Galahad.Client.Properties;
    using Galahad.Client.Net;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Galahad.UI.BasePage, IDisposable
    {
        private readonly static ClientSettings settings = ClientSettings.Load().Result;
        private readonly Dispatcher dispatcher;
        private string responderName;

        public MainPage() : base()
        {
            startProcessesOnLoad = false;
            dispatcher = new Dispatcher(settings.IpAddress, settings.Port);
            responderName = dispatcher.GetResponderName();
                
            _current = this;
        }

        internal static ClientSettings Settings { get { return settings; } }
        protected override string UserName
        {
            get { return settings.UserName; }
        }

        protected override void CameraOff()
        {
            dispatcher.CameraOn = false;
        }
        protected override void CameraOn()
        {
            dispatcher.CameraOn = true;
        }

        protected override string GetResponse(string inputText, string userName)
        {
            string result = dispatcher.GetResponse(userName, inputText);

            string msg = string.Format(CultureInfo.CurrentCulture, ChatbotResources.ChatBotMessageFormat,
                responderName.ToUpper(), result);

            Task.Run(async () =>
            {
                await SendResponse(msg);
            });

            return result;
        }

        protected override void MicrophoneOff()
        {
            dispatcher.MicrophoneOn = false;
        }
        protected override void MicrophoneOn()
        {
            dispatcher.MicrophoneOn = true;
        }

        protected override void SetPinValue(int pinNumber, int value)
        {
            dispatcher.SetPinValue(pinNumber, value);
        }

        internal static MainPage Current { get { return _current; } } // TODO: Step

        private static MainPage _current;
    }
}

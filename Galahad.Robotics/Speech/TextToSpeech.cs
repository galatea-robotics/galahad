using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Galahad.Robotics.Speech
{
    using Galatea.Runtime;
    using Galatea.Speech;

    class TextToSpeech : RuntimeComponent, ITextToSpeech
    {
        private ISpeechModule _speechModule;
        SpeechSynthesizer _speechSynthesizer;
        private List<IVoice> _voices = new List<IVoice>();
        private IVoice _current;
        private class GalaVoice : IVoice
        {
            public Gender Gender { get; set; }
            public string Name { get; set; }
            public string Locale { get; set; }
            public VoiceInformation VoiceInformation { get; set; }
            object IVoice.VoiceObject
            {
                get { return VoiceInformation; }
                set
                {
                    VoiceInformation obj = (VoiceInformation)value;
                    if (obj != null) VoiceInformation = obj;

                    // Bad 
                    throw new ArgumentException("Voice object must be of type 'Windows.Media.SpeechSynthesis.VoiceInformation'.");
                }
            }
        }
        public TextToSpeech(ISpeechModule speechModule)
        {
            // Do Component
            _speechModule = speechModule;
            _speechModule.TextToSpeech = this;
            _speechModule.Add(this);

            // Initialize Voice
            _speechSynthesizer = new SpeechSynthesizer();
            
            // Get available voices
            foreach (var v in SpeechSynthesizer.AllVoices)
            {
                _voices.Add(new GalaVoice
                {
                    Gender = v.Gender.Convert(),
                    Name = v.DisplayName,
                    Locale = v.Language,
                    VoiceInformation = v
                });
            }
        }

        public event EventHandler<MouthPositionEventArgs> MouthPositionChange;
        public event EventHandler<WordEventArgs> Word;
        public event EventHandler SpeechEnded;

        protected internal MediaElement MediaElement
        {
            get { return mediaElement; }
            set
            {
                if (mediaElement != null) mediaElement.MediaEnded -= mediaElement_MediaEnded;

                mediaElement = value;
                mediaElement.MediaEnded += mediaElement_MediaEnded;
            }
        }
        protected void mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            _isSpeaking = false;
            if (SpeechEnded != null) SpeechEnded(this, EventArgs.Empty);
        }

        protected override void Dispose(bool disposing)
        {
            _speechSynthesizer.Dispose();
            mediaElement.MediaEnded -= mediaElement_MediaEnded;

            base.Dispose(disposing);
        }

        IVoice ITextToSpeech.GetVoice(int voiceIndex)
        {
            return _voices[voiceIndex];
        }
        IVoice ITextToSpeech.CurrentVoice
        {
            get { return _current; }
            set
            {
                if (value == null) throw new Galatea.TeaArgumentNullException("value");

                _current = value;
                _speechSynthesizer.Voice = (VoiceInformation)_current.VoiceObject;
            }
        }
        async void ITextToSpeech.Speak(string response, Galatea.IProvider sender)
        {
            if (mediaElement == null) throw new NotImplementedException("MediaElement must be initialized on App startup."); // TODO

            try
            {
                // Correct Punctuation
                response = response.Replace(".", ". ");

                // Initialize a new instance of the SpeechSynthesizer.
                _isSpeaking = true;
                SpeechSynthesisStream stream = await _speechSynthesizer.SynthesizeTextToStreamAsync(response);

                // Send the stream to the media object.
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    mediaElement.Volume = (double)_volume / 100;
                    mediaElement.SetSource(stream, stream.ContentType);
                    mediaElement.Play();
                });
            }
            catch
            {
                _isSpeaking = false;
                throw;
            }

            // Logging
            if (_speechModule.LanguageModel.AI.Engine.Debugger.LogLevel == Galatea.Diagnostics.DebuggerLogLevel.Diagnostic)
            {
                _speechModule.LanguageModel.AI.Engine.Debugger.Log(Galatea.Diagnostics.DebuggerLogLevel.Diagnostic,
                    "TextToSpeech[" + sender.ProviderID + "]: " + response);
            }
        }

        #region Not Implemented
        void ITextToSpeech.PauseTTS()
        {
            throw new NotImplementedException();
        }
        void ITextToSpeech.ResumeTTS()
        {
            throw new NotImplementedException();
        }
        void ITextToSpeech.StopTTS()
        {
            throw new NotImplementedException();
        }
        int ITextToSpeech.Rate
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
        MouthPosition ITextToSpeech.MouthPosition
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
        PhonemeCollection ITextToSpeech.Phonemes
        {
            get { throw new NotImplementedException(); }
        }
        #endregion
        
        bool ITextToSpeech.IsSpeaking { get { return _isSpeaking; } }

        /// <remarks>
        /// Must be an integer between 0 and 100.
        /// </remarks>
        int ITextToSpeech.Volume
        {
            get { return _volume; }
            set { _volume = value; }
        }

        private bool _isSpeaking;
        private int _volume = 50;
        MediaElement mediaElement;
    }
}

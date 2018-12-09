using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galahad.Robotics.Speech
{
    using Galatea.AI;
    using Galatea.Runtime;
    using Galatea.Speech;

    internal class SpeechModule : RuntimeContainer, ISpeechModule
    {
        /// <summary>
        /// Creates a new instance of the <see cref="SpeechModule"/> class.
        /// </summary>
        public SpeechModule()
        {
        }
        /// <summary>
        /// Adds the <see cref="SpeechModule"/> component to the <see cref="CognitiveModelingSystem.LanguageModel"/>
        /// container.
        /// </summary>
        /// <param name="languageModel">
        /// the <see cref="CognitiveModelingSystem.LanguageModel"/> container.
        /// </param>
        public void Initialize(ILanguageAnalyzer languageModel)
        {
            if (languageModel == null) throw new Galatea.TeaArgumentNullException("languageModel");

            _languageAnalyzer = languageModel;

            // Component Model
            languageModel.SpeechModule = this;
        }
        /// <summary>
        /// The Speech Recognition component of the <see cref="SpeechModule"/>.
        /// </summary>
        ISpeechRecognition ISpeechModule.SpeechRecognition
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _speechRecognition; }
            [System.Diagnostics.DebuggerStepThrough]
            set { _speechRecognition = value; }
        }
        /// <summary>
        /// The Text-to-Speech component of the <see cref="SpeechModule"/>.
        /// </summary>
        ITextToSpeech ISpeechModule.TextToSpeech
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _textToSpeech; }
            [System.Diagnostics.DebuggerStepThrough]
            set { _textToSpeech = value; }
        }

        bool ISpeechModule.StaySilent
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _silent; }
            [System.Diagnostics.DebuggerStepThrough]
            set { _silent = value; }
        }

        ILanguageAnalyzer ISpeechModule.LanguageModel { [System.Diagnostics.DebuggerStepThrough] get { return _languageAnalyzer; } }

        #region Component Designer generated code

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion

        private ILanguageAnalyzer _languageAnalyzer;
        private ISpeechRecognition _speechRecognition;
        private ITextToSpeech _textToSpeech;
        private bool _silent;
    }
}

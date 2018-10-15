using Galatea.AI;
using Galatea.AI.Imaging;
using Galatea.Diagnostics;
using Galatea.Runtime;
using Galatea.Runtime.Services;

namespace Gala.Dolly.Test
{
    using Galatea.AI.Abstract;
    using Gala.Data;

    internal class TestEngine : Engine, IRuntimeEngine, IEngine
    {
        public TestEngine(IDebugger debugger, DataAccessManager dataAccessManager)
        {
            // Initialize Debugger before anything else
            this.Debugger = debugger;

            // Initialize Database
            _dataAccessManager = dataAccessManager;

            // Initialize Engine
            Initialize();
        }

        internal void Initialize()
        {
            // Initialize Foundation Components
            Galatea.AI.Robotics.SensoryMotorSystem machine = new Galatea.AI.Robotics.Machine();
            machine.Initialize(this);

            VisualProcessor vision = new VisualProcessor();
            vision.Initialize(this);

            // Become Self-Aware
            IRobot robot = SelfAwareness.BecomeSelfAware(this, "Skynet");
            Gala.Data.Runtime.ContextCache newContextCache = new Data.Runtime.ContextCache();
            newContextCache.Initialize(this.ExecutiveFunctions);
            // Verify that ContextCache is instantiated
            System.Diagnostics.Debug.Assert(ExecutiveFunctions.ContextCache != null);

            // Initialize Language Module
            this.User = new Galatea.Runtime.Services.User(Properties.Settings.Default.DefaultUserName);
            IChatbotManager chatbots = new Gala.Dolly.Test.ChatbotManager();
            robot.LanguageModel.LoadChatBots(chatbots);

            System.Collections.Specialized.StringCollection substitutions = new System.Collections.Specialized.StringCollection();
            substitutions.Add("I ,eye ");
            substitutions.Add(".,.  ");
            substitutions.Add("Ayuh,If you say so|false");
            robot.LanguageModel.LoadSubstitutions(substitutions);

            /*
            Galatea.Speech.ISpeechModule speech = new Galatea.Speech.SpeechModule();
            speech.Initialize(robot.LanguageModel);
            speech.StaySilent = Properties.Settings.Default.SpeechIsSilent;

            if (!speech.StaySilent)
            {
                /*
                //_engine.Machine.SpeechModule = _speech;
                Galatea.Speech.TextToSpeech5 tts5 = new Galatea.Speech.TextToSpeech5(speech);


                // ********** WIN 10 ZIRA ********** // 
                SpeechLib.SpVoice spV = tts5.GetSpeechObject() as SpeechLib.SpVoice;

                int defaultVoiceIndex = Properties.Settings.Default.TextToSpeechDefaultVoiceIndex;
                SpeechLib.SpObjectToken spVoice = tts5.GetVoice(defaultVoiceIndex) as SpeechLib.SpObjectToken;
                spV.Voice = spVoice;

                string d = spV.Voice.GetDescription();
                Debugger.Log(Galatea.Diagnostics.DebuggerLogLevel.Log, string.Format("SpeechLib.SpVoice: {0}", d));
                 * /
            }
             */

            _ai = robot;

            // Apply default conversational labels
            _ai.LanguageModel.ChatbotManager.Current = new Chatbot(Properties.Settings.Default.ChatBotName);

            InitializeDatabase();
        }
        public void InitializeDatabase()
        {
            // Add Memory
            _dataAccessManager.Initialize(this);
            _dataAccessManager.InitializeMemoryBank();

            _ai.InitializeMemory(_dataAccessManager);

            // Set Application Settings
            ((ColorTemplateCollection)_dataAccessManager[TemplateType.Color]).HybridResultThreshold = Properties.Settings.Default.ColorTemplateHybridResultThreshold;
        }

        public override void Startup()
        {
            base.Startup();
        }

        public override void Shutdown()
        {
            _dataAccessManager.SaveAll();
            base.Shutdown();

            /*
            _dataAccessManager.FileLogger.StopLogging();
             */
        }

        internal new DataAccessManager DataAccessManager
        {
            get { return _dataAccessManager; }
            set { _dataAccessManager = value; }
        }
        internal new IRobot AI { get { return _ai; } }

        private static DataAccessManager _dataAccessManager;
        private static IRobot _ai;
        //private static Galatea.Speech.TextToSpeech5 _tts5;
    }
}

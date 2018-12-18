using System;
using System.Reflection;
using Windows.Storage;

namespace Galahad.Robotics
{
    //using Gala.Data.Configuration;
    using Galahad.Properties;
    using Galatea.AI;
    using Galatea.AI.Imaging;
    using Galatea.AI.Robotics;
    using Galatea.Runtime;
    using Galatea.Runtime.Services;

    internal class Engine : Galatea.Runtime.Services.RuntimeEngine, IRuntimeEngine, IEngine
    {
        private static UWPSettings _settings;
        private static Galahad.Robotics.Debugger _debugger;

        internal Engine(UWPSettings settings) : this(settings, null)
        {
        }
        internal Engine(UWPSettings settings, Debugger debugger) : base()
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));

            // Initialize Debugger
            try
            {
                if (debugger == null) debugger = new Debugger();

                _debugger = debugger;
                this.Debugger = _debugger;

                if (!_debugger.FileLogger.IsLogging)
                {
                    // Initialize Log File
                    Assembly thisAsm = typeof(Engine).GetTypeInfo().Assembly;

                    System.Threading.Tasks.Task.Run(async () =>
                    {
                        try
                        {
                            StorageFile logFile = await thisAsm.GetStorageFile(ApplicationData.Current.LocalFolder, "Galahad.log").ConfigureAwait(false);

                            _debugger.FileLogger.StartLogging(logFile.Path, System.IO.FileMode.Append);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.Message);
                            throw;
                        }
                    });
                }

                debugger = null;
            }
            finally
            {
                if (debugger != null) debugger.Dispose();
            }
        }

        public override void Startup()
        {
            // Load Components
            Initialize(_settings.DataAccessManagerConnectionString);

            // Validate 
            base.Startup();

            // Notify UI

        }

        protected override void Dispose(bool disposing)
        {
            _debugger.FileLogger.StopLogging();
            _debugger.Dispose();

            base.Dispose(disposing);
        }

        internal static UWPSettings Settings { get { return _settings; } }

        internal event EventHandler<EngineInitializationEventArgs> InitializationStatusUpdated;

        private void Initialize(string connectionString)
        {
            IDataAccessManager database = null;
            SensoryMotorSystem machine = null;
            VisualProcessingSystem vision = null;
            Gala.Data.Runtime.ContextCache newContextCache = null;
            IRobot robot = null;
            IChatbotManager chatbots = null;
            Galatea.Speech.ISpeechModule speech = null;
            Galatea.Speech.ITextToSpeech tts = null;
            Galatea.Speech.ISpeechRecognition sr = null;

            try
            {
                // Load Memory Library
                database = new Gala.Data.Databases.SerializedDataAccessManager(connectionString);
                database.Initialize(this);

                // Load Robotics Interfaces
                InitializationStatusUpdated?.Invoke(this, new EngineInitializationEventArgs(2, EngineInitializationResources.Engine_Initializing_Machine));
                machine = new Galahad.Robotics.MachineSystem();
                machine.Initialize(this);

                InitializationStatusUpdated?.Invoke(this, new EngineInitializationEventArgs(20, EngineInitializationResources.Engine_Initializing_Visuals));
                vision = new VisualProcessor(_settings.ImagingSettings);
                vision.Initialize(this);

                // Become Self-Aware
                InitializationStatusUpdated?.Invoke(this, new EngineInitializationEventArgs(33, EngineInitializationResources.Engine_Initializing_AI));
                robot = SelfAwareness.BecomeSelfAware(this, "Galahad");
                newContextCache = new Gala.Data.Runtime.ContextCache();
                newContextCache.Initialize(this.ExecutiveFunctions);

                // Initialize Language Module
                InitializationStatusUpdated?.Invoke(this, new EngineInitializationEventArgs(50, EngineInitializationResources.Engine_Initializing_Language));
                this.User = new Galatea.Runtime.Services.User(_settings.DefaultUserName);
                chatbots = new ChatbotManager(this.User, _settings);
                robot.LanguageModel.LoadChatbots(chatbots);

                // Add Text to Speech (even if silent)
                speech = new Galahad.Robotics.Speech.SpeechModule();
                speech.Initialize(robot.LanguageModel);
                speech.StaySilent = _settings.SpeechIsSilent;

                InitializationStatusUpdated?.Invoke(this, new EngineInitializationEventArgs(65, EngineInitializationResources.Engine_Initializing_Language_Speech));
                tts = new Galahad.Robotics.Speech.TextToSpeech(speech);
                tts.CurrentVoice = tts.GetVoice(0);
                InitializationStatusUpdated?.Invoke(this, new EngineInitializationEventArgs(80, EngineInitializationResources.Engine_Initializing_Language_Recognition));
                sr = new Galahad.Robotics.Speech.SpeechRecognition(speech);

                // Add Memory
                InitializationStatusUpdated?.Invoke(this, new EngineInitializationEventArgs(90, EngineInitializationResources.Engine_Initializing_Memory));
                robot.InitializeMemory(database);

                // Finalize
                database = null;
                machine = null;
                vision = null;
                robot = null;
                newContextCache = null;
                chatbots = null;
                speech = null;
                tts = null;
                sr = null;
            }
            finally
            {
                if (database != null) database.Dispose();
                if (machine != null) machine.Dispose();
                if (vision != null) vision.Dispose();
                if (robot != null) robot.Dispose();
                if (newContextCache != null) newContextCache.Dispose();
                if (chatbots != null) chatbots.Dispose();
                if (speech != null) speech.Dispose();
                if (sr != null) sr.Dispose();
            }
        }
    }
}

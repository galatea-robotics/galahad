using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Galatea;
using Galatea.AI.Robotics;
using Galatea.Runtime;

namespace Galahad.Robotics
{
    using Galahad.Robotics.MotorControls;

    internal class MachineSystem : RuntimeContainer, SensoryMotorSystem
    {
        void IFoundation.Initialize(IEngine engine)
        {
            // Initialize
            _engine = engine ?? throw new TeaArgumentNullException(nameof(engine));
            _engine.Add(this);

            _isInitialized = true;

            // Add sub-components
            _motorController = new MotorController();
            _motorController.Initialize(this);
            _sensorAnalyzer = new SensorAnalyzer();
            _sensorAnalyzer.Initialize(this);
        }
        bool IFoundation.IsInitialized { get { return _isInitialized; } }
        IEngine IFoundation.Engine { get { return _engine; } }  // TODO:  Step

        IMotorController SensoryMotorSystem.MotorController { get { return _motorController; } }
        ISensorAnalyzer SensoryMotorSystem.SensorAnalyzer { get { return _sensorAnalyzer; } }
        ISerialPortController SensoryMotorSystem.SerialPortController { get { throw new NotImplementedException(); } }

        private bool _isInitialized;
        private IMotorController _motorController;
        private ISensorAnalyzer _sensorAnalyzer;
        private IEngine _engine;
    }
}

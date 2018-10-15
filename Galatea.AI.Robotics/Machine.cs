using System;
using Galatea.Runtime;
using Galatea.Speech;

namespace Galatea.AI.Robotics
{
    /// <summary>
    /// A running process that contains Artificial Intelligence methods for hardware 
    /// feedback and control.
    /// </summary>
    internal sealed class Machine : RuntimeContainer, SensoryMotorSystem
    {
        void IFoundation.Initialize(IEngine engine)
        {
            if (engine == null)
                throw new TeaArgumentNullException("engine");

            // Initialize
            _engine = engine;
            _engine.Add(this);

            _isInitialized = true;

            // Add sub-components
            _serialPort = new SerialPortController();
            _serialPort.Initialize(this);
        }
        bool IFoundation.IsInitialized { get { return _isInitialized; } }

        #region Not Implemented

        /// <summary>
        /// Gets a reference to the Motor Controller component of the 
        /// <see cref="Galatea.AI.Robotics.SensoryMotorSystem"/>.
        /// </summary>
        IMotorController SensoryMotorSystem.MotorController { get { throw new NotImplementedException(); } }
        /// <summary>
        /// Gets a reference to the Sensor Feedback Processing component 
        /// of the <see cref="Galatea.AI.Robotics.SensoryMotorSystem"/>.
        /// </summary>
        ISensorAnalyzer SensoryMotorSystem.SensorAnalyzer { get { throw new NotImplementedException(); } }

        #endregion

        ISerialPortController SensoryMotorSystem.SerialPortController { get { return _serialPort; } }

        IEngine IFoundation.Engine { get { return _engine; } }

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

        private bool _isInitialized;
        private ISerialPortController _serialPort;
        private IEngine _engine;
    }
}

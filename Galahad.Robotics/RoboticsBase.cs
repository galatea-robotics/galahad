using Galatea;
using Galatea.AI.Robotics;
using Galatea.Runtime;

namespace Galahad.Robotics
{
    internal class RoboticsBase : RuntimeContainer, IController
    {
        void IController.Initialize(SensoryMotorSystem machine)
        {
            if (machine == null) throw new TeaArgumentNullException("machine");
            if (!machine.IsInitialized) throw new TeaInitializationException(machine, this);

            _machine = machine;
            _machine.Add(this);

            _isInitialized = true;
        }

        bool IController.IsInitialized { get { return _isInitialized; } }
        SensoryMotorSystem IController.Machine { get { return _machine; } }

        protected SensoryMotorSystem Machine { get { return _machine; } }

        private bool _isInitialized;
        private SensoryMotorSystem _machine;
    }
}

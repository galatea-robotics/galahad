using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace Galahad.Robotics.MotorControls
{
    using Galatea.AI.Robotics;
    using Galatea.Runtime;

    internal class MotorController : RoboticsBase, IMotorController, IPinController
    {
        public MotorController()
        {
            _current = this;
            PinCommands = new PinCommandCollection();
        }

        protected override void Initialize(SensoryMotorSystem machine)
        {
            Task.Run(() =>
            {
                base.Initialize(machine);

                GpioController controller = GpioController.GetDefault();
                PinCommand leftForwardPinCommand = new PinCommand(Engine.Settings.GpioLeftForwardPin, GpioPinDriveMode.Output, controller);
                PinCommand leftReversePinCommand = new PinCommand(Engine.Settings.GpioLeftReversePin, GpioPinDriveMode.Output, controller);
                PinCommand rightForwardPinCommand = new PinCommand(Engine.Settings.GpioRightForwardPin, GpioPinDriveMode.Output, controller);
                PinCommand rightReversePinCommand = new PinCommand(Engine.Settings.GpioRightReversePin, GpioPinDriveMode.Output, controller);

                if (leftForwardPinCommand.GpioPin != null) PinCommands.Add(leftForwardPinCommand);
                if (leftReversePinCommand.GpioPin != null) PinCommands.Add(leftReversePinCommand);
                if (rightForwardPinCommand.GpioPin != null) PinCommands.Add(rightForwardPinCommand);
                if (rightReversePinCommand.GpioPin != null) PinCommands.Add(rightReversePinCommand);

                //MotorCommands.MoveForward = new MultiPinMotorCommand
                //{

                //}
            });
        }

        public void SendCommand(IMotorCommand command)
        {
            throw new NotImplementedException();
        }

        public IMotorCommandCollection MotorCommands { get; }

        public IPinCommandCollection PinCommands { get; }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            // Close Gpio Pins
            PinCommands.Dispose();
        }

        internal static IMotorController Current { get { return _current; } } // TODO:  Step 

        private static IMotorController _current;
    }
}

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
            _pinCommands = new PinCommandCollection();
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

               if (leftForwardPinCommand.GpioPin != null) _pinCommands.Add(leftForwardPinCommand);
               if (leftReversePinCommand.GpioPin != null) _pinCommands.Add(leftReversePinCommand);
               if (rightForwardPinCommand.GpioPin != null) _pinCommands.Add(rightForwardPinCommand);
               if (rightReversePinCommand.GpioPin != null) _pinCommands.Add(rightReversePinCommand);

                //MotorCommands.MoveForward = new MultiPinMotorCommand
                //{

                //}
            });
        }

        public void SendCommand(IMotorCommand command)
        {
            throw new NotImplementedException();
        }

        public IMotorCommandCollection MotorCommands { get { return _motorCommands; } }

        public IPinCommandCollection PinCommands { get { return _pinCommands; } }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            // Close Gpio Pins
            _pinCommands.Dispose();
        }

        internal static IMotorController Current { get { return _current; } } // TODO:  Step

        private readonly IMotorCommandCollection _motorCommands;
        private readonly IPinCommandCollection _pinCommands;

        private static IMotorController _current;
    }
}

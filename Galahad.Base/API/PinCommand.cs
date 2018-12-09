using System;
using Windows.Devices.Gpio;

namespace Galahad.API
{
    using Galatea.AI.Robotics;
    using Galatea.Runtime;

    public class PinCommand : RuntimeComponent, IMotorCommand
    {
        public PinCommand(int pin) : this(pin, null)
        {
        }
        public PinCommand(int pin, GpioController controller)
        {
            if (controller == null) controller = GpioController.GetDefault();
            gpioPin = controller.OpenPin(pin);

            Pin = pin;
        }

        public int Pin { get; }

        public virtual void Execute(string command, params object[] args)
        {

        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                gpioPin.Dispose();
            }

            gpioPin = null;
        }

        private GpioPin gpioPin;
    }
}
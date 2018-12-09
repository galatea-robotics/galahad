using System;
using Windows.Devices.Gpio;

namespace Galahad.Robotics.MotorControls
{
    using Galatea.AI.Robotics;
    using Galatea.Diagnostics;
    using Galatea.Runtime;

    internal class PinCommand : RuntimeComponent, IPinCommand
    {
        private GpioPin gpioPin;

        public PinCommand(int pin, GpioPinDriveMode driveMode, GpioController controller)
        {
            if (controller == null) controller = GpioController.GetDefault();

            try
            {
                // Validate GpioController again
                if (controller == null)
                {
                    throw new TeaInitializationException("Unable to initialize a GpioController."); // TODO
                }
                try
                {
                    gpioPin = controller.OpenPin(pin);
                    gpioPin.SetDriveMode(driveMode);
                }
                catch (System.Runtime.InteropServices.COMException ex)
                {
                    throw new TeaInitializationException($"Cannot open Gpio Pin #{pin}.", ex); // TODO
                }
            }
            catch(TeaInitializationException e)
            {
                MotorController.Current.Machine.Engine.Debugger.HandleTeaException(e, MotorController.Current);
                // Log error but don't break
            }
        }

        public int Pin { get { return gpioPin.PinNumber; } }

        void IPinCommand.Write(object data)
        {
            GpioPinValue value = (GpioPinValue)data;

            if(MotorController.Current.Machine.Engine.Debugger.LogLevel == DebuggerLogLevel.Diagnostic)
            {
                MotorController.Current.Machine.Engine.Debugger.Log(DebuggerLogLevel.Diagnostic, $"Pin Out [{gpioPin.PinNumber}:{value}]");
            }

            this.Write(value);
        }
        object IPinCommand.Read()
        {
            GpioPinValue result = this.Read();

            switch (result)
            {
                case GpioPinValue.Low: return false;
                case GpioPinValue.High: return true;
                default: throw new NotImplementedException(result.ToString());
            }            
        }

        protected internal GpioPin GpioPin { get { return gpioPin; } }

        protected void Write(GpioPinValue value)
        {
            gpioPin.Write(value);
        }
        protected GpioPinValue Read()
        {
            return gpioPin.Read();
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
    }
}
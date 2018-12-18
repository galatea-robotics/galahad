using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Devices.Gpio;

namespace Galahad.Robotics.MotorControls
{
    using Galatea.AI.Robotics;

    internal class MotorCommands : KeyedCollection<string, IMotorCommand>, IMotorCommandCollection
    {
        public MotorCommands()
        {
            this.Add(new MultiPinMotorCommand("MoveForward")
            {
                { MotorController.Current.PinCommands[Engine.Settings.GpioLeftForwardPin], p => p.Write(GpioPinValue.High) },
                { MotorController.Current.PinCommands[Engine.Settings.GpioRightForwardPin], p => p.Write(GpioPinValue.High) },
                { MotorController.Current.PinCommands[Engine.Settings.GpioLeftReversePin], p => p.Write(GpioPinValue.Low) },
                { MotorController.Current.PinCommands[Engine.Settings.GpioRightReversePin], p => p.Write(GpioPinValue.Low) }
            });

            this.Add(new MultiPinMotorCommand("MoveBackward")
            {
                { MotorController.Current.PinCommands[Engine.Settings.GpioLeftReversePin], p => p.Write(GpioPinValue.High) },
                { MotorController.Current.PinCommands[Engine.Settings.GpioRightReversePin], p => p.Write(GpioPinValue.High) },
                { MotorController.Current.PinCommands[Engine.Settings.GpioLeftForwardPin], p => p.Write(GpioPinValue.Low) },
                { MotorController.Current.PinCommands[Engine.Settings.GpioRightForwardPin], p => p.Write(GpioPinValue.Low) }
            });
        }

        protected override string GetKeyForItem(IMotorCommand item)
        {
            return item.CommandName;
        }

        IMotorCommand IMotorCommandCollection.MoveForward { get { return this["MoveForward"]; } }
        IMotorCommand IMotorCommandCollection.MoveBackward { get { return this["MoveBackward"]; } }





        IMotorCommand IMotorCommandCollection.RotateLeft
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        IMotorCommand IMotorCommandCollection.RotateRight
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        IMotorCommand IMotorCommandCollection.Halt
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        IMotorCommand IMotorCommandCollection.TurnLeft
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        IMotorCommand IMotorCommandCollection.TurnRight
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}

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
            var moveForwardCommand = new MultiPinMotorCommand("MoveForward");
            moveForwardCommand.Add(MotorController.Current.PinCommands[Engine.Settings.GpioLeftForwardPin], p => p.Write(GpioPinValue.High));
            moveForwardCommand.Add(MotorController.Current.PinCommands[Engine.Settings.GpioRightForwardPin], p => p.Write(GpioPinValue.High));
            moveForwardCommand.Add(MotorController.Current.PinCommands[Engine.Settings.GpioLeftReversePin], p => p.Write(GpioPinValue.Low));
            moveForwardCommand.Add(MotorController.Current.PinCommands[Engine.Settings.GpioRightReversePin], p => p.Write(GpioPinValue.Low));
            this.Add(moveForwardCommand);

            var moveBackwardCommand = new MultiPinMotorCommand("MoveBackward");
            moveBackwardCommand.Add(MotorController.Current.PinCommands[Engine.Settings.GpioLeftReversePin], p => p.Write(GpioPinValue.High));
            moveBackwardCommand.Add(MotorController.Current.PinCommands[Engine.Settings.GpioRightReversePin], p => p.Write(GpioPinValue.High));
            moveForwardCommand.Add(MotorController.Current.PinCommands[Engine.Settings.GpioLeftForwardPin], p => p.Write(GpioPinValue.Low));
            moveForwardCommand.Add(MotorController.Current.PinCommands[Engine.Settings.GpioRightForwardPin], p => p.Write(GpioPinValue.Low));
            this.Add(moveBackwardCommand);


            //this.Add(new MotorCommand("MoveBackward",                {

            //    MotorController.Current.PinCommands[Engine.Settings.GpioLeftForwardPin].Write(Windows.Devices.Gpio.GpioPinValue.High);
            //MotorController.Current.PinCommands[Engine.Settings.GpioRightForwardPin].Write(Windows.Devices.Gpio.GpioPinValue.High);
            //    });

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

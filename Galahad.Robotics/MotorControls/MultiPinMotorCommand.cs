﻿using System;
using System.Collections.Generic;

namespace Galahad.Robotics.MotorControls
{
    using Galatea.AI.Robotics;

    internal class MultiPinMotorCommand : Dictionary<IPinCommand, Action<IPinCommand>>, IMotorCommand
    {
        internal MultiPinMotorCommand(string commandName)
        {
            CommandName = commandName;
        }

        public string CommandName { get; }

        void IMotorCommand.Execute(params object[] args)
        {
            System.Threading.Tasks.Parallel.ForEach(this, (cmd) =>
            {
                IPinCommand pin = cmd.Key;
                Action<IPinCommand> action = cmd.Value;

                action(pin);
            });
        }
    }
}
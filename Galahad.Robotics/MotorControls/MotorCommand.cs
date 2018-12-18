using System;
using System.Runtime;
using Galatea.AI.Robotics;
using Galatea.Runtime;

namespace Galahad.Robotics.MotorControls
{
    internal class MotorCommand : RuntimeComponent, IMotorCommand
    {
        internal MotorCommand (string commandName, Action<object> exec)
        {
            CommandName = commandName;
            Exec = exec;
        }

        public string CommandName { get; }

        protected internal Action<object> Exec { get; }

        void IMotorCommand.Execute(params object[] args)
        {
            this.Execute(args);
        }

        protected virtual void Execute(params object[] args)
        {
            Exec(args[0]);
        }
    }
}

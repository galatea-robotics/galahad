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
            _commandName = commandName;
            _exec = exec;
        }

        public string CommandName { get { return _commandName; } }

        protected internal Action<object> Exec { get { return _exec; } }

        void IMotorCommand.Execute(params object[] args)
        {
            this.Execute(args);
        }

        protected virtual void Execute(params object[] args)
        {
            _exec(args[0]);
        }


        private string _commandName;
        private Action<object> _exec;
    }
}

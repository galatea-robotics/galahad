using System;
using System.Collections.ObjectModel;

namespace Galahad.API
{
    using Galatea.AI.Robotics;

    public class MultiPinMotorCommand : KeyedCollection<int, IMotorCommand>, IMotorCommands
    {
        int IMotorCommand.Pin { get { throw new NotImplementedException(); } }

        void IMotorCommand.Execute(string command, params object[] args)
        {
            System.Threading.Tasks.Parallel.ForEach(this, (cmd) =>
            {
                cmd.Execute(command);
            });
        }

        protected override int GetKeyForItem(IMotorCommand item)
        {
            return item.Pin;
        }
    }
}
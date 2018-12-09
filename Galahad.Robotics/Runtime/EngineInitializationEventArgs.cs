using System;

namespace Galahad.Robotics
{
    internal class EngineInitializationEventArgs : EventArgs
    {
        public EngineInitializationEventArgs(int progress, string message)
        {
            this.Progress = progress;
            this.Message = message;
        }
        public int Progress { get; }
        public string Message { get; }
    }
}
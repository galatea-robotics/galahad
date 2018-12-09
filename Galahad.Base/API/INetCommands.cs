using System;

namespace Galahad.API
{
    /// <summary>
    /// Provides a common method interface for Http Client and 
    /// Http Service components.
    /// </summary>
    public interface INetCommands
    {
        /// <summary>
        /// Gets or sets a value determining the Robotics camera status.
        /// </summary>
        bool CameraOn { get; set; }
        /// <summary>
        /// Gets or sets a value determining the Robotics microphone status.
        /// </summary>
        bool MicrophoneOn { get; set; }

        void SetPinValue(int pinNumber, int value);

        /// <summary>
        /// Sends a text input to the Robotics AI language component.
        /// </summary>
        string GetResponse(string userName, string input);
    }
}

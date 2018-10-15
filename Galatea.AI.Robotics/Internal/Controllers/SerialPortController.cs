using System;
using System.Globalization;
using System.IO.Ports;

namespace Galatea.AI.Robotics
{
    using Galatea.Diagnostics;
    using Galatea.Globalization;

    internal class SerialPortController : RoboticsBase, ISerialPortController
    {
        private SerialPort serialPort;

        internal SerialPortController()
        {
            serialPort = new SerialPort();
            base.Add(serialPort, serialPort.GetType().FullName);

            this.Disposed += SerialPortController_Disposed;
        }

        private void SerialPortController_Disposed(object sender, EventArgs e)
        {
            if(serialPort.IsOpen)
            {
                // Close the Port
                serialPort.Close();
            }
        }

        void ISerialPortController.OpenSerialPort(string portName, int baudRate)
        {
            if (serialPort.IsOpen)
            {
                Machine.Engine.Debugger.Log(DebuggerLogLevel.Warning, RoboticsResources.SerialPortAlreadyOpen);
                return;
            }

            serialPort.PortName = portName;
            serialPort.BaudRate = baudRate;
            
            // TODO: Handle if Port is unavailable
            serialPort.Open();

            // Log Open Event
            Machine.Engine.Debugger.Log(DebuggerLogLevel.Event, RoboticsResources.SerialPortOpenSuccess);
        }

        void ISerialPortController.CloseSerialPort()
        {
            serialPort.Close();

            // Log Close Event
            Machine.Engine.Debugger.Log(DebuggerLogLevel.Event, RoboticsResources.SerialPortClosing);
        }

        void ISerialPortController.SendCommand(int command)
        {
            if (!serialPort.IsOpen)
            {
                if (!_disableWarning)
                    Machine.Engine.Debugger.Log(DebuggerLogLevel.Warning, RoboticsResources.SerialPortClosed);

                _disableWarning = true;
                return;
            }
            // Send the Serial Data through the Port
            serialPort.WriteLine(command.ToString(CultureInfo.InvariantCulture));

            // Diagnostic
            Machine.Engine.Debugger.Log(DebuggerLogLevel.Diagnostic, string.Format(CultureInfo.InvariantCulture, RoboticsResources.SerialDataSent, command));
        }

        int ISerialPortController.WaitInterval { get { return _waitInterval; } set { _waitInterval = value; } }

        bool ISerialPortController.DisableWarning
        {
            get { return _disableWarning; }
            set
            {
                _disableWarning = value;

                Machine.Engine.Debugger.Log(DebuggerLogLevel.Event,
                    string.Format(CultureInfo.InvariantCulture,
                        RoboticsResources.SerialPortLogWarningsChanged, value ?
                        RoboticsResources.SerialPortLogWarningsDisabledToken :
                        RoboticsResources.SerialPortLogWarningsEnabledToken));
            }
        }

        bool ISerialPortController.IsSerialPortOpen { get { return serialPort.IsOpen; } }

        /*
        public SerialPort SerialPort
        {
            get { return _serialPort; }
        }
         */

        private int _waitInterval;
        private bool _disableWarning;
    }
}

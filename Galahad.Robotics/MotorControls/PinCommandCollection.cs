using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Devices.Gpio;

namespace Galahad.Robotics.MotorControls
{
    using Galatea.AI.Robotics;

    internal class PinCommandCollection : KeyedCollection<int, IPinCommand>, IPinCommandCollection
    {
        protected override int GetKeyForItem(IPinCommand item)
        {
            return item.Pin;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects).
                    foreach (IPinCommand cmd in this)
                    {
                        cmd.Dispose();
                    }
                }

                disposedValue = true;
            }
        }

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}

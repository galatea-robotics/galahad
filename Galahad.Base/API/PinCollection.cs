using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace Galahad.API
{
    public class PinCollection : KeyedCollection<int, GpioPin>
    {
        public PinCollection()
        {
        }

        protected override int GetKeyForItem(GpioPin item)
        {
            return item.PinNumber;
        }
    }
}

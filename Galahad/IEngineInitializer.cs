using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galahad
{
    using Galahad.Robotics;

    interface IEngineInitializer
    {
        void OnEngineInitializationStatusUpdated(object sender, EngineInitializationEventArgs e);
        void OnEngineStartupComplete(object sender, EventArgs e);
    }
}

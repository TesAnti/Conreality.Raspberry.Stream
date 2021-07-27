using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Inspectron.CovidTest.Raspberry
{
    /// <summary>
    /// Supports work with only one camera assuming that it's connected
    /// </summary>
    public class RaspberryCameraProvider : ICameraProvider
    {
        public string Name => "Raspberry camera";

        public ReadOnlyCollection<ICamera> Cameras => new ReadOnlyCollection<ICamera>(new List<ICamera>(){ new RaspberryCamera(this) });

        public ReadOnlyCollection<ICamera> Discover()
        {
            return Cameras;
        }
    }
}
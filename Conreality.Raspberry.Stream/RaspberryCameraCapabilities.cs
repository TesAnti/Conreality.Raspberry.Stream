namespace Inspectron.CovidTest.Raspberry
{
    public class RaspberryCameraCapabilities : ICameraCapabilities
    {
        public bool CanGrabSingle => false;

        public bool CanGrabContinuous => true;

        public bool CanSaveParametersToFile => false;

        public bool CanSaveParametersToDevice => false;
    }
}
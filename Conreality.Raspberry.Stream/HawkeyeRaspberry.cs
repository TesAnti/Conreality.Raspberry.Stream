using System.Collections.Concurrent;

namespace Conreality.Raspberry.Stream
{
    internal class HawkeyeRaspberry
    {
        public HawkeyeRaspberry()
        {
            _camera = new RaspberryCamera();
            
        }

        public void Init()
        {
            _camera.Open();
            
            _camera.ImageGrabbed += Camera_ImageGrabbed;
            _camera.StartGrabContinuous();
        }
        public ConcurrentQueue<byte[]> _images = new ConcurrentQueue<byte[]>();
        private RaspberryCamera _camera;

        private void Camera_ImageGrabbed(RaspberryCamera sender, byte[] image)
        {
            if (_images.Count < 5)
            {
                _images.Enqueue(image);
            }
        }

        public byte[] GetImage()
        {
            byte[] bmp;
            while (!_images.TryDequeue(out bmp))
            {
                
            }

            return bmp;
        }


       
    }
}
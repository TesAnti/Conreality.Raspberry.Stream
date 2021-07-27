using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inspectron.CovidTest.Raspberry.Image;
using MMALSharp;
using MMALSharp.Common;
using MMALSharp.Common.Utility;
using MMALSharp.Components;
using MMALSharp.Config;
using MMALSharp.Native;
using MMALSharp.Ports;

namespace Inspectron.CovidTest.Raspberry
{
    
    public class RaspberryCamera :ICamera
    {
        private VideoHandler _imgCaptureHandler;
        private MMALSplitterComponent _splitter;
        private MMALImageEncoder _imgEncoder;
        private MMALNullSinkComponent _nullSink;
        private CancellationTokenSource _cancellationSource;
        private int _width=640;
        private int _height=480;


        public RaspberryCamera(ICameraProvider provider)
        {
            Provider = provider;
        }

        public string Name => "Raspberry main camera";

        public bool IsOpen { get; private set; }

        public bool IsGrabbingContinuous { get; private set; }

        public ICameraCapabilities Capabilities => new RaspberryCameraCapabilities();

        public IEnumerable<ICameraParameter> Parameters => new List<ICameraParameter>()
        {
            new CameraParameter("Width",()=>_width,x=>_width=(int)x),
            new CameraParameter("Height",()=>_height,x=>_height=(int)x)
        };

        public ICameraProvider Provider { get; private set; }

        public event ImageGrabbedHandler ImageGrabbed=delegate{ };

        public void Open()
        {
            if(IsOpen)throw new Exception("Camera already open");
            IsOpen = true;
            MMALCameraConfig.VideoResolution = Resolution.As1080p;
            MMALCameraConfig.Flips = MMAL_PARAM_MIRROR_T.MMAL_PARAM_MIRROR_BOTH;
            MMALCameraConfig.VideoStabilisation = false;
            MMALCameraConfig.StillResolution=Resolution.As1080p;
            MMALCameraConfig.InlineHeaders = true;
            MMALCameraConfig.SensorMode = MMALSensorMode.Mode4;
            _imgCaptureHandler = new VideoHandler();
            _splitter = new MMALSplitterComponent();
            _imgEncoder = new MMALImageEncoder(continuousCapture: true);
            _nullSink = new MMALNullSinkComponent();


           
        }

        public void Close()
        {
            IsOpen = false;
            _nullSink?.Dispose();
            _imgEncoder?.Dispose();
            _splitter?.Dispose();
            _imgCaptureHandler?.Dispose();
        }
        
        
        #region NotImplemented
        public IImage GrabSingle()
        {
            throw new NotImplementedException();
        }
        public void LoadParameters(string parametersFile)
        {
            throw new NotImplementedException();
        }
        public void SaveParameters(string parametersFile)
        {
            throw new NotImplementedException();
        }

        public void SaveParametersToDevice()
        {
            throw new NotImplementedException();
        }
        #endregion

        public void StartGrabContinuous()
        {
            if (!IsOpen) throw new Exception("Camera is not open");
            if (IsGrabbingContinuous) return;
            IsGrabbingContinuous = true;
            MMALCamera cam = MMALCamera.Instance;
            cam.ConfigureCameraSettings();

            //var portConfig = new MMALPortConfig(MMALEncoding.JPEG, MMALEncoding.I420, _width, _height, 30, 90, 0, false, null);
            var portConfig = new MMALPortConfig(MMALEncoding.JPEG, MMALEncoding.I420, 90);

            // Create our component pipeline.         
            _imgEncoder.ConfigureOutputPort(portConfig,_imgCaptureHandler);

            cam.Camera.VideoPort.ConnectTo(_splitter);
            _splitter.Outputs[0].ConnectTo(_imgEncoder);
            cam.Camera.PreviewPort.ConnectTo(_nullSink);

            _cancellationSource = new CancellationTokenSource();

            cam.ProcessAsync(cam.Camera.VideoPort, _cancellationSource.Token);

            Task.Run(async () =>
            {
                
                while (IsGrabbingContinuous)
                {
                    
                    if (!_imgCaptureHandler.ImageQueue.TryDequeue(out byte[] imageData))
                    {
                        await Task.Delay(10);
                    }
                    else
                    {
                        
                        ImageGrabbed(this, new ByteImage(_width, _height, 3,imageData,_width*3));
                    }
                    
                }
            });

        }

        public void StopGrabContinuous()
        {
            if (!IsGrabbingContinuous) return;
            IsGrabbingContinuous = false;
            _cancellationSource.Cancel();
        }
    }
}

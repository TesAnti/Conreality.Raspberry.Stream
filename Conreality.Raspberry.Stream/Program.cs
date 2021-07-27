using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Inspectron.CovidTest.Raspberry.Image;
using rtaNetworking.Streaming;


namespace Inspectron.CovidTest.Raspberry
{

    class HawkeyeRaspberry
    {
        public HawkeyeRaspberry()
        {
            _camera = new RaspberryCamera(null);
            
        }

        public void Init()
        {
            _camera.Open();
            
            _camera.ImageGrabbed += Camera_ImageGrabbed;
            _camera.StartGrabContinuous();
        }
        public ConcurrentQueue<byte[]> _images = new ConcurrentQueue<byte[]>();
        private RaspberryCamera _camera;

        private void Camera_ImageGrabbed(ICamera sender, IImage image)
        {
            if (_images.Count < 5)
            {
                _images.Enqueue((image as ByteImage).Data);
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
    static class SocketExtensions
    {

        public static IEnumerable<Socket> IncommingConnectoins(this Socket server)
        {
            while(true)
                yield return server.Accept();
        }

    }
    class Program
    {
        static HawkeyeRaspberry camera;
        static void Main(string[] args)
        {

            camera = new HawkeyeRaspberry();
            camera.Init();

            Socket Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            Server.Bind(new IPEndPoint(IPAddress.Any,(int)8080));
            Server.Listen(10);

            System.Diagnostics.Debug.WriteLine(string.Format("Server started on port {0}.", 8080));
                
            foreach (Socket client in Server.IncommingConnectoins())
                ThreadPool.QueueUserWorkItem(new WaitCallback(ClientThread), client);
            

            Console.ReadLine();
        }

        private static void ClientThread(object client)
        {
            Socket socket = (Socket)client;
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                using (MjpegWriter wr = new MjpegWriter(new NetworkStream(socket, true)))
                {

                    // Writes the response header to the client.
                    wr.WriteHeader();


                    while (true)
                    {
                        var imgStream = new MemoryStream(camera.GetImage());
                        
                        wr.Write(imgStream);
                        Console.WriteLine(sw.ElapsedMilliseconds);
                        sw.Restart();
                    }


                }
            }
            catch
            {

            }
        }
    }
}

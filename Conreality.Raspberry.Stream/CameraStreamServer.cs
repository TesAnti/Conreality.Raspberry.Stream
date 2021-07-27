using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Conreality.Raspberry.Stream
{
    public class CameraStreamServer
    {
        private Socket _server;
        private HawkeyeRaspberry _camera;

        public CameraStreamServer()
        {
            _camera = new HawkeyeRaspberry();
            _camera.Init();

            _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start(int port)
        {
            _server.Bind(new IPEndPoint(IPAddress.Any,port));
            _server.Listen(100);

            
                
            foreach (Socket client in _server.IncommingConnectoins())
                ThreadPool.QueueUserWorkItem(new WaitCallback(ClientThread), client);
        }

        private void ClientThread(object client)
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
                        var imgStream = new MemoryStream(_camera.GetImage());
                        
                        wr.Write(imgStream);
                        
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
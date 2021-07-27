using System;

namespace Conreality.Raspberry.Stream.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            Conreality.Raspberry.Stream.CameraStreamServer server = new();
            server.Start(8080);
        }
    }
}

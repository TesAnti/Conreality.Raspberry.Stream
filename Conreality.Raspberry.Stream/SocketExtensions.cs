﻿using System.Collections.Generic;
using System.Net.Sockets;

namespace Conreality.Raspberry.Stream
{
    static class SocketExtensions
    {

        public static IEnumerable<Socket> IncommingConnectoins(this Socket server)
        {
            while(true)
                yield return server.Accept();
        }

    }
}
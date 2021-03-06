using System.Collections.Concurrent;
using MMALSharp.Common;
using MMALSharp.Handlers;

namespace Conreality.Raspberry.Stream
{
    
    internal class VideoHandler : InMemoryCaptureHandler
    {
        public ConcurrentQueue<byte[]> ImageQueue = new ConcurrentQueue<byte[]>();

        public override void Process(ImageContext context)
        {
            base.Process(context);
            if (context.Eos)
            {
                var c = WorkingData.ToArray();
            
                ImageQueue.Enqueue(c);
                WorkingData.Clear();
            }
            
        }

        
    }
}
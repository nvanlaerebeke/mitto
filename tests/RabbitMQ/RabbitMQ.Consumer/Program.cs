using System.Collections.Generic;
using System.Threading;
using Mitto;

namespace RabbitMQ.Consumer {

    internal class Program {
        private static ManualResetEvent _quit = new ManualResetEvent(false);

        private static void Main(string[] args) {
            RabbitMQ.Start();
            _quit.WaitOne();
        }
    }
}
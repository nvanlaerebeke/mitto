using System.Threading;

namespace RabbitMQ.Publisher {

    internal class Program {
        private static ManualResetEvent _quit = new ManualResetEvent(false);

        private static void Main(string[] args) {
            RabbitMQ.Start();
            _quit.WaitOne();
        }
    }
}
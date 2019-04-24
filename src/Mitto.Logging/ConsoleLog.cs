using System;

namespace Mitto.Logging {

    public class ConsoleLog : ILog {

        public ConsoleLog() {
        }

        public void Debug(object message) {
            Console.WriteLine(message);
        }

        public void Debug(object message, Exception exception) {
            Console.WriteLine(message);
            Console.WriteLine(exception);
        }

        public void Error(object message) {
            Console.WriteLine(message);
        }

        public void Error(object message, Exception exception) {
            Console.WriteLine(message);
            Console.WriteLine(exception);
        }

        public void Fatal(object message) {
            Console.WriteLine(message);
        }

        public void Fatal(object message, Exception exception) {
            Console.WriteLine(message);
            Console.WriteLine(exception);
        }

        public void Info(object message) {
            Console.WriteLine(message);
        }

        public void Info(object message, Exception exception) {
            Console.WriteLine(message);
            Console.WriteLine(exception);
        }

        public void Warn(object message) {
            Console.WriteLine(message);
        }

        public void Warn(object message, Exception exception) {
            Console.WriteLine(message);
            Console.WriteLine(exception);
        }

        public void Trace(object message) {
            Console.WriteLine(message);
        }

        public void Trace(object message, Exception exception) {
            Console.WriteLine(message);
            Console.WriteLine(exception);
        }
    }
}
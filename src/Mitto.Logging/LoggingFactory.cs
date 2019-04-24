using System;

namespace Mitto.Logging {

    public class LoggingFactory {
        private static ILog Logger { get; set; }

        public static void Initialize(ILog pLogger) {
            Logger = pLogger;
        }

        public static ILog GetLogger() {
            return Logger;
        }

        public static ILog GetLogger(Type pType) {
            return Logger;
        }
    }
}
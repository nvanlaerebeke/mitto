using System;

namespace Mitto.ILogging {
	public class LogFactory {
		private static ILogProvider Provider { get; set; }
		public static void Initialize(ILogProvider pProvider) {
			Provider = pProvider;
		}
		public static ILog GetLogger(Type pType) {
			return Provider.GetLogger(pType);
		}

		public static ILog GetLogger() {
			return Provider.GetLogger();
		}
	}
}

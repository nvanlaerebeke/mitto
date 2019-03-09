using Mitto.ILogging;

namespace Mitto.Logging {
	public class Log4NetParams {
		public Log4NetParams() { }

		public LogLevel LogLevel { get; set; } = LogLevel.Debug;
		public string LogFilePath { get; set; } = "";
	}
}

using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using Mitto.ILogging;
using System;

namespace Mitto.Logging {
	public class LogProvider : ILogProvider {
		public LogProvider() : this(new Log4NetParams()){ }

		public LogProvider(Log4NetParams pParams) {
			//Configure log4net
			var patternLayout = new PatternLayout() {
				ConversionPattern = "%date [%thread] %-5level %logger{1} - %message%newline"
			};
			patternLayout.ActivateOptions();

			var hierarchy = (Hierarchy)LogManager.GetRepository();
			switch (pParams.LogLevel) {
				case LogLevel.Debug:
					hierarchy.Root.Level = Level.Debug;
					break;
				case LogLevel.Error:
					hierarchy.Root.Level = Level.Error;
					break;
				case LogLevel.Info:
					hierarchy.Root.Level = Level.Info;
					break;
				case LogLevel.Off:
					hierarchy.Root.Level = Level.Off;
					break;
				case LogLevel.Warn:
					hierarchy.Root.Level = Level.Warn;
					break;
				case LogLevel.Trace:
					hierarchy.Root.Level = Level.Trace;
					break;
			}

			if (!String.IsNullOrEmpty(pParams.LogFilePath)) {
				var roller = new RollingFileAppender {
					AppendToFile = true,
					File = pParams.LogFilePath,
					Layout = patternLayout,
					ImmediateFlush = true,
					MaxSizeRollBackups = 5,
					MaximumFileSize = "10MB",
					RollingStyle = RollingFileAppender.RollingMode.Size,
					StaticLogFileName = true
				};
				roller.ActivateOptions();
				hierarchy.Root.AddAppender(roller);
			} else {
				var consoleappender = new ConsoleAppender {
					Layout = patternLayout
				};
				consoleappender.ActivateOptions();
				hierarchy.Root.AddAppender(consoleappender);
			}
			hierarchy.Configured = true;
		}

		public ILogging.ILog GetLogger(Type pType) {
			return new Log(LogManager.GetLogger(pType));
		}
		public ILogging.ILog GetLogger() {
			return new Log(LogManager.GetLogger("Main"));
		}
	}
}
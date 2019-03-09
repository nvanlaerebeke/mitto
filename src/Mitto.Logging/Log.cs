﻿using System;
using log4net;

namespace Mitto.Logging {
	class Log : ILogging.ILog {
		private ILog Logger { get; set; }
		public Log(ILog pLogger) {
			Logger = pLogger;
		}

		public void Debug(object message) {
			Logger.Debug(message);
		}

		public void Debug(object message, Exception exception) {
			Logger.Debug(message, exception);
		}

		public void Error(object message) {
			Logger.Error(message);
		}

		public void Error(object message, Exception exception) {
			Logger.Error(message, exception);
		}

		public void Fatal(object message) {
			Logger.Fatal(message);
		}

		public void Fatal(object message, Exception exception) {
			Logger.Fatal(message, exception);
		}

		public void Info(object message) {
			Logger.Info(message);
		}

		public void Info(object message, Exception exception) {
			Logger.Info(message, exception);
		}

		public void Warn(object message) {
			Logger.Warn(message);
		}

		public void Warn(object message, Exception exception) {
			Logger.Warn(message, exception);
		}

		public void Trace(object message) {
			Logger.Debug(message);
		}

		public void Trace(object message, Exception exception) {
			Logger.Debug(message, exception);
		}
	}
}
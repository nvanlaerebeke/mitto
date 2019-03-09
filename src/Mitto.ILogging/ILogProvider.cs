using System;

namespace Mitto.ILogging {
	public interface ILogProvider {
		ILog GetLogger(Type pType);
		ILog GetLogger();
	}
}
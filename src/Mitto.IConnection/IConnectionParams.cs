using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mitto.Main")]
namespace Mitto.IConnection {
	public interface IClientParams { }
	public interface IServerParams {
		/// <summary>
		/// Action to execute when a client connects
		/// </summary>
		Action<IClientConnection> ClientConnected { get; }
	}

	public abstract class ServerParams : IServerParams {
		/// <summary>
		/// Action to execute when a client connects
		/// </summary>
		public Action<IClientConnection> ClientConnected { get; internal set; }
	}
}
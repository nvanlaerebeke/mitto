using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mitto.Main")]

namespace Mitto.IConnection {

    public interface IClientParams {
        string HostName { get; }
        int Port { get; }
        bool Secure { get; }
    }

    public interface IServerParams { }
}
﻿using System;
using Mitto.IConnection;

namespace Mitto {

    public class Server {
        private readonly IServer _objServer;

        public Server() {
            _objServer = ConnectionFactory.CreateServer();
        }

        /// <summary>
        /// Starts the Server connection
        /// </summary>
        /// <param name="pParams">Parameters for the server</param>
        /// <param name="pAction">Action that will be run when a client connects</param>
        public void Start(IServerParams pParams, Action<ClientConnection> pAction) {
            _objServer.Start(pParams, c => pAction.Invoke(new ClientConnection(c)));
        }

        public void Stop() {
            _objServer.Stop();
        }
    }
}
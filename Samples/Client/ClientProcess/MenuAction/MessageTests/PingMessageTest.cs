﻿using ClientProcess;
using ClientManager;
using System;
using Messaging.Base.Response;
using IConnection;

namespace ConnectionClient.MenuAction.MessageTests {
	public class PingMesasgeTest : ITest {
		private Client _objClient;

		public void Test() {
			if (_objClient != null) {
				Close();
			}
			_objClient = new Client();
			_objClient.Connected += ObjClient_Connected;
			_objClient.Disconnected += ObjClient_Disconnected;

			_objClient.ConnectAsync(Controller.Host, Controller.Port, Controller.Secure);
		}

		private void ObjClient_Connected(Client pClient) {
			Console.WriteLine("Client Connected, sending some data");

			_objClient.Request<Pong>(new Messaging.Base.Request.Ping(), (Pong pResponse) => {
				Console.WriteLine("Pong Received");
			});
		}

		private void ObjClient_Disconnected(Client pClient) {
			Console.WriteLine("Client Disconnected");
		}

		internal void Close() {
			_objClient.Close();
			_objClient = null;
		}
	}
}
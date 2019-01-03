﻿using IConnection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClientUI.MenuAction.ConnectionTests {
	class MultiConnectionTest : ITest {
		private List<IConnection.IConnection> _lstClient = new List<IConnection.IConnection>();

		public void Test() {
			Parallel.For(1, 5000, new ParallelOptions { MaxDegreeOfParallelism = 10 }, (i) => {
				var objClient = ConnectionFactory.GetClient();
				objClient.Connected += ObjClient_Connected;
				objClient.Disconnected += ObjClient_Disconnected;

				lock (_lstClient) {
					_lstClient.Add(objClient);
				}

				objClient.ConnectAsync("localhost", 80, false);
			});
		}

		private void ObjClient_Connected(IConnection.IConnection pClient) {
			lock (_lstClient) {
				_lstClient.Add(pClient);
				Console.WriteLine("Client " + _lstClient.Count + "Connected");
			}

		}

		private void ObjClient_Disconnected(IConnection.IConnection pClient) {
			lock (_lstClient) {
				_lstClient.Remove(pClient);
				Console.WriteLine("Client " + _lstClient.Count + "Connected");
			}
		}

		internal void Close() {
			lock (_lstClient) {
				_lstClient.ForEach(c => c.Close());
				_lstClient.Clear();
			}
		}
	}
}
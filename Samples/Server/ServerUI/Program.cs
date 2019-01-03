using System;
using System.Net;
using System.Threading;
using Connection.Websocket.Server;
using IConnection;
using IMessaging;
using IQueue;
using log4net.Appender;
using log4net.Config;
using Queue.PassThrough;
using ServerManager;
using Unity;

namespace ConnectionServer {
	class Program {
		static ManualResetEvent _objClose = new ManualResetEvent(false);
		static Server _objServer;
		static void Main(string[] args) {
			ConnectionFactory.UnityContainer.RegisterType<IServer, WebsocketServer>();
			QueueFactory.UnityContainer.RegisterType<IQueue.IQueue, PassThrough>();
			MessagingFactory.UnityContainer.RegisterType<IMessageCreator, Messaging.Json.MessageCreator>();
			MessagingFactory.UnityContainer.RegisterType<IMessageProvider, Messaging.App.Server.ServerMessageProvider>();

			ConfigureLogger();

			_objServer = new Server();
			_objServer.Start(IPAddress.Any, 80);

			Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e) {
				_objClose.Set();
			};
			_objClose.WaitOne();
		}

		static void ConfigureLogger() {
			BasicConfigurator.Configure(new ConsoleAppender() {
				Threshold = log4net.Core.Level.Info,
				Name = "ConsoleAppender",
				Layout = new log4net.Layout.SimpleLayout()
			});
		}
	}
}
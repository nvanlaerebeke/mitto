using Connection.Websocket.Server;
using IConnection;
using IQueue;
using log4net.Appender;
using log4net.Config;
using Queue.RabbitMQ.Publisher;
using System;
using System.Net;
using System.Threading;
using Unity;

namespace Server.Publisher {
	class Program {
		static ManualResetEvent _objClose = new ManualResetEvent(false);
		static ServerManager.Server _objServer;

		static void Main(string[] args) {
			ConnectionFactory.UnityContainer.RegisterType<IServer, WebsocketServer>();
			QueueFactory.UnityContainer.RegisterType<IQueue.IQueue, RabbitMQ>();

			RabbitMQ.SetConfig(new Queue.RabbitMQ.Config() {
				//Host = "localhost",
				Host = "test.crazyzone.be",
				MainQueue = "MittoMain"
			});

			ConfigureLogger();

			_objServer = new ServerManager.Server();
			_objServer.Start(IPAddress.Any, 80);

			Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e) {
				_objClose.Set();
			};
			_objClose.WaitOne();
		}

		static void ConfigureLogger() {
			BasicConfigurator.Configure(new ConsoleAppender() {
				Threshold = log4net.Core.Level.Error,
				Name = "ConsoleAppender",
				Layout = new log4net.Layout.SimpleLayout()
			});
		}
	}
}

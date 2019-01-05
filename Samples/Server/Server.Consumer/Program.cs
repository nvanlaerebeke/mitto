using System;
using System.Net;
using System.Threading;
using IMessaging;
using IQueue;
using log4net.Appender;
using log4net.Config;
using Unity;
using Queue.RabbitMQ.Consumer;

namespace Server.Consumer {
	class Program {
		static ManualResetEvent _objClose = new ManualResetEvent(false);
		static void Main(string[] args) {
			QueueFactory.UnityContainer.RegisterType<IQueue.IQueue, RabbitMQ>();
			MessagingFactory.UnityContainer.RegisterType<IMessageCreator, Messaging.Json.MessageCreator>();
			MessagingFactory.UnityContainer.RegisterType<IMessageProvider, Messaging.App.Server.ServerMessageProvider>();

			ConfigureLogger();

			RabbitMQ.SetConfig(new Queue.RabbitMQ.Config() {
				Host = "localhost",
				MainQueue = "MittoMain"
			});

			RabbitMQReader.Start();

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

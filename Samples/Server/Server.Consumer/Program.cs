using System;
using System.Threading;
using log4net.Appender;
using log4net.Config;
using Unity;
using Queue.RabbitMQ.Consumer;

namespace Server.Consumer {
	class Program {
		static ManualResetEvent _objClose = new ManualResetEvent(false);
		static void Main(string[] args) {
			Mitto.IQueue.QueueFactory.UnityContainer.RegisterType<Mitto.IQueue.IQueue, RabbitMQ>();
			Mitto.IMessaging.MessagingFactory.UnityContainer.RegisterType<Mitto.IMessaging.IMessageCreator, Mitto.Messaging.Json.MessageCreator>();
			Mitto.IMessaging.MessagingFactory.UnityContainer.RegisterType<Mitto.IMessaging.IMessageProvider, Messaging.App.Server.ServerMessageProvider>();

			ConfigureLogger();

			RabbitMQ.SetConfig(new Queue.RabbitMQ.Config() {
				//Host = "localhost",
				Host = "test.crazyzone.be",
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

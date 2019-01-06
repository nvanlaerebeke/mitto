using ClientProcess;
using log4net.Appender;
using log4net.Config;
using System.Threading;
using Unity;

namespace ClientUI {
	/// <summary>
	/// Sample application that makes a connection and can send messages between client/server
	/// 
	/// For internal communication this sample uses the passthrough method
	///   This means the process itself will run the actions and not a separate process as 
	///   when using an AMQP(example RabbitMQ)
	///   
	/// For a client/server connection websockets are used
	/// 
	/// The messaging over the connection is done by using Json (Class -> Json || byte[] || -> Json -> Class)
	/// A message povider must also be set when using the messaging, this extends the base funionallity with 
	/// your own messages and actions(handlers)
	/// </summary>
	class Program {
		static ManualResetEvent _quit = new ManualResetEvent(false);
		static void Main(string[] args) {
			ConfigureLogger();

			IQueue.QueueFactory.UnityContainer.RegisterType<IQueue.IQueue, Queue.PassThrough.PassThrough>();
			IConnection.ConnectionFactory.UnityContainer.RegisterType<IConnection.IClient, Connection.Websocket.Client.WebsocketClient>();
			IMessaging.MessagingFactory.UnityContainer.RegisterType<IMessaging.IMessageCreator, Messaging.Json.MessageCreator>();
			IMessaging.MessagingFactory.UnityContainer.RegisterType<IMessaging.IMessageProvider, Messaging.App.Client.ClientMessageProvider>();

			Controller.Start("localhost", 80, false);
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
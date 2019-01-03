using ClientProcess;
using Connection.Websocket.Client;
using Connection.Websocket.Server;
using IConnection;
using IMessaging;
using IQueue;
using log4net.Appender;
using log4net.Config;
using Messaging.App.Client;
using Messaging.Json;
using Queue.PassThrough;
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
			QueueFactory.UnityContainer.RegisterType<IQueue.IQueue, PassThrough>();
			ConnectionFactory.UnityContainer.RegisterType<IClient, WebsocketClient>();
			ConnectionFactory.UnityContainer.RegisterType<IServer, WebsocketServer>();
			MessagingFactory.UnityContainer.RegisterType<IMessageCreator, MessageCreator>();
			MessagingFactory.UnityContainer.RegisterType<IMessageProvider, ClientMessageProvider>();

			ConfigureLogger();

			Controller.Start();
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
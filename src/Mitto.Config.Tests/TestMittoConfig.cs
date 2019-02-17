using NUnit.Framework;
using NSubstitute;
using System.Linq;
using Mitto.IQueue;
using Mitto.IConnection;
using Mitto.IMessaging;

namespace Mitto.Config.Tests {
	[TestFixture]
	public class ConfigTests {
		/// <summary>
		/// Tests setting the QueueProvider
		/// This means that when the Create method is called on the QueueFactory
		/// the methods will be call on the mocked provider that was passed in 
		/// the configuration
		/// 
		/// Said create method should also return an IQueue object
		/// </summary>
		[Test]
		public void ConfigQueueTest() {
			var objProvider = Substitute.For<IQueueProvider>();
			Mitto.Initialize(new Mitto.Config() {
				QueueProvider = objProvider
			});

			var objQueue = QueueFactory.Create();

			Assert.IsInstanceOf<IQueue.IQueue>(objQueue);
			objProvider.Received(1).Create();
		}

		/// <summary>
		/// Tests setting the MessageCreator
		/// This means that when the IMessageCreator methods are called using the ConnectionFactory.Creator 
		/// the same methods will be called on the mocked IMessageCreator that was configured
		/// </summary>
		[Test]
		public void ConfigMessageCreatorTest() {
			var objProvider = Substitute.For<IMessageCreator>();
			Mitto.Initialize(new Mitto.Config() {
				MessageCreator = objProvider
			});

			var objMessage = MessagingFactory.Creator.Create(new byte[] { 1, 2, 3, 4 });
			Assert.IsInstanceOf<IMessage>(objMessage);

			var arrBytes = MessagingFactory.Creator.GetBytes(objMessage);
			Assert.IsInstanceOf<byte[]>(arrBytes);

			var objResponse = MessagingFactory.Creator.GetResponseMessage(objMessage, ResponseCode.Success);
			Assert.IsInstanceOf<IMessage>(objResponse);

			objProvider.Received(1).Create(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 })));
			objProvider.Received(1).GetBytes(Arg.Any<IMessage>());
			objProvider.Received(1).GetResponseMessage(Arg.Any<IMessage>(), ResponseCode.Success);
		}

		/// <summary>
		/// Tests setting the MessageProvider
		/// This means that when the methods are called on the MessagingFactory.Provider 
		/// said methods will be called on the mocked provider that was passed in the configuration
		/// </summary>
		[Test]
		public void ConfigMessageProviderTest() {
			var objProvider = Substitute.For<IMessageProvider>();
			Mitto.Initialize(new Mitto.Config() {
				MessageProvider = objProvider
			});

			MessagingFactory.Provider.GetTypes();
			MessagingFactory.Provider.GetResponseType("MyMessageType");
			MessagingFactory.Provider.GetType(MessageType.Request, 1);
			MessagingFactory.Provider.GetActionType(Substitute.For<IMessage>());

			objProvider.Received(1).GetTypes();
			objProvider.Received(1).GetResponseType(Arg.Any<string>());
			objProvider.Received(1).GetType(MessageType.Request, 1);
			objProvider.Received(1).GetActionType(Arg.Any<IMessage>());
		}

		/// <summary>
		/// Tests setting the MessageProcessor
		/// This means that when the Process method is called on the MessagingFactory.Processor 
		/// said method will be called on the mocked processor that was passed in the configuration
		/// </summary>
		[Test]
		public void ConfigMessageProcessorTest() {
			var objProvider = Substitute.For<IMessageProcessor>();
			Mitto.Initialize(new Mitto.Config() {
				MessageProcessor = objProvider
			});

			MessagingFactory.Processor.Process(Substitute.For<IQueue.IQueue>(), new Message("MyClientID", new byte[] { 1,2,3,4 }));
			
			objProvider.Received(1).Process(Arg.Any<IQueue.IQueue>(), Arg.Any<IQueue.Message>());
		}

		/// <summary>
		/// Tests setting the ConnectionProvider
		/// This means that when the CreateClient/Server methods are called on the ConnectionFactory
		/// the same methods will be called on the mocked provider that was passed in the configuration
		/// 
		/// Said methods should also return an IServer/IClient object respectively
		/// </summary>
		[Test]
		public void ConfigConnectionTest() {
			var objProvider = Substitute.For<IConnectionProvider>();
			Mitto.Initialize(new Mitto.Config() {
				ConnectionProvider = objProvider
			});

			var objClient = ConnectionFactory.CreateClient();
			var objServer = ConnectionFactory.CreateServer();

			Assert.IsInstanceOf<IClient>(objClient);
			Assert.IsInstanceOf<IServer>(objServer);

			objProvider.Received(1).CreateClient();
			objProvider.Received(1).CreateServer();
		}
	}
}
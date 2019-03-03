using NUnit.Framework;
using NSubstitute;
using System.Linq;
using Mitto.IQueue;
using Mitto.IConnection;
using Mitto.IMessaging;

namespace Mitto.Tests {
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
			//Arrange
			var objProvider = Substitute.For<IQueueProvider>();
			Config.Initialize(new Config.ConfigParams() {
				QueueProvider = objProvider
			});
			
			//Act
			var objQueue = QueueFactory.Create();

			//Assert
			Assert.IsInstanceOf<IQueue.IQueue>(objQueue);
			objProvider.Received(1).Create();
		}

		/// <summary>
		/// Tests setting the MessageConverter
		/// This means that when the IMessageConverter methods are called using the ConnectionFactory.Creator 
		/// the same methods will be called on the mocked IMessageConverter that was configured
		/// </summary>
		[Test]
		public void ConfigMessageCreatorTest() {
			//Arrange
			var objProvider = Substitute.For<IMessageConverter>();
			Config.Initialize(new Config.ConfigParams() {
				MessageConverter = objProvider
			});

			//Act
			var objMessage = MessagingFactory.Converter.GetMessage(typeof(Message), new byte[] { 1, 2, 3, 4 });
			var arrBytes = MessagingFactory.Converter.GetByteArray(objMessage);

			//Assert
			Assert.IsInstanceOf<IMessage>(objMessage);
			Assert.IsInstanceOf<byte[]>(arrBytes);
		}

		/// <summary>
		/// Tests setting the MessageProvider
		/// This means that when the methods are called on the MessagingFactory.Provider 
		/// said methods will be called on the mocked provider that was passed in the configuration
		/// </summary>
		[Test]
		public void ConfigMessageProviderTest() {
			//Arrange
			var objProvider = Substitute.For<IMessageProvider>();
			var objMessage = Substitute.For<IRequestMessage>();
			var objClient = Substitute.For<IMessaging.IClient>();
			objClient.ID.Returns("MyClientID");
			objMessage.ID.Returns("MyMessageID");

			Config.Initialize(new Config.ConfigParams() {
				MessageProvider = objProvider
			});
			

			//Act
			MessagingFactory.Provider.GetMessage(new byte[] { 1, 2, 3, 4 });
			MessagingFactory.Provider.GetResponseMessage(objMessage, ResponseCode.Success);
			MessagingFactory.Provider.GetAction(objClient, objMessage);

			//Assert
			objProvider.Received(1).GetMessage(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1,2,3,4})));
			objProvider.Received(1).GetResponseMessage(Arg.Is<IRequestMessage>(m => m.Equals(objMessage)), Arg.Is(ResponseCode.Success));
			objProvider.Received(1).GetAction(Arg.Is(objClient), Arg.Is<IRequestMessage>(m => m.Equals(objMessage)));
		}

		/// <summary>
		/// Tests setting the MessageProcessor
		/// This means that when the Process method is called on the MessagingFactory.Processor 
		/// said method will be called on the mocked processor that was passed in the configuration
		/// </summary>
		[Test]
		public void ConfigMessageProcessorTest() {
			//Arrange
			var objProvider = Substitute.For<IMessageProcessor>();
			Mitto.Config.Initialize(new Config.ConfigParams() {
				MessageProcessor = objProvider
			});
			//Act
			MessagingFactory.Processor.Process(Substitute.For<IQueue.IQueue>(), new byte[] { 1, 2, 3, 4 });

			//Assert
			objProvider.Received(1).Process(Arg.Any<IQueue.IQueue>(), Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 })));
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
			//Arrange
			var objProvider = Substitute.For<IConnectionProvider>();
			Config.Initialize(new Config.ConfigParams() {
				ConnectionProvider = objProvider
			});
			
			//Act
			var objClient = ConnectionFactory.CreateClient();
			var objServer = ConnectionFactory.CreateServer();

			//Assert
			Assert.IsInstanceOf<IConnection.IClient>(objClient);
			Assert.IsInstanceOf<IConnection.IServer>(objServer);
			objProvider.Received(1).CreateClient();
			objProvider.Received(1).CreateServer();
		}
	}
}
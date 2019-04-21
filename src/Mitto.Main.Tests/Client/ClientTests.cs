using Mitto.IConnection;
using Mitto.IRouting;
using NSubstitute;
using NUnit.Framework;
using System;

namespace Mitto.Main.Tests.Client {

    [TestFixture]
    public class ClientTests {

        /// <summary>
        /// Tests the IConnection.IClient connect event
        /// This means that the Connected event is raised
        /// </summary>
        [Test]
        public void ClientConnectedTest() {
            //Arrange
            var objProvider = Substitute.For<IConnectionProvider>();
            var objConnection = Substitute.For<IClient>();
            var objHandler = Substitute.For<EventHandler<Mitto.Client>>();
            var objConnectionParams = Substitute.For<IClientParams>();

            objProvider.CreateClient().Returns(objConnection);
            Config.Initialize(new Config.ConfigParams() {
                ConnectionProvider = objProvider
            });

            //Act
            var obj = new Mitto.Client();
            obj.ConnectAsync(objConnectionParams);
            obj.Connected += objHandler;
            objConnection.Connected += Raise.Event<EventHandler<IClient>>(new object(), objConnection);

            //Assert
            objHandler
                .Received(1)
                .Invoke(
                    Arg.Any<object>(),
                    Arg.Is(obj)
                )
            ;
        }

        /// <summary>
        /// Tests the handling of the IConnection.IClient disconnect event
        ///
        /// This means that the Connection subscription handlers are removed,
        /// the router is cleaned up and that the Disconnected event is called
        /// </summary>
        [Test]
        public void ClientDisconnectedTest() {
            //Arrange
            var objConnectionProvider = Substitute.For<IConnectionProvider>();
            var objRouterProvider = Substitute.For<IRouterProvider>();

            var objConnection = Substitute.For<IClient>();
            var objRouter = Substitute.For<IRouter>();
            var objHandler = Substitute.For<EventHandler<Mitto.Client>>();
            var objConnectionParams = Substitute.For<IClientParams>();

            objConnectionProvider.CreateClient().Returns(objConnection);
            objRouterProvider.Create(Arg.Is(objConnection)).Returns(objRouter);

            Config.Initialize(new Config.ConfigParams() {
                ConnectionProvider = objConnectionProvider,
                RouterProvider = objRouterProvider
            });

            //Act
            var obj = new Mitto.Client();
            obj.ConnectAsync(objConnectionParams);
            obj.Disconnected += objHandler;
            objConnection.Connected += Raise.Event<EventHandler<IClient>>(new object(), objConnection);
            System.Threading.Thread.Sleep(50);
            objConnection.Disconnected += Raise.Event<EventHandler<IConnection.IConnection>>(objConnection, objConnection);

            //Assert
            objConnection.Received(1).Connected -= Arg.Any<EventHandler<IClient>>();
            objConnection.Received(1).Disconnected -= Arg.Any<EventHandler<IConnection.IConnection>>();
            objRouter.Received(1).Close();

            objHandler
                .Received(1)
                .Invoke(Arg.Is(obj), Arg.Is(obj))
            ;
        }

        /// <summary>
        /// Tests the ConnectAsync method
        /// This means that the connect <see langword="async"/> method is called on the
        /// the Connection returned from the ConnectionProvider
        /// </summary>
        [Test]
        public void ConnectAsyncTest() {
            //Arrange
            var objProvider = Substitute.For<IConnectionProvider>();
            var objConnection = Substitute.For<IClient>();
            var objParams = Substitute.For<IClientParams>();

            objProvider.CreateClient().Returns(objConnection);

            Config.Initialize(new Config.ConfigParams() {
                ConnectionProvider = objProvider
            });

            //Act
            var obj = new Mitto.Client();
            obj.ConnectAsync(objParams);

            //Assert
            objConnection.Received(1).ConnectAsync(Arg.Is(objParams));
        }

        /// <summary>
        /// Tests the Client creation
        /// This means that the IConnection is set as well as the IQueue
        /// The event handlers for connected, disconnected, data received are also attached
        /// </summary>
        [Test]
        public void CreateTest() {
            //Arrange
            var objConnectionProvider = Substitute.For<IConnectionProvider>();
            var objRouterProvider = Substitute.For<IRouterProvider>();

            var objConnection = Substitute.For<IClient>();
            var objRouter = Substitute.For<IRouter>();
            var objConnectionParams = Substitute.For<IClientParams>();

            objConnectionProvider.CreateClient().Returns(objConnection);
            objRouterProvider.Create(objConnection).Returns(objRouter);

            Config.Initialize(new Config.ConfigParams() {
                ConnectionProvider = objConnectionProvider,
                RouterProvider = objRouterProvider
            });

            //Act
            var obj = new Mitto.Client();
            obj.ConnectAsync(objConnectionParams);
            objConnection.Connected += Raise.Event<EventHandler<IClient>>(new object(), objConnection);

            //Assert
            objConnectionProvider.Received(1).CreateClient();
            objRouterProvider.Received(1).Create(objConnection);

            objConnection.Received(1).Connected += Arg.Any<EventHandler<IClient>>();
            objConnection.Received(1).Disconnected += Arg.Any<EventHandler<IConnection.IConnection>>();
        }

        /// <summary>
        /// Test the Disconnect method
        /// This means that the close is called and the Disconnect on the IConnection.IClient
        /// Also verifies that the disconnect event is not fired
        /// </summary>
        [Test]
        public void DisconnectTest() {
            //Arrange
            var objConnectionProvider = Substitute.For<IConnectionProvider>();
            var objRouterProvider = Substitute.For<IRouterProvider>();

            var objConnection = Substitute.For<IClient>();
            var objHandler = Substitute.For<EventHandler<Mitto.Client>>();
            var objRouter = Substitute.For<IRouter>();
            var objConnectionParams = Substitute.For<IClientParams>();

            objConnectionProvider.CreateClient().Returns(objConnection);
            objRouterProvider.Create(objConnection).Returns(objRouter);

            Config.Initialize(new Config.ConfigParams() {
                ConnectionProvider = objConnectionProvider,
                RouterProvider = objRouterProvider
            });

            //Act
            var obj = new Mitto.Client();
            obj.ConnectAsync(objConnectionParams);
            obj.Disconnected += objHandler;

            objConnection.Connected += Raise.Event<EventHandler<IClient>>(new object(), objConnection);
            System.Threading.Thread.Sleep(50);

            obj.Disconnect();

            //Assert
            objConnection.Received(1).Connected -= Arg.Any<EventHandler<IClient>>();
            objConnection.Received(1).Disconnected -= Arg.Any<EventHandler<IConnection.IConnection>>();
            objConnection.Received(1).Disconnect();
            objRouter.Received(1).Close();
            objHandler
                .Received(1)
                .Invoke(
                    Arg.Is(obj),
                    Arg.Is(obj)
                )
            ;
        }

        /// <summary>
        /// Tests the Request method
        /// This means that the Request is passed onto the MessageProcesser
        /// </summary>
        [Test]
        public void RequestTest() {
            //Arrange
            var objProvider = Substitute.For<IConnectionProvider>();
            var objRouterProvider = Substitute.For<IRouterProvider>();
            var objProcessor = Substitute.For<IMessaging.IMessageProcessor>();

            var objConnection = Substitute.For<IClient>();
            var objRouter = Substitute.For<IRouter>();
            var objRequestMessage = Substitute.For<IMessaging.IRequestMessage>();
            var objAction = Substitute.For<Action<IMessaging.IResponseMessage>>();
            var objConnectionParams = Substitute.For<IClientParams>();

            objProvider.CreateClient().Returns(objConnection);
            objRouterProvider.Create(objConnection).Returns(objRouter);

            Config.Initialize(new Config.ConfigParams() {
                ConnectionProvider = objProvider,
                RouterProvider = objRouterProvider,
                MessageProcessor = objProcessor
            });

            //Act
            var obj = new Mitto.Client();
            obj.ConnectAsync(objConnectionParams);
            objConnection.Connected += Raise.Event<EventHandler<IClient>>(new object(), objConnection);
            System.Threading.Thread.Sleep(50);
            obj.Request(objRequestMessage, objAction);

            //Assert
            objProcessor.Received(1).Request(Arg.Is(objRouter), Arg.Is(objRequestMessage), Arg.Is(objAction));
        }
    }
}
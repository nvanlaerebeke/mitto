using Mitto.IMessaging;
using Mitto.IRouting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mitto.Messaging {

    /// <summary>
    /// Represents an easy to use interface to communicate with the IQueue.IQueue
    /// </summary>
    public class Client : IClient, IEquatable<Client> {
        public string ID { get { return Router.ConnectionID; } }
        private IRequestManager RequestManager { get; set; }
        public IRouter Router { get; private set; }

        public Client(IRouter pRouter, IRequestManager pRequestManager) {
            Router = pRouter;
            RequestManager = pRequestManager;
        }

        #region Request Methods

        /// <summary>
        /// Sends a request over the IQueue connection and runs the
        /// action when the response is received
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <param name="pMessage"></param>
        /// <param name="pAction"></param>
        public void Request<R>(IRequestMessage pMessage, Action<R> pAction) where R : IResponseMessage {
            RequestManager.Request<R>(new Request<R>(this, pMessage, pAction));
        }

        /// <summary>
        /// Runs the Request asynchronously in a Task
        ///
        /// Note that this uses an extra thread
        /// it is recommended to use Request<R>(IRequestMessage, Action<R>) instead
        /// as that uses much less resources
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <param name="pRequest"></param>
        /// <returns></returns>
        public async Task<R> RequestAsync<R>(IRequestMessage pRequest) where R : IResponseMessage {
            return await Task.Run<R>(() => {
                ManualResetEvent objBlock = new ManualResetEvent(false);
                IResponseMessage objResponse = null;
                Request<R>(pRequest, r => {
                    objResponse = r;
                    objBlock.Set();
                });
                objBlock.WaitOne();
                return (R)objResponse;
            });
        }

        /// <summary>
        /// Makes a synchronous request
        ///
        /// Note that this block the application run
        /// It is recommended to use Request<R>(IRequestMessage, Action<R>) instead
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <param name="pRequest"></param>
        /// <returns></returns>
        public R Request<R>(IRequestMessage pRequest) where R : IResponseMessage {
            var objTask = RequestAsync<R>(pRequest);
            objTask.Wait();
            return objTask.Result;
        }

        #endregion Request Methods

        /// <summary>
        /// Transmits an IMessage on the IRouter
        /// </summary>
        /// <param name="pMessage"></param>
        public void Transmit(IMessage pMessage) {
            Router.Transmit(
                new RoutingFrame(
                    RoutingFrameType.Messaging,
                    pMessage.Type,
                    pMessage.ID,
                    Router.ConnectionID,
                    Router.ConnectionID,
                    new Frame(
                        pMessage.Type,
                        pMessage.ID,
                        pMessage.Name,
                        MessagingFactory.Converter.GetByteArray(pMessage)
                    ).GetByteArray()
                ).GetBytes()
            );
        }

        public bool Equals(Client pClient) {
            return (
                this == pClient ||
                this.ID == pClient.ID
            );
        }

        public bool Equals(IClient pClient) {
            return (
                this == pClient ||
                this.ID == pClient.ID
            );
        }

        public bool IsAlive(string pRequestID) {
            return Router.IsAlive(pRequestID);
        }
    }
}
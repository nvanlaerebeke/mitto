using Mitto.IMessaging;
using Mitto.IRouting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mitto.Messaging {
	/// <summary>
	/// Represents an easy to use interface to communicate with the IQueue.IQueue
	/// </summary>
	internal class Client : IClient, IEquatable<Client> {
		public string ID => Guid.NewGuid().ToString();
		private IRequestManager RequestManager { get; set; }
		private IRouter Router { get; set; }

		public Client(IRouter pClient, IRequestManager pRequestManager) {
			Router = pClient;
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
		/// Runs the Request asynchoniously in a Task
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
		/// Makes a synchronious request
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


		#endregion
		/// <summary>
		/// Transmits an IMessage over the IQueue connection
		/// Nothing as response is expected
		/// </summary>
		/// <param name="pMessage"></param>
		public void Transmit(IMessage pMessage) {
			Router.Transmit(new Frame(
				pMessage.Type, 
				pMessage.Name, 
				MessagingFactory.Converter.GetByteArray(pMessage)
			).GetByteArray());
		}

		public bool Equals(Client pClient) {
			return (
				this.ID == pClient.ID
			);
		}

		public bool Equals(IClient pClient) {
			return (
				this.ID == pClient.ID
			);
		}
	}
}
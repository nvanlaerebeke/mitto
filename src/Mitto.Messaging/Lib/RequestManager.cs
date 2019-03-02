using Mitto.IMessaging;
using Mitto.Utilities;
using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
[assembly: InternalsVisibleTo("Mitto.Messaging.Json.Tests")]
namespace Mitto.Messaging {
	/// <summary>
	/// ToDo: Create a IMessageClient (IClient) in the messaging context that
	/// provides communication based on IMessages instead of byte[]
	/// So the following can be done in an action Client.Request<IResponseMessage>(IRequestMessage);
	/// 
	/// Add the following methods do do requests:
	/// - based on a callback method, does not block, IResponseMessage Client.Request<IResponseMessage>(IRequestMessage, Action<IResponseMessage>)
	/// - synchonious, blocks thread, IResponseMessage Client.Request<IResponseMessage>(IRequestMessage)
	/// - async, Task<IResponseMessage> Client.RequesAsync<IResponseMessage>(IRequestMessage)
	/// </summary>
	/// /// <summary>
	/// Starts a request and runs the given action when a response is given
	/// 
	/// For message types where we do not care about waiting for a response we return right away
	/// 
	/// ToDo: add a message to have the ability to know if request is still being processed
	///      
	///       Possible fixes:
	///       1.
	///       check if a message is still being processed by requesting it from the worker(s)
	///       might be a bit tricky as we do not really know what worker is doing the work
	///       an idea might be a broadcast to all workers and wait for a response.
	///	      Could be a bad idea as this will limit the scalibillity and will require to create extra queues for each worker
	///
	///       2.
	///       A different idea might be a pong message that gets send from the workers by themselfs
	///       They know the return path and can let the publishers know they're still busy.
	///       This can be accomplished by keeping a list of actions and do a pong every x seconds that action is running.
	///     
	/// Both solutions will have the downside of still breaking off requests still in the Queue's
	/// But I wonder if we really should keep waiting for those, do we need to still run outdated requests?
	/// I'd say yes if this requester is still waiting for a response.
	/// But the queus should never have messages in them for a long time(this is subjective though), as that would mean we need more workers.
	/// In an ideal world world this should even be auto scaled 
	///  
	/// In practice we know that at some point or another our queues cat start to fill due to for example a problem somewhere else
	/// This might be a bit tricky to handle if we put a timeout here as then we'd have to take it out of the queue & even kill the action.
	/// 
	/// It is not possible to remove items from a queue (AMQP), canceling an action might be as simple as doing a kill on the thread, 
	/// but what negative effects that will have..., as a consumer you'd have to implement transactions for 
	/// all your Messaging.Actions, and we all know developers don't do this.
	/// 
	/// A middle ground and best case might be the following:
	/// 1. The requester puts a request on the Queue
	///    This has a timeout of lets say 60 seconds
	/// 2. Once the Consumer picks up the message, the first thing it does is send a ping request
	///    This ping request is to let this Requester know the message is still being processed and the pong response of this requester 
	///    Will let the Consumer know it's ok to start working on the request
	/// 3. Once the consumer picks up a message and gets a pong response within x seconds it will start processing the request and 
	///    send a ping every x seconds to let this requester know we're working on it
	///    The action doesn't need to listen for the pong as we would not be killing actions once started
	///    No killing of actions due to the fact that we'd be saying all handlers would need to be implemented as transactions
	///    so that it has the ability to be killed at any time.
	///    
	///    In an idea world all actions would be implemented this way, so maybe have a switch to support it per request?, or globally?
	/// 
	/// </summary>
	internal class RequestManager : IRequestManager {
        ConcurrentDictionary<string, IRequest> Requests = new ConcurrentDictionary<string, IRequest>();

		public void Request<T>(IRequest pRequest) where T : IResponseMessage {
			if (Requests.TryAdd(pRequest.Message.ID, pRequest)) {
				pRequest.RequestTimedOut += PRequest_RequestTimedOut;
				pRequest.Transmit();
			}
		}

		public void SetResponse(IResponseMessage pMessage) {
			IRequest objRequest;
			if (
				Requests.ContainsKey(pMessage.ID) &&
				Requests.TryRemove(pMessage.ID, out objRequest)
			) {
				objRequest.RequestTimedOut -= PRequest_RequestTimedOut;
				objRequest.SetResponse(pMessage);
			}
		}

		private void PRequest_RequestTimedOut(object sender, IRequest e) {
			SetResponse(MessagingFactory.Provider.GetResponseMessage(e.Message, ResponseCode.TimeOut));
		}

		public MessageStatusType GetStatus(string pRequestID) {
			if(Requests.ContainsKey(pRequestID)) {
				return MessageStatusType.Queued;
			}
			return MessageStatusType.UnKnown;
		}
	}
}
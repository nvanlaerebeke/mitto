using Mitto.IMessaging;
using System;

namespace Mitto.Messaging.Base {
	internal class MessageRequest {
		private Delegate _objAction;
		/// <summary>
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
		/// <typeparam name="R"></typeparam>
		/// <param name="pClient"></param>
		/// <param name="pMessage"></param>
		/// <param name="action"></param>
		internal void Request<R>(MessageClient pClient, RequestMessage pMessage, Action<R> action) where R : ResponseMessage {
			//only wait when the message actually expects a response, which is only RequestMessage and (Un)SubscribeMessage
			if (
				pMessage.Type == MessageType.Request ||
				pMessage.Type == MessageType.Subscribe ||
				pMessage.Type == MessageType.UnSubscribe ||
				pMessage.Type == MessageType.Event
			) {
				_objAction = action;
				pClient.Queue.Transmit(new IQueue.Message(pClient.ClientID, MessagingFactory.GetMessageCreator().GetBytes(pMessage)));
			} else if (pMessage.Type == MessageType.Notification) { // -- return the response right away for having successfully adding the msg to the queue
				pClient.Queue.Transmit(new IQueue.Message(pClient.ClientID, MessagingFactory.GetMessageCreator().GetBytes(pMessage)));
				SetResponse((R)MessageProvider.GetResponseMessage(pMessage, ResponseCode.Success));
			}
		}

		internal void SetResponse(ResponseMessage pResponse) {
			//((Action<ResponseMessage>)_objAction).Invoke(pResponse); // -- no idea how to get it like this as it's typesafe
			_objAction.DynamicInvoke(pResponse);
		}
	}
}
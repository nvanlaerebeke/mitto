using Mitto.IMessaging;
using System;
using System.Threading;

namespace Mitto.Messaging {
	public class MessageProcessor : IMessageProcessor {
		/// <summary>
		/// Takes in a messages and process it
		/// 
		/// ToDo: 
		///       1. 
		///       Should the MessageProcessor know about the IQueue?, shouldn't it just return 
		///       a message?, but not every request needs a response - So keep it like this?
		///       Maybe in that case the IQueue should be part of the actual message?
		///       
		///      2.
		///      Process shouldn't be getting an IQueue.Message, but just the byte[] that represends the Messaging.Message
		///      The message processing shouldn't care about more then what message we're getting
		///      The queue can be argued because that's where we respond to, but the IQueue.Message is obsolete and shouldn't be cared about
		///      All we're interested in is the IQueue.Message.Data (byte[])
		/// 
		/// </summary>
		/// <param name="pQueue"></param>
		/// <param name="pMessage"></param>
		public void Process(IQueue.IQueue pQueue, IQueue.Message pMessage) {
			new Thread(() => {
				try {
					Job objJob = new Job(new MessageClient(pMessage.ClientID, pQueue), pMessage.Data);

					IMessage objMessage = MessagingFactory.Creator.GetMessage(objJob.Data);
					if (objMessage == null) {  // --- don't know what we're receiving, so skip it
						//Log.Error("Unknown Message");
						return;
					}

					if (objMessage is ResponseMessage) { // -- this is coupling the Request object with the MessageProcessor, don't know how else we can easily improve this.
						Requester.SetResponse(objMessage as ResponseMessage);
					} else {
						RunAction(objJob, objMessage);
					}
				} catch (Exception ex) {
					//Log.Error("Failed to process message", ex);
				}
			}) { IsBackground = true }.Start();
        }

        private static void RunAction(Job pJob, IMessage pMessage) {
            //Run the handler on a seperate thread
            try {
                //Log.Info("Client received message type " + pMessage.Type.ToString());

                var objActionType = IMessaging.MessagingFactory.Provider.GetActionType(pMessage);

                if (objActionType == null) {
					throw new Exception("Unable to create handler for " + pMessage.Name.ToString());
                }

                switch (pMessage.Type) {
                    case MessageType.Notification:
                        objActionType.GetMethod("Start").Invoke(Activator.CreateInstance(objActionType, pJob, pMessage), new object[] { });
                        break;
                    case MessageType.Request:
                    case MessageType.Subscribe:
                    case MessageType.UnSubscribe:
                    case MessageType.Event:
						var objResponse = (ResponseMessage)objActionType.GetMethod("Start").Invoke(Activator.CreateInstance(objActionType, pJob, pMessage), new object[] { });
						var arrData = MessagingFactory.Creator.GetByteArray(objResponse);
						var objMessage = new IQueue.Message(pJob.Client.ClientID, arrData);
						pJob.Client.Queue.Transmit(objMessage);
						break;
                    case MessageType.Response: // -- nothing to do
                        break;
                }
            } catch (Exception ex) {
				/*
				 * ToDo: clean this up we shouldn't need to do this:
				 *   - catch exception -> return generic error message
				 *   - catch MessagingException -> return message with error code from messaging exception
				 *   - NO INNER EXCEPTIONS!! - the actions should throw MessageExceptions when doing validation checks
				 *     Unexpected errors (System.Exception) must be logged and return a generic error message
				 *   
				 *   The reason for this is that we are not interested in exception handling in handlers
				 *   We don't care about the context, an exception = stop, so stop => return response
				 */
				//Log.Error(ex.Message);

				ResponseCode enmCode = ResponseCode.Error;
                if (ex is MessagingException) {
                    enmCode = ((MessagingException)ex).Code;
                }

                if ( //other types than these don't expect a response
                    pMessage.Type != MessageType.Response ||
                    pMessage.Type != MessageType.Notification ||
                    pMessage.Type != MessageType.Event
                ) {
                    var objResponse = MessagingFactory.Creator.GetResponseMessage(pMessage, enmCode);
                    if (objResponse != null) {
						pJob.Client.Queue.Transmit(
							new IQueue.Message(
								pJob.Client.ClientID, 
								MessagingFactory.Creator.GetByteArray(objResponse)
							)
						);
					}
                }

                //Log.Error("Failed processing message " + pMessage.Name.ToString() + " with id " + pMessage.ID.ToString());
                //Log.Info(ex); 
            }
        }
    }
}
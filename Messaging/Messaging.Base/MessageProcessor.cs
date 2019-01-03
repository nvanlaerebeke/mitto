using IMessaging;
using System;
using System.Threading;

namespace Messaging.Base {
	public class MessageProcessor {
		public static void Process(IQueue.IQueue pQueue, IQueue.Message pMessage) {
			new Thread(() => {
				try {
					Job objJob = new Job(new MessageClient(pMessage.ClientID, pQueue), pMessage.Data);

					IMessage objMessage = MessagingFactory.GetMessageCreator().Create(objJob.Data);
					if (objMessage == null) {  // --- don't know what we're receiving, so skip it
						Console.WriteLine("Unknown Message");
						return;
					}

					if (objMessage is ResponseMessage) { // -- this is coupling the Request object with the MessageProcessor, don't know how else we can easily improve this.
						Requester.SetResponse(objMessage as ResponseMessage);
					} else {
						RunAction(objJob, objMessage);
					}
				} catch (Exception ex) {
					Console.WriteLine("Failed to process message", ex);
				}
			}) { IsBackground = true }.Start();
        }

        internal static void RunAction(Job pJob, IMessage pMessage) {
            //Run the handler on a seperate thread
            try {
                Console.WriteLine("Client received message type " + pMessage.Type.ToString());

                var objActionType = MessageProvider.GetActionType(pMessage);

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
						var arrData = MessagingFactory.GetMessageCreator().GetBytes(objResponse);
						var objMessage = new IQueue.Message(pJob.Client.ClientID, arrData);
						pJob.Client.Queue.Respond(objMessage);
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
				Console.WriteLine(ex.Message);

				ResponseCode enmCode = ResponseCode.Error;
                if (ex is MessagingException) {
                    enmCode = ((MessagingException)ex).Code;
                }

                if ( //other types than these don't expect a response
                    pMessage.Type != MessageType.Response ||
                    pMessage.Type != MessageType.Notification ||
                    pMessage.Type != MessageType.Event
                ) {
                    var objResponse = MessageProvider.GetResponseMessage(pMessage, enmCode);
                    if (objResponse != null) {
						pJob.Client.Queue.Transmit(
							new IQueue.Message(
								pJob.Client.ClientID, 
								MessagingFactory.GetMessageCreator().GetBytes(objResponse)
							)
						);
					}
                }

                Console.WriteLine("Failed processing message " + pMessage.Name.ToString() + " with id " + pMessage.ID.ToString());
                Console.WriteLine(ex); // -- log as debug, we don't want this spammed in a production enviroment as it's 'just' logging
            }
        }
    }
}
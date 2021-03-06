﻿using Mitto.Logging;
using Mitto.IRouting;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
[assembly: InternalsVisibleTo("Mitto.Messaging.Json.Tests")]

namespace Mitto.IMessaging {

    /// <summary>
    /// ToDo: Move back to Mitto.Messaging, IMessaging is for interfaces only
    /// Starts a request and runs the given action when a response is given
    /// For message types where we do not care about waiting for a response we return right away
    ///</summary>
    public class RequestManager : IRequestManager {

        private ILog Log {
            get {
                return LoggingFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            }
        }

        private ConcurrentDictionary<string, IRequest> Requests = new ConcurrentDictionary<string, IRequest>();

        public void Request<T>(IRequest pRequest) where T : IResponseMessage {
            if (Requests.TryAdd(pRequest.Message.ID, pRequest)) {
                pRequest.RequestTimedOut += RequestTimedOut;
                pRequest.Transmit();
            }
        }

        public void SetResponse(IResponseMessage pMessage) {
            if (
                Requests.ContainsKey(pMessage.ID) &&
                Requests.TryRemove(pMessage.ID, out IRequest objRequest)
            ) {
                objRequest.RequestTimedOut -= RequestTimedOut;
                objRequest.SetResponse(pMessage);
            } else {
                Log.Error($"Dropping Response {pMessage.ID}, no request found");
            }
        }

        public MessageStatus GetStatus(string pRequestID) {
            if (Requests.ContainsKey(pRequestID)) {
                return MessageStatus.Queued;
            }
            return MessageStatus.UnKnown;
        }

        private void RequestTimedOut(object sender, IRequest e) {
            if (Requests.ContainsKey(e.Message.ID)) {
                if (Requests.TryRemove(e.Message.ID, out IRequest objRequest)) {
                    objRequest.RequestTimedOut -= RequestTimedOut;
                } else {
                    Log.Error($"Unable to remove Request {e.Message.Name}({e.Message.ID}), leaking memory...");
                }
            }
        }
    }
}
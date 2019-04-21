using System;
using System.Runtime.Remoting.Messaging;

namespace Mitto.IRouting {

    public interface ISubscriptionRouter {
        string SourceID { get; }
        string DestinationID { get; }

        bool Sub(RoutingFrame pFrame);

        bool UnSub(RoutingFrame pFrame);

        bool Notify(RoutingFrame pFrame);
    }
}
﻿using Mitto.IConnection;
using Mitto.IRouting;

namespace Mitto.Routing.PassThrough {

    public class RouterProvider : IRouterProvider {

        public IRouter Create(IClientConnection pConnection) {
            return new PassThroughRouter(pConnection);
        }

        /*public IRouter GetSubscriptionRouter<T>(IClient pClient) {
            return new PassThroughSubscriptionRouter<T>(pClient);
        }*/
    }
}
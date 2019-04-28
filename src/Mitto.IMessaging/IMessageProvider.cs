<<<<<<< HEAD
﻿using System.Collections.Generic;
=======
﻿using System;
using System.Collections.Generic;
>>>>>>> 6252b712a9ebbbe3e87e6b4b01b399792be3b1af
using System.Reflection;
using Mitto.IRouting;

namespace Mitto.IMessaging {

    /// <summary>
    /// Providers the messages and actions for the specific provider
    ///
    /// Implement this interface to use a custom MessageProvider
    /// this is done when implementing your own custom way of how
    /// messages are represented(IMessage/IResponseMessage) and how they are handled (IAction)
    /// </summary>
    public interface IMessageProvider {

        void Load(IEnumerable<AssemblyName> pAssemblies);

        IMessage GetMessage(byte[] pData);

        IResponseMessage GetResponseMessage(IRequestMessage pMessage, ResponseStatus pStatus);

        IResponseMessage GetResponseMessage(Type pType, IRequestMessage pMessage, ResponseStatus pStatus);

        IAction GetAction(IClient pClient, IRequestMessage pMessage);

        T GetSubscriptionHandler<T>();

        ISubscriptionHandler GetSubscriptionHandler(IMessage pMessage);
    }
}

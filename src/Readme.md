ToDo:

- Mitto.ClientManager references Metto.Messaging.Base - should be Mitto.IMessaging only
- For requets, add a IRequestMessage interface, it's the same as IMessage but it's 
  so we have a type difference between the base IMessage and derived IRequestMessage and IResponseMessage
- Subscription support
- Keepalive for message processing between Client -> Server -> Queue -> Worker
- Threading - only need ThreadPool.QueueWorkerItem when processing starts (example the Action<T> and callback methods)
- Autoscale ThreadPool.MinThreads so the application ThreadPool autoscales in time
- WebSocketSharp - autoscale the message size within boundaries
- Add bandwidth limiting 
- Create a pull request for the autoscale message sizes and bandwidth limiting/monitoring
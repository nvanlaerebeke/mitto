ToDo:
- Mitto.ClientManager references Metto.Messaging.Base - should be Mitto.IMessaging only
- Mitto.Queue.PassThrough references Mitto.Messaging.Base - should only be passing IConnection.IMessage classes and now know about what a Request/Response msg is   
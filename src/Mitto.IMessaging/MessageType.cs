﻿namespace Mitto.IMessaging {
    public enum MessageType {
        Notification = 0x01,
        Request = 0x02,
        Response = 0x03,
		Sub = 0x04,
		UnSub = 0x05
    }
}

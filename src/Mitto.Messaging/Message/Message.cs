using Mitto.IMessaging;
using System;
using System.Collections.Generic;
using System.IO;

namespace Mitto.Messaging {
    public abstract class Message : IMessage{
        public MessageType Type { get; private set; }

        #region Constructors
        public Message(MessageType pType, string pID) {
            Type = pType;
            ID = pID;
        }

		#endregion

		#region DataMembers
		public virtual string Name {
            get {
                return this.GetType().Name;
            }
        }

		/// <summary>
		/// ToDo: make this an enum and the make it so that request and response use the same code
		/// Can still differentiate becase the message type is different
		/// </summary>
		/// <returns></returns>
        public abstract byte GetCode();

		string _strID;
		public virtual String ID {
			get {
				return _strID;
			}
			set {
				_strID = value;
			}
		}
        #endregion
    }
}
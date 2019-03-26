using Mitto.IMessaging;
using Mitto.IRouting;
using System;

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
﻿namespace Mitto.Routing.RabbitMQ {
	public class Config {
		public string Host { get; set; } = "localhost";
		public string MainQueue { get; set; } = "MittoMain";

		public Config() { }
	}
}
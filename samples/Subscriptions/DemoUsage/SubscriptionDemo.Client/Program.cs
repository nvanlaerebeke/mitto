﻿using System;
using System.Threading;
using Mitto;
using Mitto.Connection.Websocket;
using Mitto.IMessaging;
using Mitto.Messaging.Response;
using Mitto.Subscription.Messaging.Action.Request;
using Mitto.Subscription.Messaging.Request;
using Mitto.Subscription.Messaging.Subscribe;

namespace Quickstart.Client
{
    internal class Program
    {
        private static ManualResetEvent _quit = new ManualResetEvent(false);

        private static Mitto.Client _objClient;

        private static void Main(string[] args)
        {
            //Initialize using the default config
            Config.Initialize();

            //When a message is received, display it on the console
            ReceiveOnChannelRequestAction.ChannelMessageReceived += delegate (string pChannel, string pMessage)
            {
                Console.WriteLine($"{pChannel} > {pMessage}");
            };

            //Establish a connection and subscribe to the "MyChannel"
            _objClient = new Mitto.Client();
            _objClient.Connected += delegate (object sender, Mitto.Client pClient)
            {
                Console.WriteLine("Client Connected");
                _objClient.Request<ACKResponse>(
                    new ChannelSubscribe("MyChannel"), r =>
                    {
                        if (r.Status.State == ResponseState.Success)
                        {
                            Start();
                        }
                        else
                        {
                            Console.WriteLine("Failed Subscribing to Channel");
                        }
                    }
                );
            };
            _objClient.ConnectAsync(new ClientParams()
            {
                Hostname = "localhost",
                Port = 8080,
                Secure = false
            });

            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
            {
                _quit.Set();
            };
            _quit.WaitOne();
        }

        private static void Start()
        {
            ThreadPool.QueueUserWorkItem(s =>
            {
                while (true)
                {
                    var text = Console.ReadLine();
                    _objClient.Request<ACKResponse>(
                        new SendToChannelRequest("MyChannel", text),
                        r =>
                        {
                            if (r.Status.State != ResponseState.Success)
                            {
                                Console.WriteLine($"Failed Sending: {text}");
                            }
                        }
                    );
                }
            });
        }
    }
}
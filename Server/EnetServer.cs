using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENet;

namespace Server
{
    class EnetServer
    {

        private State state;

        public EnetServer(State state)
        {
            this.state = state;
        }

        public void RunServer(ushort port, int maxClients)
        {
            Console.WriteLine("starting server");
            using (Host server = new Host())
            {
                Address address = new Address();
                address.SetIP("127.0.0.1");
                address.Port = port;
                server.Create(address, maxClients);

                Event netEvent;

                while (!Console.KeyAvailable)
                {
                    bool polled = false;

                    while (!polled)
                    {
                        if (state.Outbox.TryDequeue(out var message))
                        {
                            var packet = default(Packet);
                            packet.Create(Encoding.ASCII.GetBytes(message));
                            server.Broadcast(2, ref packet);
                        }

                        if (server.CheckEvents(out netEvent) <= 0)
                        {
                            if (server.Service(15, out netEvent) <= 0)
                                break;

                            polled = true;
                        }
                         
                        switch (netEvent.Type)
                        {
                            case EventType.None:
                                break;

                            case EventType.Connect:
                                Console.WriteLine("Client connected - ID: " + netEvent.Peer.ID + ", IP: " + netEvent.Peer.IP);
                                break;

                            case EventType.Disconnect:
                                Console.WriteLine("Client disconnected - ID: " + netEvent.Peer.ID + ", IP: " + netEvent.Peer.IP);
                                break;

                            case EventType.Timeout:
                                Console.WriteLine("Client timeout - ID: " + netEvent.Peer.ID + ", IP: " + netEvent.Peer.IP);
                                break;

                            case EventType.Receive:
                                Console.WriteLine("Packet received from - ID: " + netEvent.Peer.ID + ", IP: " + netEvent.Peer.IP + ", Channel ID: " + netEvent.ChannelID + ", Data length: " + netEvent.Packet.Length);
                                netEvent.Packet.Dispose();
                                var buffer = new byte[1024];
                                netEvent.Packet.CopyTo(buffer);
                                var receivedmessage = Encoding.ASCII.GetString(buffer);
                                Console.WriteLine("Message received: "+receivedmessage);
                                state.Inbox.Enqueue(receivedmessage);
                                break;
                        }
                    }
                }

                server.Flush();
            }
		}
    }
}

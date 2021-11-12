using System;
using System.Text;
using ENet;
using stuff;

namespace ClientA
{
    public class EnetClient
    {
        private State state;

        public EnetClient(State state)
        {
            this.state = state;
        }

        public void StartClient()
        {
            
            using (Host client = new Host())
            {
                Address address = new Address();

                address.SetHost("127.0.0.1");
                address.Port = 56565;
                client.Create();

                Peer peer = client.Connect(address,10);

                Event netEvent;

                while (true)
                {
                    bool polled = false;

                    while (!polled)
                    {
                        if (state.Outbox.TryPeek(out var message))
                        {
                            var packet = default(Packet); 
                            packet.Create(Encoding.ASCII.GetBytes(message), PacketFlags.Reliable);
                            if (peer.Send(1, ref packet))
                            {
                                state.Outbox.TryDequeue(out var readMessage);
                            }
                        }
                        if (client.CheckEvents(out netEvent) <= 0)
                        {
                            if (client.Service(15, out netEvent) <= 0)
                                break;
                             
                            polled = true;
                        } 

                        switch (netEvent.Type)
                        {
                            case EventType.None:
                                break;

                            case EventType.Connect:
                                Console.WriteLine("Client connected to server");
                                break;

                            case EventType.Disconnect:
                                Console.WriteLine("Client disconnected from server");
                                break;

                            case EventType.Timeout:
                                Console.WriteLine("Client connection timeout");
                                break;

                            case EventType.Receive:
                                Console.WriteLine("Packet received from server - Channel ID: " + netEvent.ChannelID + ", Data length: " + netEvent.Packet.Length);
                                var buffer = new byte[netEvent.Packet.Length];
                                netEvent.Packet.CopyTo(buffer);
                                state.Inbox.Enqueue(Encoding.ASCII.GetString(buffer));
                                netEvent.Packet.Dispose();

                                break;
                        }
                    }
                }

                client.Flush();
            }
		}


    }
}
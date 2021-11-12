using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using ClientA;

namespace stuff
{

    public class Program
    {



        public static int Main(String[] args)
        {
            ENet.Library.Initialize();

            var state = new State();
            var client = new EnetClient(state);
            var thread = new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                /* run your code here */
                client.StartClient();
            });
            thread.Start();

            Thread.Sleep(500);
            state.Outbox.Enqueue("Nummer en");
            Thread.Sleep(500);
            state.Outbox.Enqueue("Nummer to");
            Thread.Sleep(500);
            state.Outbox.Enqueue("Nummer tre");

            Console.ReadKey();
            return 0;
        }


    }

    public class State
    {
        public Guid Id { get; }

        public State()
        {
            Id = Guid.NewGuid();
        }

        public ConcurrentQueue<string> Inbox { get; set; } = new();
        public ConcurrentQueue<string> Outbox { get; set; } = new();
    }
}

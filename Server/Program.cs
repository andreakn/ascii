using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Ascii;
using Microsoft.VisualBasic;
using Server;


public class Program
{
    private readonly State _state;

    public Program(State state)
    {
        _state = state;
    }

    public static void Main()
    {
        ENet.Library.Initialize();
        var state = new State();
        var server = new EnetServer(state);
        server.RunServer(56565,10);

    }

    public void Start()
    {

        while (true)
        {
            if (NativeKeyboard.IsKeyDown(KeyCode.M)) { _state.Outbox.Enqueue($"{_state.Id}:M"); }
            if (NativeKeyboard.IsKeyDown(KeyCode.Right)) { _state.Outbox.Enqueue($"{_state.Id}:>"); }
            if (NativeKeyboard.IsKeyDown(KeyCode.Left)) { _state.Outbox.Enqueue($"{_state.Id}:<"); }
            if (NativeKeyboard.IsKeyDown(KeyCode.Up)) { _state.Outbox.Enqueue($"{_state.Id}:^"); }
            if (NativeKeyboard.IsKeyDown(KeyCode.Down)) { _state.Outbox.Enqueue($"{_state.Id}:V"); }

            while (_state.Inbox.TryDequeue(out var msg))
            {
                Console.WriteLine($"Got msg: {msg}");
            }
        }
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
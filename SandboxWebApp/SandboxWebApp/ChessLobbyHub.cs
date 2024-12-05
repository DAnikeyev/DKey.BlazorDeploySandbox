using ChessClockModel;
using Microsoft.AspNetCore.SignalR;

namespace SandboxWebApp;

public class ChessLobbyHub : Hub
{
    private static int _connectedClients = 0;
    private static readonly int _maxClients = 2; // Set your limit here

    public override async Task OnConnectedAsync()
    {
        if (_connectedClients >= _maxClients)
        {
            await Clients.Caller.SendAsync("ConnectionRejected", "The lobby is full.");
            Context.Abort();
            return;
        }

        _connectedClients++;
        await Clients.Client(Context.ConnectionId).SendAsync("UpdateLobby", ChessClockLobby.Clock);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _connectedClients--;
        await base.OnDisconnectedAsync(exception);
    }

    private static ChessClockLobby ChessClockLobby = new ChessClockLobby("DefaultLobbyId");

    private ChessClock _clock => ChessClockLobby.Clock;
    public async Task HandlePlayer1Click()
    {
        if((_clock.IsRunning == true && _clock.IsPlayer1Turn == true) || _clock.IsPaused == true)
        {
            _clock.SwitchTurn(false);
            await Clients.All.SendAsync("UpdateLobby", _clock);
        }
        if(_clock.IsRunning == false && _clock.IsPlayer1Winner == null)
        {
            _clock.Start(false);
            await Clients.All.SendAsync("UpdateLobby", _clock);
        }
    }

    public async Task HandlePlayer2Click()
    {
        if((_clock.IsRunning == true && _clock.IsPlayer2Turn == true) || _clock.IsPaused == true)
        {
            _clock.SwitchTurn(true);
            await Clients.All.SendAsync("UpdateLobby", _clock);
        }
        if(_clock.IsRunning == false && _clock.IsPlayer1Winner == null)
        {
            _clock.Start(true);
            await Clients.All.SendAsync("UpdateLobby", _clock);
        }
    }

    public async Task PauseClock()
    {
        _clock.Pause();
        await Clients.All.SendAsync("UpdateLobby", _clock);
    }

    public async Task ResetClock()
    {
        _clock.Reset();
        await Clients.All.SendAsync("UpdateLobby", _clock);
    }

    public async Task SetNewClock(int timeControlSeconds, int delaySeconds)
    {
        ChessClockLobby.Clock = new ChessClock(TimeSpan.FromSeconds(timeControlSeconds), TimeSpan.FromSeconds(delaySeconds));
        await Clients.All.SendAsync("UpdateLobby", _clock);
    }
}
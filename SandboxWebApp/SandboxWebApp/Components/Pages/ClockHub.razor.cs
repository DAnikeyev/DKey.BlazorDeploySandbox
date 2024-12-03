using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Threading.Tasks;

public class ClockHub : Hub
{
    private static ConcurrentDictionary<string, int> _lobbyClocks = new ConcurrentDictionary<string, int>();

    public async Task JoinLobby(string lobbyName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, lobbyName);
        if (!_lobbyClocks.ContainsKey(lobbyName))
        {
            _lobbyClocks[lobbyName] = 0;
        }
        await Clients.Group(lobbyName).SendAsync("UpdateClock", _lobbyClocks[lobbyName]);
    }

    public async Task IncrementClock(string lobbyName)
    {
        if (_lobbyClocks.ContainsKey(lobbyName))
        {
            _lobbyClocks[lobbyName]++;
            await Clients.Group(lobbyName).SendAsync("UpdateClock", _lobbyClocks[lobbyName]);
        }
    }

    public async Task DecrementClock(string lobbyName)
    {
        if (_lobbyClocks.ContainsKey(lobbyName))
        {
            _lobbyClocks[lobbyName]--;
            await Clients.Group(lobbyName).SendAsync("UpdateClock", _lobbyClocks[lobbyName]);
        }
    }
}
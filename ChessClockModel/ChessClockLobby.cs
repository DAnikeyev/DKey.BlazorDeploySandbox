namespace ChessClockModel;

public class ChessClockLobby
{
    public ChessClockLobby(string lobbyId, string lobbyName = "Chess Lobby")
    {
        LobbyId = lobbyId;
        LobbyName = lobbyName;
        Clock = new ChessClock(TimeSpan.FromSeconds(90), TimeSpan.FromSeconds(3));
    }

    public ChessClock Clock { get; set; }
    public string LobbyId { get; set; }
    public string LobbyName { get; set; }
    public string? Player1 { get; set; }
    public string? Player2 { get; set; }
    public string? Player1ConnectionId { get; set; }
    public string? Player2ConnectionId { get; set; }

    public bool AddPlayer(string connectionId, string playerName)
    {
        if (Player1 == null)
        {
            Player1 = playerName;
            Player1ConnectionId = connectionId;
            return true;
        }
        if (Player2 == null)
        {
            Player2 = playerName;
            Player2ConnectionId = connectionId;
            return true;
        }
        return false; // Lobby is full
    }

    public bool RemovePlayer(string connectionId)
    {
        if (Player1ConnectionId == connectionId)
        {
            Player1 = null;
            Player1ConnectionId = null;
            return true;
        }
        if (Player2ConnectionId == connectionId)
        {
            Player2 = null;
            Player2ConnectionId = null;
            return true;
        }
        return false; // Player not found
    }

    public int GetPlayerId(string connectionId)
    {
        if (Player1ConnectionId == connectionId)
        {
            return 1;
        }
        if (Player2ConnectionId == connectionId)
        {
            return 2;
        }
        return 0; // Player not found
    }

    public bool IsFull()
    {
        return Player1 != null && Player2 != null;
    }
}
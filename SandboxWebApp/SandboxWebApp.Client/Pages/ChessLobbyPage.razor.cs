using Microsoft.AspNetCore.Components;
using ChessClockModel;
using System.Timers;
using Microsoft.AspNetCore.SignalR.Client;
using Timer = System.Timers.Timer;

namespace SandboxWebApp.Client.Pages;

public partial class ChessLobbyPage : ComponentBase
{
    public static ChessClockLobby? ChessClockLobby { get; set; }

    private HubConnection? hubConnection;

    private Timer? _timer;
    public string displayedTime1 { get; set; } = "00:00.00";
    public string displayedTime2 { get; set; } = "00:00.00";

    public string Player1DisplayName => ChessClockLobby?.Player1 ?? "";
    public string Player2DisplayName => ChessClockLobby?.Player2 ?? "";
    public string WinnerDisplay => ChessClockLobby?.Clock.IsPlayer1Winner switch
    {
        true => $"Player 1 Wins!",
        false => $"Player 2 Wins!",
        _ => ""
    };

    protected override async Task OnInitializedAsync()
    {
        ChessClockLobby = new ChessClockLobby("DefaultLobbyId");
        hubConnection = new HubConnectionBuilder()
            .WithUrl(Navigation.ToAbsoluteUri("/chessLobbyHub"))
            .Build();

        hubConnection.On<ChessClock>("UpdateLobby", (updatedClock) =>
        {
            if (ChessClockLobby != null)
                ChessClockLobby.Clock = updatedClock;
            Refresh();
        });

        try
        {
            await hubConnection.StartAsync();
            Console.WriteLine("SignalR connection established.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error establishing SignalR connection: {ex.Message}");
        }

        hubConnection.SendAsync("Reset");
        _timer = new Timer(100);
        _timer.Elapsed += OnTimerElapsed;
        _timer.Start();
    }

    public async Task HandlePlayer1Click()
    {
        await hubConnection.SendAsync("HandlePlayer1Click");
    }

    public async Task HandlePlayer2Click()
    {
        await hubConnection.SendAsync("HandlePlayer2Click");
    }

    private async Task PauseClock()
    {
        await hubConnection.SendAsync("PauseClock");
    }

    private async Task ResetClock()
    {
        await hubConnection.SendAsync("ResetClock");
    }

    private async Task ApplySettings()
    {
        await hubConnection.SendAsync("SetNewClock", timeControlSeconds, delaySeconds);
        Refresh();
    }


    protected override void OnInitialized()
    {
        _timer = new Timer(100);
        _timer.Elapsed += OnTimerElapsed;
        _timer.Start();
    }

    private void Refresh()
    {
        var clock = ChessClockLobby?.Clock ?? null;
        if ( clock == null)
        {
            return;
        }

        if (!clock.IsRunning)
        {
            displayedTime1 = FormatTime(clock.Time1);
            displayedTime2 = FormatTime(clock.Time2);
            InvokeAsync(StateHasChanged);
            return;
        }

        var delta = DateTime.Now - ChessClockLobby!.Clock.LastUpdated;
        var t1 = clock.IsPlayer2Turn ? clock.Time1 : clock.Time1 - delta;
        var t2 = clock.IsPlayer1Turn ? clock.Time2 : clock.Time2 - delta;
        displayedTime1 = FormatTime(t1);
        displayedTime2 = FormatTime(t2);
        if (t1 <= TimeSpan.Zero || t2 <= TimeSpan.Zero)
        {
            clock.Pause();
        }
        InvokeAsync(StateHasChanged);

    }

    public Task UpdateLobby(ChessClock clock)
    {
        if (ChessClockLobby != null) ChessClockLobby.Clock = clock;
        Refresh();
        return Task.CompletedTask;
    }

    private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        Refresh();
    }

    private string FormatTime(TimeSpan time)
    {
        return $"{time.Minutes:D2}:{time.Seconds:D2}.{time.Milliseconds / 10:D2}";
    }
}
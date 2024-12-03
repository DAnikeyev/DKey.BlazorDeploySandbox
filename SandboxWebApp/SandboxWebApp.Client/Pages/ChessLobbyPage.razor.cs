using Microsoft.AspNetCore.Components;
using ChessClockModel;
using System.Timers;
using Timer = System.Timers.Timer;

namespace SandboxWebApp.Client.Pages;

public partial class ChessLobbyPage : ComponentBase
{
    public ChessClockLobby? ChessClockLobby { get; set; }

    private Timer? _timer;
    public string displayedTime1 { get; set; } = "00:00:00";
    public string displayedTime2 { get; set; } = "00:00:00";

    public string Player1DisplayName => ChessClockLobby?.Player1 ?? "Unknown1";
    public string Player2DisplayName => ChessClockLobby?.Player2 ?? "Unknown2";
    public string WinnerDisplay => ChessClockLobby?.Clock.IsPlayer1Winner switch
    {
        true => $"{Player1DisplayName} Wins!",
        false => $"{Player2DisplayName} Wins!",
        _ => ""
    };

    public void HandlePlayer1Click()
    {
        if((ChessClockLobby?.Clock.IsRunning == true  && ChessClockLobby?.Clock.IsPlayer1Turn == true) || ChessClockLobby?.Clock.IsPaused == true)
        {
            ChessClockLobby?.Clock.SwitchTurn(false);
            Refresh();
        }
        if(ChessClockLobby?.Clock.IsRunning == false && ChessClockLobby?.Clock.IsPlayer1Winner == null)
        {
            ChessClockLobby?.Clock.Start(false);
            Refresh();
        }
    }

    public void HandlePlayer2Click()
    {
        if((ChessClockLobby?.Clock.IsRunning == true && ChessClockLobby?.Clock.IsPlayer2Turn == true) || ChessClockLobby?.Clock.IsPaused == true)
        {
            ChessClockLobby?.Clock.SwitchTurn(true);
            Refresh();
        }
        if(ChessClockLobby?.Clock.IsRunning == false && ChessClockLobby?.Clock.IsPlayer1Winner == null)
        {
            ChessClockLobby?.Clock.Start(true);
            Refresh();
        }
    }

    private void PauseClock()
    {
        ChessClockLobby?.Clock.Pause();
        Refresh();
    }

    private void ResetClock()
    {
        ChessClockLobby?.Clock.Reset();
        Refresh();
    }

    protected override void OnInitialized()
    {
        ChessClockLobby = new ChessClockLobby("DefaultLobbyId");
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

    private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        Refresh();
    }

    private string FormatTime(TimeSpan time)
    {
        return $"{time.Minutes:D2}:{time.Seconds:D2}:{time.Milliseconds / 10:D2}";
    }
}
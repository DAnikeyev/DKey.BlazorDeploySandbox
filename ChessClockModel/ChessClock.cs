namespace ChessClockModel;

[Serializable]
public class ChessClock
{
    public TimeSpan InitialTime1 { get; set; }
    public TimeSpan InitialTime2 { get; set; }
    public TimeSpan Delay { get; set; }
    public TimeSpan Time1 { get; set; }
    public TimeSpan Time2 { get; set; }
    public bool IsRunning { get; set; }

    public bool IsPaused { get; set; }
    public bool? IsPlayer1Winner { get; set; }

    public DateTime LastUpdated { get; set; }
    public bool IsPlayer1Turn { get; set; }
    public bool IsPlayer2Turn => !IsPlayer1Turn;

    public ChessClock()
    {
    }

    public ChessClock(TimeSpan time, TimeSpan delay)
    {
        InitialTime1 = time;
        InitialTime2 = time;
        Delay = delay;
        Time1 = time;
        Time2 = time;
        IsRunning = false;
        IsPlayer1Turn = true;
    }
    public ChessClock(TimeSpan time1, TimeSpan time2, TimeSpan delay)
    {
        InitialTime1 = time1;
        InitialTime2 = time2;
        Delay = delay;
        Time1 = time1;
        Time2 = time2;
        IsRunning = false;
        IsPlayer1Turn = true;
    }

    public void SwitchTurn(bool isPlayer1Turn)
    {
        IsRunning = true;
        if (IsPaused)
        {
            IsPaused = false;
            LastUpdated = DateTime.Now;
        }
        UpdateTime();
        IsPlayer1Turn = isPlayer1Turn;
        if(isPlayer1Turn)
        {
            Time2 += Delay;
        }
        else
        {
            Time1 += Delay;
        }
    }

    public void Start(bool isPlayer1Turn)
    {
        Reset();
        IsRunning = true;
        IsPlayer1Turn = isPlayer1Turn;
        LastUpdated = DateTime.Now;
    }

    public void Pause()
    {
        UpdateTime();
        IsRunning = false;
        IsPaused = true;
    }

    public void Reset()
    {
        Time1 = InitialTime1;
        Time2 = InitialTime2;
        IsRunning = false;
        IsPlayer1Turn = false;
        IsPlayer1Winner = null;
    }

    public void SetTime(TimeSpan time1, TimeSpan time2)
    {
        Time1 = time1;
        Time2 = time2;
    }

    private bool CheckWin()
    {
        if (Time1 <= TimeSpan.Zero)
        {
            IsPlayer1Winner = false;
            return true;
        }
        if (Time2 <= TimeSpan.Zero)
        {
            IsPlayer1Winner = true;
            return true;
        }

        return false;
    }

    public void Finish()
    {
        if(Time1 < TimeSpan.Zero)
        {
            Time1 = TimeSpan.Zero;
        }
        if(Time2 < TimeSpan.Zero)
        {
            Time2 = TimeSpan.Zero;
        }
    }

    private void UpdateTime()
    {
        if(!IsRunning) return;
        var now = DateTime.Now;
        var elapsed = now - LastUpdated;
        LastUpdated = now;
        if (IsPlayer1Turn)
        {
            Time1 -= elapsed;
        }
        else
        {
            Time2 -= elapsed;
        }

        if (CheckWin())
        {
            IsRunning = false;
            Finish();
        }
    }
}
using System.Text.Json;
using ChessClockModel;

namespace ModelTests;

public class ChessClockTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        var clock = new ChessClock(TimeSpan.FromMinutes(5), TimeSpan.FromSeconds(5));
        Assert.DoesNotThrow(() =>
        {
            string json = JsonSerializer.Serialize(clock);
            var deserializedClock = JsonSerializer.Deserialize<ChessClock>(json);
            Console.WriteLine("Serialization and deserialization successful.");
        });
    }
}
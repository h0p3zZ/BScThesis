namespace JsonVerifier.Models;

internal class Schedule
{
    public int Horizon { get; set; }
    public required Dictionary<string, Task> Tasks { get; set; }
}

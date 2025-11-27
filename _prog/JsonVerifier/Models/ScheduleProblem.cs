namespace JsonVerifier.Models;

public class ScheduleProblem
{
    public int Horizon { get; set; } // horizon in time slots exclusive
    public required Dictionary<string, Task> Tasks { get; set; }
}

namespace JsonVerifier.Models;

public class ScheduleProblem
{
    public int Horizon { get; set; }
    public required Dictionary<string, Task> Tasks { get; set; }
}

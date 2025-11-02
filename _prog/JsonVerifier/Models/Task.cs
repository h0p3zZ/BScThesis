namespace JsonVerifier.Models;

public class Task
{
    public required string Name { get; set; }
    public int Duration { get; set; }
    public double Importance { get; set; }
    public int Urgency { get; set; }
    public required List<IntervalCost> TimeSlots { get; set; }
    public List<Dependency>? Dependencies { get; set; }
    public Location? Location { get; set; }
}

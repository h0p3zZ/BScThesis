namespace JsonVerifier.Models;

public class Task
{
    public required string Name { get; set; }
    public required int Duration { get; set; }
    public required double Importance { get; set; } // Double used to allow for infinite importance
    public required int Urgency { get; set; }
    public required List<IntervalCost> TimeSlots { get; set; }
    public List<Dependency>? Dependencies { get; set; }
    public Location? Location { get; set; }
}

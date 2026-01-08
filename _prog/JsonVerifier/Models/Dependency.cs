namespace JsonVerifier.Models;

public class Dependency
{
    public required string Task { get; set; }
    public required List<IntervalCost> Intervals { get; set; }
    public required double UnmetCost { get; set; } // Double used to allow for infinite importance
}

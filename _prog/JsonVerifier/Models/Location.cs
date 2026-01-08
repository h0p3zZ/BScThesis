namespace JsonVerifier.Models;

public class Location
{
    public required int Id { get; set; }
    public required double UnmetCost { get; set; } // Double used to allow for infinite importance
}

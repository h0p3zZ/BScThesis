using JsonVerifier.Models;
using SolutionVerifier;

namespace VerfifierTests;

[TestClass]
public sealed class SolutionVerifierTests
{
    private ScheduleProblem _schedule = new()
    {
        Horizon = 20,
        Tasks = new Dictionary<string, JsonVerifier.Models.Task>
        {
            {
                "Task1", new JsonVerifier.Models.Task
                {
                    Name = "Task 1",
                    Duration = 3,
                    Importance = 5,
                    Urgency = 1,
                    TimeSlots = new List<IntervalCost>
                    {
                        new() { Interval = [0, 4], Cost = 1 },
                        new() { Interval = [4, 10], Cost = 2 },
                    },
                    Location = new Location { Id = 1, UnmetCost = 1 },
                }
            },
            {
                "Task2", new JsonVerifier.Models.Task
                {
                    Name = "Task 2",
                    Duration = 3,
                    Importance = 5,
                    Urgency = 2,
                    TimeSlots = new List<IntervalCost>
                    {
                        new() { Interval = [0, 4], Cost = 1 },
                        new() { Interval = [4, 10], Cost = 2 },
                    },
                    Location = new Location { Id = 2, UnmetCost = 3},
                }
            },
            {
                "Task3", new JsonVerifier.Models.Task
                {
                    Name = "Task 3",
                    Duration = 3,
                    Importance = Double.PositiveInfinity,
                    Urgency = 3,
                    TimeSlots = new List<IntervalCost>
                    {
                        new() { Interval = [0, 4], Cost = 1 },
                        new() { Interval = [4, 10], Cost = 2 },
                    },
                    Location = new Location { Id = 2, UnmetCost = 5 },
                }
            },
        }
    };

    private SolutionVerifier.SolutionVerifier _verifier;

    [TestInitialize]
    public void SetUp()
    {

        _verifier = new SolutionVerifier.SolutionVerifier(_schedule);
    }

    [TestMethod]
    [DataRow(0, 3, 6, 3)] // Task1 and Task2 scheduled back to back at different locations task2 location cost = 3
    [DataRow(0, 6, 3, 5)] // Task1 and Task3 scheduled back to back at different locations task3 location cost = 5
    [DataRow(3, 0, 6, 6)] // Task2 and Task 1 back to back and Task1 and Task3 at different different locations, location cost = 6
    [DataRow(3, 6, 0, 4)] // task3-task1 location cost = 1, task1-task2 location cost = 3, total = 4
    [DataRow(6, 3, 0, 1)] // task3-task2 location cost = 0, task2-task1 location cost = 1, total = 1
    [DataRow(6, 0, 3, 1)] // task2-task3 location cost = 0, task3-task1 location cost = 1, total = 1
    [DataRow(0, 6, 12, 0)] // no tasks are back to back, location cost = 0
    public void TestLocationCostCorrectlyCalculated(int? assignmentT1, int? assignmentT2, int? assignmentT3, double expectedLocationCost)
    {
        var assignments = new Dictionary<string, int?>
        {
            { "Task1", assignmentT1 },
            { "Task2", assignmentT2 },
            { "Task3", assignmentT3 },
        };

        var cost = _verifier.CalcCost(assignments);

        Assert.IsTrue(cost.Valid);
        Assert.AreEqual(expectedLocationCost, cost.LocationCost);
    }
}

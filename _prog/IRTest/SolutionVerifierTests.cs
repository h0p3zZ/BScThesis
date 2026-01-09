using JsonVerifier.Models;

namespace VerifierTests;

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
                        new() { Interval = [10, 20], Cost = 3 },
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
                        new() { Interval = [10, 20], Cost = 3 },
                    },
                    Dependencies = new List<Dependency>
                    {
                        new() {
                            Task = "Task1",
                            Intervals = new List<IntervalCost>
                            {
                                new() { Interval = [0, 10], Cost = 15 },
                            },
                            UnmetCost = 5,
                        }
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
                        new() { Interval = [10, 20], Cost = 3 },
                    },
                    Location = new Location { Id = 2, UnmetCost = 5 },
                }
            },
            {
                "Task4", new JsonVerifier.Models.Task
                {
                    Name = "Task 4",
                    Duration = 3,
                    Importance = 5,
                    Urgency = 3,
                    TimeSlots = new List<IntervalCost>
                    {
                        new() { Interval = [0, 4], Cost = 1 },
                        new() { Interval = [4, 10], Cost = 2 },
                        new() { Interval = [11, 20], Cost = 3 },
                    },
                    Location = new Location { Id = 2, UnmetCost = double.PositiveInfinity },
                }
            },
            {
                "Task5", new JsonVerifier.Models.Task
                {
                    Name = "Task 5",
                    Duration = 3,
                    Importance = 5,
                    Urgency = 3,
                    TimeSlots = new List<IntervalCost>
                    {
                        new() { Interval = [0, 20], Cost = 1 },
                    },
                    Dependencies = new List<Dependency>
                    {
                        new() {
                            Task = "Task1",
                            Intervals = new List<IntervalCost>
                            {
                                new() { Interval = [0, 10], Cost = 15 },
                            },
                            UnmetCost = double.PositiveInfinity,
                        }
                    },
                    Location = new Location { Id = 3, UnmetCost = 2 },
                }
            }
        }
    };

    private readonly SolutionVerifier.SolutionVerifier _verifier;

    public SolutionVerifierTests()
    {
        _verifier = new SolutionVerifier.SolutionVerifier(_schedule);
    }

    [TestMethod]
    [DataRow(0, 5, 200)]
    [DataRow(0, 5, 20)]
    [DataRow(0, 5, 18)]
    public void TestExceedHorizon(int? assignmentT1, int? assignmentT3, int? assignmentT4)
    {
        var assignments = new Dictionary<string, int?>
        {
            { "Task1", assignmentT1 },
            { "Task3", assignmentT3 },
            { "Task4", assignmentT4 },
        };

        var cost = _verifier.CalcCost(assignments, out var messages);

        foreach (var message in messages)
            Console.WriteLine(message);

        Assert.IsNull(cost);
        Assert.IsTrue(messages.Any(x => x.Contains("horizon")));
    }

    // new () { Interval = [0, 4], Cost = 1 },
    // new () { Interval = [4, 10], Cost = 2 },
    // new () { Interval = [10, 20], Cost = 3 },

    [TestMethod]
    [DataRow(10, 13, 16, 3 * 3)]
    [DataRow(0, 4, 10, 1 + 2 + 3)]
    [DataRow(4, 7, 10, 2 * 2 + 3)]
    [DataRow(null, 0, 10, 0 + 1 + 3)]
    [DataRow(null, null, 3, (1 + 2 * 2) / 3d)]
    public void TestValidTimeSlots(int? assignmentT1, int? assignmentT2, int? assignmentT3, double timeSlotCost)
    {
        var assignments = new Dictionary<string, int?>
        {
            { "Task1", assignmentT1 },
            { "Task2", assignmentT2 },
            { "Task3", assignmentT3 },
        };

        var cost = _verifier.CalcCost(assignments, out var messages);

        foreach (var message in messages)
            Console.WriteLine(message);

        Assert.IsNotNull(cost);
        Assert.AreEqual(timeSlotCost, cost.TimeSlotCost);
    }

    [TestMethod]
    public void TestInvalidTimeSlots()
    {
        var assignments = new Dictionary<string, int?>
        {
            { "Task1", 0 },
            { "Task2", 4 },
            { "Task4", 10 },
        };

        var cost = _verifier.CalcCost(assignments, out var messages);

        foreach (var message in messages)
            Console.WriteLine(message);

        Assert.IsNull(cost);
        Assert.IsTrue(messages.Any(x => x.Contains("partially scheduled")));
    }


    [TestMethod]
    [DataRow(0, 0, 0)]
    [DataRow(0, 2, 4)]
    [DataRow(0, 3, 5)]
    public void TestOverlap(int? assignmentT1, int? assignmentT3, int? assignmentT4)
    {
        var assignments = new Dictionary<string, int?>
        {
            { "Task1", assignmentT1 },
            { "Task3", assignmentT3 },
            { "Task4", assignmentT4 },
        };

        var cost = _verifier.CalcCost(assignments, out var messages);

        foreach (var message in messages)
            Console.WriteLine(message);

        Assert.IsNull(cost);
        Assert.IsTrue(messages.Any(x => x.Contains("overlap")));
    }

    [TestMethod]
    [DataRow(0, 3, 6, 3)] // Task1 and Task2 scheduled back to back at different locations task2 location cost = 3
    [DataRow(0, 6, 3, 5)] // Task1 and Task3 scheduled back to back at different locations task3 location cost = 5
    [DataRow(3, 0, 6, 6)] // Task2 and Task 1 back to back and Task1 and Task3 at different different locations, location cost = 6
    [DataRow(3, 6, 0, 4)] // task3-task1 location cost = 1, task1-task2 location cost = 3, total = 4
    [DataRow(6, 3, 0, 1)] // task3-task2 location cost = 0, task2-task1 location cost = 1, total = 1
    [DataRow(6, 0, 3, 1)] // task2-task3 location cost = 0, task3-task1 location cost = 1, total = 1
    [DataRow(0, 6, 12, 0)] // no tasks are back to back, location cost = 0
    public void TestValidLocationCost(int? assignmentT1, int? assignmentT2, int? assignmentT3, double expectedLocationCost)
    {
        var assignments = new Dictionary<string, int?>
        {
            { "Task1", assignmentT1 },
            { "Task2", assignmentT2 },
            { "Task3", assignmentT3 },
        };

        var cost = _verifier.CalcCost(assignments, out var messages);

        foreach (var message in messages)
            Console.WriteLine(message);

        Assert.IsNotNull(cost);
        Assert.AreEqual(expectedLocationCost, cost.LocationCost);
    }

    [TestMethod]
    [DataRow(0, 10, 3)] // Task1-Task4 have different locations Task4 location cost = infinite, therfore the solution is invalid
    public void TestInvalidLocationCost(int? assignmentT1, int? assignmentT3, int? assignmentT4)
    {
        var assignments = new Dictionary<string, int?>
        {
            { "Task1", assignmentT1 },
            { "Task3", assignmentT3 },
            { "Task4", assignmentT4 },
        };

        var cost = _verifier.CalcCost(assignments, out var messages);

        foreach (var message in messages)
            Console.WriteLine(message);

        Assert.IsNull(cost);
        Assert.IsTrue(messages.Any(x => x.Contains("mandatory same location")));
    }

    [TestMethod]
    public void TestValidImportanceCost()
    {
        var assignments = new Dictionary<string, int?>
        {
            { "Task1", 0 },
            { "Task2", null }, // Not scheduled, should incur importance cost
            { "Task3", 3 },
            { "Task4", 6 },
        };

        var cost = _verifier.CalcCost(assignments, out var messages);

        foreach (var message in messages)
            Console.WriteLine(message);

        Assert.IsNotNull(cost);
        Assert.AreEqual(_schedule.Tasks["Task2"].Importance, cost.ImportanceCost);
    }

    [TestMethod]
    [DataRow(null, null, null)] // All tasks not scheduled, Task3 importance is infinite, therefore the solution is invalid
    [DataRow(0, 5, null)]
    [DataRow(0, null, null)]
    [DataRow(null, 0, null)]
    public void TestInvalidImportanceCost(int? assignmentT1, int? assignmentT2, int? assignmentT3)
    {
        var assignments = new Dictionary<string, int?>
        {
            { "Task1", assignmentT1 },
            { "Task2", assignmentT2 },
            { "Task3", assignmentT3 },
        };

        var cost = _verifier.CalcCost(assignments, out var messages);

        foreach (var message in messages)
            Console.WriteLine(message);

        Assert.IsNull(cost);
        Assert.IsTrue(messages.Any(x => x.Contains("infinite importance cost")));
    }

    [TestMethod]
    [DataRow(0, 3, 10, 15)]
    public void TestValidDependencyCost(int? assignmentT1, int? assignmentT2, int? assignmentT3, double dependencyCost)
    {
        var assignments = new Dictionary<string, int?>
        {
            { "Task1", assignmentT1 },
            { "Task2", assignmentT2 },
            { "Task3", assignmentT3 },
        };

        var cost = _verifier.CalcCost(assignments, out var messages);

        foreach (var message in messages)
            Console.WriteLine(message);

        Assert.IsNotNull(cost);
        Assert.AreEqual(dependencyCost, cost.DependencyCost);
    }

    [TestMethod]
    public void TestInvalidDependencyCost()
    {
        var assignments = new Dictionary<string, int?>
        {
            { "Task1", 0 },
            { "Task5", 12 },
        };
        var cost = _verifier.CalcCost(assignments, out var messages);

        foreach (var message in messages)
            Console.WriteLine(message);

        Assert.IsNull(cost);
        Assert.IsTrue(messages.Any(x => x.Contains("mandatory dependency")));
    }
}

using JsonVerifier;

namespace VerifierTests;

[TestClass]
public sealed class JsonVerifierTests
{
    [TestMethod]
    [DataRow("ExampleSchedules/generated1.json")]
    [DataRow("ExampleSchedules/generated2.json")]
    [DataRow("ExampleSchedules/generated3.json")]
    [DataRow("ExampleSchedules/generated4.json")]
    [DataRow("ExampleSchedules/generated5.json")]
    [DataRow("ExampleSchedules/generated6.json")]
    [DataRow("ExampleSchedules/generated7.json")]
    [DataRow("ExampleSchedules/schedule.json")]
    [DataRow("ExampleSchedules/validSchedule.json")]
    public void TestValidJson(string filePath)
    {
        var verifier = new JsonParser(filePath);

        var isValid = verifier.ValidateSchema(out var messages);

        foreach (var message in messages)
        {
            Console.WriteLine(message);
        }

        Assert.IsTrue(isValid, "Expected JSON to be valid.");
    }

    [TestMethod]
    [DataRow("ExampleSchedules/invalidSchedule.json")]
    public void TestInvalidJson(string filePath)
    {
        var verifier = new JsonParser(filePath);

        var isValid = verifier.ValidateSchema(out var messages);

        foreach (var message in messages)
        {
            Console.WriteLine(message);
        }

        Assert.IsFalse(isValid, "Expected JSON to be invalid.");
    }
}

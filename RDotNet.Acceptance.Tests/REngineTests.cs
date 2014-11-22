using RDotNet.Client;
using Moq;
using NUnit.Framework;

namespace RDotNet.Acceptance.Tests
{
    [TestFixture]
    public class REngineTests
    {
        [Test]
        public void ConfiguresEngineProperly()
        {
            var engine = new REngine(123456, new Mock<IRManagementAPI>().Object, new Mock<IRObjectAPI>().Object, new Mock<IROutputAPI>().Object);

            Assert.That(engine.ID, Is.EqualTo(123456));
            Assert.That(engine.EngineName, Is.EqualTo("R.NET"));
        }
    }
}

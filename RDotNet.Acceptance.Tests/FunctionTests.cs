using RDotNet.Client;
using RDotNet.R.Adapter;
using RDotNet.Server;
using Moq;
using NUnit.Framework;

namespace RDotNet.Acceptance.Tests
{
    [TestFixture]
    public class FunctionTests
    {
        [Test]
        public void Test()
        {
            var handle = new Mock<IRSafeHandle>();
            handle.SetupGet(s => s.API).Returns(new Mock<IRObjectAPI>().Object);
            var context = new Mock<SymbolicExpressionContext>();
            context.SetupGet(g => g.Type).Returns(SymbolicExpressionType.BuiltinFunction);
            handle.SetupGet(s => s.Context).Returns(context.Object);
            var bif = new BuiltinFunction(handle.Object);

            Assert.That(bif, Is.Not.Null);
        }

    }
}

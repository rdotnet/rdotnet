using NUnit.Framework;
using RDotNet.Utilities;
using System.Numerics;

namespace RDotNet
{
    [TestFixture]
    public class UtilityTests
    {
        [Test]
        public void CanSerializeComplexValues()
        {
            var result = RTypesUtil.SerializeComplexToDouble(new[] { new Complex(1, 0), new Complex(0, 1), new Complex(1, 1) });

            Assert.That(result, Is.EqualTo(new[] { 1d, 0, 0, 1, 1, 1 }));
        }
    }
}
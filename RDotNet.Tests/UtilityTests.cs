using Xunit;
using RDotNet.Utilities;
using System.Numerics;

namespace RDotNet
{
    [Collection("R.NET unit tests")]
    public class UtilityTests
    {
        [Fact]
        public void CanSerializeComplexValues()
        {
            var result = RTypesUtil.SerializeComplexToDouble(new[] { new Complex(1, 0), new Complex(0, 1), new Complex(1, 1) });

            Assert.Equal(result, (new[] { 1d, 0, 0, 1, 1, 1 }));
        }
    }
}
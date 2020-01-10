using Xunit;

namespace RDotNet
{
    public class CharacterMatrixTest : RDotNetTestFixture
    {
        [Fact]
        public void TestCharacter()
        {
            SetUpTest();
            var engine = this.Engine;
            var matrix = engine.Evaluate("x <- matrix(c(1, NA, 2, 3, NA, 4), nrow=3, ncol=2, byrow=TRUE)").AsCharacterMatrix();
            Assert.Equal(3, matrix.RowCount);
            Assert.Equal(2, matrix.ColumnCount);
            Assert.Equal("1", matrix[0,0]);
            Assert.Null(matrix[0,1]);
            Assert.Equal("2", matrix[1, 0]);
            Assert.Equal("3", matrix[1, 1]);
            Assert.Null(matrix[2, 0]);
            Assert.Equal("4", matrix[2, 1]);
            
            matrix[0, 0] = null;
            matrix[0, 1] = "A";
            Assert.Null(matrix[0,0]);
            Assert.Equal("A", matrix[0, 1]);

            var logical = engine.Evaluate("is.na(x)").AsLogical();  // This gets strung out as col1 + col2
            // The original x was never modified, just our copy of it did.  So the logical should reflect the
            // original value of x.
            Assert.False(logical[0]); // 1
            Assert.False(logical[1]); // 2
            Assert.True(logical[2]);  // NA
            Assert.True(logical[3]);  // NA
            Assert.False(logical[4]); // 3
            Assert.False(logical[5]); // 4
        }

        [Fact]
        public void TestCharacter_NoConversion()
        {
            SetUpTest();
            var engine = this.Engine;
            var matrix = engine.Evaluate("x <- matrix(c('1', NA, '2', '3', NA, '4'), nrow=3, ncol=2, byrow=TRUE)").AsCharacterMatrix();
            Assert.Equal(3, matrix.RowCount);
            Assert.Equal(2, matrix.ColumnCount);
            Assert.Equal("1", matrix[0, 0]);
            Assert.Null(matrix[0, 1]);
            Assert.Equal("2", matrix[1, 0]);
            Assert.Equal("3", matrix[1, 1]);
            Assert.Null(matrix[2, 0]);
            Assert.Equal("4", matrix[2, 1]);
            
            matrix[0, 0] = null;
            matrix[0, 1] = "A";
            Assert.Null(matrix[0, 0]);
            Assert.Equal("A", matrix[0, 1]);

            var logical = engine.Evaluate("is.na(x)").AsLogical();
            Assert.True(logical[0]);  // Was 1, now NA because we modified the original x
            Assert.False(logical[1]); // 2
            Assert.True(logical[2]);  // NA
            Assert.False(logical[3]); // "A"
            Assert.False(logical[4]); // 3
            Assert.False(logical[5]); // 4
        }
    }
}
using RDotNet.Internals;
using RDotNet.Utilities;
using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace RDotNet
{
    /// <summary>
    /// A matrix of real numbers in double precision.
    /// </summary>
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public class NumericMatrix : Matrix<double>
    {
        /// <summary>
        /// Creates a new empty NumericMatrix with the specified size.
        /// </summary>
        /// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
        /// <param name="rowCount">The row size.</param>
        /// <param name="columnCount">The column size.</param>
        /// <seealso cref="REngineExtension.CreateNumericMatrix(REngine, int, int)"/>
        public NumericMatrix(REngine engine, int rowCount, int columnCount)
            : base(engine, SymbolicExpressionType.NumericVector, rowCount, columnCount)
        { }

        /// <summary>
        /// Creates a new NumericMatrix with the specified values.
        /// </summary>
        /// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
        /// <param name="matrix">The values.</param>
        /// <seealso cref="REngineExtension.CreateNumericMatrix(REngine, double[,])"/>
        public NumericMatrix(REngine engine, double[,] matrix)
            : base(engine, SymbolicExpressionType.NumericVector, matrix)
        { }

        /// <summary>
        /// Creates a new instance for a numeric matrix.
        /// </summary>
        /// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
        /// <param name="coerced">The pointer to a numeric matrix.</param>
        protected internal NumericMatrix(REngine engine, IntPtr coerced)
            : base(engine, coerced)
        { }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="rowIndex">The zero-based rowIndex index of the element to get or set.</param>
        /// <param name="columnIndex">The zero-based columnIndex index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        public override double this[int rowIndex, int columnIndex]
        {
            get
            {
                if (rowIndex < 0 || RowCount <= rowIndex)
                {
                    throw new ArgumentOutOfRangeException("rowIndex");
                }
                if (columnIndex < 0 || ColumnCount <= columnIndex)
                {
                    throw new ArgumentOutOfRangeException("columnIndex");
                }
                using (new ProtectedPointer(this))
                {
                    var data = new byte[DataSize];
                    IntPtr pointer = DataPointer;
                    int offset = GetOffset(rowIndex, columnIndex);
                    for (int byteIndex = 0; byteIndex < data.Length; byteIndex++)
                    {
                        data[byteIndex] = Marshal.ReadByte(pointer, offset + byteIndex);
                    }
                    return BitConverter.ToDouble(data, 0);
                }
            }
            set
            {
                if (rowIndex < 0 || RowCount <= rowIndex)
                {
                    throw new ArgumentOutOfRangeException("rowIndex");
                }
                if (columnIndex < 0 || ColumnCount <= columnIndex)
                {
                    throw new ArgumentOutOfRangeException("columnIndex");
                }
                using (new ProtectedPointer(this))
                {
                    byte[] data = BitConverter.GetBytes(value);
                    IntPtr pointer = DataPointer;
                    int offset = GetOffset(rowIndex, columnIndex);
                    for (int byteIndex = 0; byteIndex < data.Length; byteIndex++)
                    {
                        Marshal.WriteByte(pointer, offset + byteIndex, data[byteIndex]);
                    }
                }
            }
        }

        /// <summary>
        /// Initializes this R matrix, using the values in a rectangular array.
        /// </summary>
        /// <param name="matrix"></param>
        protected override void InitMatrixFastDirect(double[,] matrix)
        {
            var values = ArrayConverter.ArrayConvertOneDim(matrix);
            Marshal.Copy(values, 0, DataPointer, values.Length);
        }

        /// <summary>
        /// Gets a rectangular array representation in the CLR, equivalent of a matrix in R.
        /// </summary>
        /// <returns>Rectangular array with values representing the content of the R matrix. Beware NA codes</returns>
        protected override double[,] GetArrayFast()
        {
            var values = new double[this.ItemCount];
            Marshal.Copy(DataPointer, values, 0, values.Length);
            return ArrayConverter.ArrayConvertAllTwoDim(values, this.RowCount, this.ColumnCount);
        }

        /// <summary>
        /// Gets the size of a real number in byte.
        /// </summary>
        protected override int DataSize
        {
            get { return sizeof(double); }
        }
    }
}
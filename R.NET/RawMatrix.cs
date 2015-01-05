using RDotNet.Internals;
using RDotNet.Utilities;
using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace RDotNet
{
    /// <summary>
    /// A matrix of byte values.
    /// </summary>
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public class RawMatrix : Matrix<byte>
    {
        /// <summary>
        /// Creates a new RawMatrix with the specified size.
        /// </summary>
        /// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
        /// <param name="rowCount">The row size.</param>
        /// <param name="columnCount">The column size.</param>
        /// <seealso cref="REngineExtension.CreateRawMatrix(REngine, int, int)"/>
        public RawMatrix(REngine engine, int rowCount, int columnCount)
            : base(engine, SymbolicExpressionType.RawVector, rowCount, columnCount)
        { }

        /// <summary>
        /// Creates a new RawMatrix with the specified values.
        /// </summary>
        /// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
        /// <param name="matrix">The values.</param>
        /// <seealso cref="REngineExtension.CreateRawMatrix(REngine, byte[,])"/>
        public RawMatrix(REngine engine, byte[,] matrix)
            : base(engine, SymbolicExpressionType.RawVector, matrix)
        { }

        /// <summary>
        /// Creates a new instance for a raw matrix.
        /// </summary>
        /// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
        /// <param name="coerced">The pointer to a raw matrix.</param>
        protected internal RawMatrix(REngine engine, IntPtr coerced)
            : base(engine, coerced)
        { }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="rowIndex">The zero-based rowIndex index of the element to get or set.</param>
        /// <param name="columnIndex">The zero-based columnIndex index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        public override byte this[int rowIndex, int columnIndex]
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
                    int offset = GetOffset(rowIndex, columnIndex);
                    return Marshal.ReadByte(DataPointer, offset);
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
                    int offset = GetOffset(rowIndex, columnIndex);
                    Marshal.WriteByte(DataPointer, offset, value);
                }
            }
        }

        /// <summary>
        /// Initializes this R matrix, using the values in a rectangular array.
        /// </summary>
        /// <param name="matrix"></param>
        protected override void InitMatrixFastDirect(byte[,] matrix)
        {
            var values = ArrayConverter.ArrayConvertOneDim(matrix);
            Marshal.Copy(values, 0, DataPointer, values.Length);
        }

        /// <summary>
        /// Efficient conversion from R vector representation to the array equivalent in the CLR
        /// </summary>
        /// <returns>Array equivalent</returns>
        protected override byte[,] GetArrayFast()
        {
            byte[] values = new byte[this.ItemCount];
            Marshal.Copy(DataPointer, values, 0, values.Length);
            return ArrayConverter.ArrayConvertAllTwoDim(values, this.RowCount, this.ColumnCount);
        }

        /// <summary>
        /// Gets the size of an Raw in byte.
        /// </summary>
        protected override int DataSize
        {
            get { return sizeof(byte); }
        }
    }
}
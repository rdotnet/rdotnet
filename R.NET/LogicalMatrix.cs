using RDotNet.Internals;
using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace RDotNet
{
   /// <summary>
   /// A matrix of Boolean values.
   /// </summary>
   [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
   public class LogicalMatrix : Matrix<bool>
   {
      /// <summary>
      /// Creates a new empty LogicalMatrix with the specified size.
      /// </summary>
      /// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
      /// <param name="rowCount">The row size.</param>
      /// <param name="columnCount">The column size.</param>
      /// <seealso cref="REngineExtension.CreateLogicalMatrix(REngine, int, int)"/>
      public LogicalMatrix(REngine engine, int rowCount, int columnCount)
         : base(engine, SymbolicExpressionType.LogicalVector, rowCount, columnCount)
      { }

      /// <summary>
      /// Creates a new LogicalMatrix with the specified values.
      /// </summary>
      /// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
      /// <param name="matrix">The values.</param>
      /// <seealso cref="REngineExtension.CreateLogicalMatrix(REngine, bool[,])"/>
      public LogicalMatrix(REngine engine, bool[,] matrix)
         : base(engine, SymbolicExpressionType.LogicalVector, matrix)
      { }

      /// <summary>
      /// Creates a new instance for a Boolean matrix.
      /// </summary>
      /// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
      /// <param name="coerced">The pointer to a Boolean matrix.</param>
      protected internal LogicalMatrix(REngine engine, IntPtr coerced)
         : base(engine, coerced)
      { }

      /// <summary>
      /// Gets or sets the element at the specified index.
      /// </summary>
      /// <param name="rowIndex">The zero-based rowIndex index of the element to get or set.</param>
      /// <param name="columnIndex">The zero-based columnIndex index of the element to get or set.</param>
      /// <returns>The element at the specified index.</returns>
      public override bool this[int rowIndex, int columnIndex]
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
               int data = Marshal.ReadInt32(DataPointer, offset);
               return Convert.ToBoolean(data);
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
               int data = Convert.ToInt32(value);
               Marshal.WriteInt32(DataPointer, offset, data);
            }
         }
      }

      protected override void InitMatrixFastDirect(bool[,] matrix)
      {
         var intValues = Utility.ArrayConvertAllOneDim(matrix, Convert.ToInt32);
         Marshal.Copy(intValues, 0, DataPointer, intValues.Length);
      }

      protected override bool[,] GetArrayFast()
      {
         int[] intValues = new int[this.ItemCount];
         Marshal.Copy(DataPointer, intValues, 0, intValues.Length);
         return Utility.ArrayConvertAllTwoDim(intValues, Convert.ToBoolean, this.RowCount, this.ColumnCount);
      }

      /// <summary>
      /// Gets the size of an integer in byte.
      /// </summary>
      protected override int DataSize
      {
         get { return sizeof(int); }
      }
   }
}
using RDotNet.Diagnostics;
using RDotNet.Dynamic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace RDotNet
{
   /// <summary>
   /// A data frame.
   /// </summary>
   [DebuggerDisplay(@"ColumnCount = {ColumnCount}; RowCount = {RowCount}; RObjectType = {Type}")]
   [DebuggerTypeProxy(typeof(DataFrameDebugView))]
   [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
   public class DataFrame : Vector<DynamicVector>
   {
      private const string RRowNamesSymbolName = "R_RowNamesSymbol";

      /// <summary>
      /// Creates a new instance.
      /// </summary>
      /// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
      /// <param name="coerced">The pointer to a data frame.</param>
      protected internal DataFrame(REngine engine, IntPtr coerced)
         : base(engine, coerced)
      { }

      /// <summary>
      /// Gets or sets the column at the specified index as a vector.
      /// </summary>
      /// <param name="columnIndex">The zero-based index of the column to get or set.</param>
      /// <returns>The column at the specified index.</returns>
      public override DynamicVector this[int columnIndex]
      {
         get
         {
            if (columnIndex < 0 || Length <= columnIndex)
            {
               throw new ArgumentOutOfRangeException();
            }
            using (new ProtectedPointer(this))
            {
               int offset = GetOffset(columnIndex);
               IntPtr pointer = Marshal.ReadIntPtr(DataPointer, offset);
               return new DynamicVector(Engine, pointer);
            }
         }
         set
         {
            if (columnIndex < 0 || Length <= columnIndex)
            {
               throw new ArgumentOutOfRangeException();
            }
            using (new ProtectedPointer(this))
            {
               SetColumn(columnIndex, value);
            }
         }
      }

      private void SetColumn(int columnIndex, DynamicVector value)
      {
         int offset = GetOffset(columnIndex);
         Marshal.WriteIntPtr(DataPointer, offset, (value ?? Engine.NilValue).DangerousGetHandle());
      }

      protected override void SetVectorDirect(DynamicVector[] values)
      {
         for (int i = 0; i < values.Length; i++)
            SetColumn(i, values[i]);
      }

      /// <summary>
      /// Gets or sets the element at the specified indexes.
      /// </summary>
      /// <param name="rowIndex">The row index.</param>
      /// <param name="columnIndex">The column index.</param>
      /// <returns>The element.</returns>
      public object this[int rowIndex, int columnIndex]
      {
         get
         {
            DynamicVector column = this[columnIndex];
            return column[rowIndex];
         }
         set
         {
            DynamicVector column = this[columnIndex];
            column[rowIndex] = value;
         }
      }

      /// <summary>
      /// Gets or sets the element at the specified index and name.
      /// </summary>
      /// <param name="rowIndex">The row index.</param>
      /// <param name="columnName">The column name.</param>
      /// <returns>The element.</returns>
      public object this[int rowIndex, string columnName]
      {
         get
         {
            DynamicVector column = this[columnName];
            return column[rowIndex];
         }
         set
         {
            DynamicVector column = this[columnName];
            column[rowIndex] = value;
         }
      }

      /// <summary>
      /// Gets or sets the element at the specified names.
      /// </summary>
      /// <param name="rowName">The row name.</param>
      /// <param name="columnName">The column name.</param>
      /// <returns>The element.</returns>
      public object this[string rowName, string columnName]
      {
         get
         {
            DynamicVector column = this[columnName];
            return column[rowName];
         }
         set
         {
            DynamicVector column = this[columnName];
            column[rowName] = value;
         }
      }

      /// <summary>
      /// Gets the number of data sets.
      /// </summary>
      public int RowCount
      {
         get { return ColumnCount == 0 ? 0 : this[0].Length; }
      }

      /// <summary>
      /// Gets the number of kinds of data.
      /// </summary>
      public int ColumnCount
      {
         get { return Length; }
      }

      /// <summary>
      /// Gets the names of rows.
      /// </summary>
      public string[] RowNames
      {
         get
         {
            SymbolicExpression rowNamesSymbol = Engine.GetPredefinedSymbol(RRowNamesSymbolName);
            SymbolicExpression rowNames = GetAttribute(rowNamesSymbol);
            if (rowNames == null)
            {
               return null;
            }
            CharacterVector rowNamesVector = rowNames.AsCharacter();
            if (rowNamesVector == null)
            {
               return null;
            }

            int length = rowNamesVector.Length;
            var result = new string[length];
            rowNamesVector.CopyTo(result, length);
            return result;
         }
      }

      /// <summary>
      /// Gets the names of columns.
      /// </summary>
      public string[] ColumnNames
      {
         get { return Names; }
      }

      protected override int DataSize
      {
         get { return Marshal.SizeOf(typeof(IntPtr)); }
      }

      /// <summary>
      /// Gets the row at the specified index.
      /// </summary>
      /// <param name="rowIndex">The index.</param>
      /// <returns>The row.</returns>
      public DataFrameRow GetRow(int rowIndex)
      {
         return new DataFrameRow(this, rowIndex);
      }

      /// <summary>
      /// Gets the row at the specified index mapping a specified class.
      /// </summary>
      /// <typeparam name="TRow">The row type with <see cref="DataFrameRowAttribute"/>.</typeparam>
      /// <returns>The row.</returns>
      public TRow GetRow<TRow>(int rowIndex)
         where TRow : class, new()
      {
         var rowType = typeof(TRow);
         var attribute = (DataFrameRowAttribute)rowType.GetCustomAttributes(typeof(DataFrameRowAttribute), false).Single();
         if (attribute == null)
         {
            throw new ArgumentException("DataFrameRowAttribute is required.");
         }
         var row = GetRow(rowIndex);
         return attribute.Convert<TRow>(row);
      }

      /// <summary>
      /// Enumerates all the rows in the data frame.
      /// </summary>
      /// <returns>The collection of the rows.</returns>
      public IEnumerable<DataFrameRow> GetRows()
      {
         int rowCount = RowCount;
         for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
         {
            yield return GetRow(rowIndex);
         }
      }

      /// <summary>
      /// Enumerates all the rows in the data frame mapping a specified class.
      /// </summary>
      /// <typeparam name="TRow">The row type with <see cref="DataFrameRowAttribute"/>.</typeparam>
      /// <returns>The collection of the rows.</returns>
      public IEnumerable<TRow> GetRows<TRow>()
         where TRow : class, new()
      {
         var rowType = typeof(TRow);
         var attribute = (DataFrameRowAttribute)rowType.GetCustomAttributes(typeof(DataFrameRowAttribute), false).Single();
         if (attribute == null)
         {
            throw new ArgumentException("DataFrameRowAttribute is required.");
         }
         int rowCount = RowCount;
         for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
         {
            var row = GetRow(rowIndex);
            yield return attribute.Convert<TRow>(row);
         }
      }

      public override DynamicMetaObject GetMetaObject(System.Linq.Expressions.Expression parameter)
      {
         return new DataFrameDynamicMeta(parameter, this);
      }
   }
}
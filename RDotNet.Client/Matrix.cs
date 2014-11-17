using System.Collections.Generic;
using RDotNet.Client.Diagnostics;
using System;
using System.Diagnostics;
using RDotNet.R.Adapter;

namespace RDotNet.Client
{
    [DebuggerDisplay("MatrixSize = {RowCount} x {ColumnCount}; RObjectType = {Type}")]
    [DebuggerTypeProxy(typeof (MatrixDebugView<>))]
    public abstract class Matrix<T> : SymbolicExpression, IMatrix<T>
    {
        private readonly SymbolicExpressionType _type;

        protected Matrix(int rowCount, int columnCount, SymbolicExpressionType type, IRObjectAPI api)
            : base(api.AllocateMatrix(type, rowCount, columnCount))
        {
            if (rowCount <= 0) throw new ArgumentOutOfRangeException("rowCount");
            if (columnCount <= 0) throw new ArgumentOutOfRangeException("columnCount");

            _type = type;
            ClearVector(type, GetItemCount());
        }

        protected Matrix(T[,] matrix, SymbolicExpressionType type, IRObjectAPI api)
            : this(matrix.GetLength(0), matrix.GetLength(1), type, api)
        {
            InitializeMatrix(type, matrix);
        }

        public T this[ int rowIndex, int columnIndex ]
        {
            get { return GetMatrixValueAt<T>( _type, rowIndex, columnIndex ); }
            set { SetMatrixValueAt( _type, rowIndex, columnIndex, value ); }
        }

        public T this[string rowName, string columnName]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(rowName)) throw new ArgumentException("rowName");
                if (string.IsNullOrWhiteSpace(columnName)) throw new ArgumentException("columnName");

                var dimensions = GetDimensions();
                var rowNames = dimensions[0].ToCharacterVector().ToList();
                var columnNames = dimensions[1].ToCharacterVector().ToList();

                int rowIndex = rowNames.IndexOf(rowName);
                int columnIndex = columnNames.IndexOf(columnName);
                return this[rowIndex, columnIndex];
            }
            set
            {
                if (string.IsNullOrWhiteSpace(rowName)) throw new ArgumentException("rowName");
                if (string.IsNullOrWhiteSpace(columnName)) throw new ArgumentException("columnName");

                var dimensions = GetDimensions();
                var rowNames = dimensions[0].ToCharacterVector().ToList();
                var columnNames = dimensions[1].ToCharacterVector().ToList();

                int rowIndex = rowNames.IndexOf(rowName);
                int columnIndex = columnNames.IndexOf(columnName);
                this[rowIndex, columnIndex] = value;
            }
        }

        public int GetRowCount()
        {
            return GetNumberOfRows();
        }

        public int GetColumnCount()
        {
            return GetNumberOfColumns();
        }

        public int GetItemCount()
        {
            return GetRowCount()*GetColumnCount();
        }

        public IList<string> GetRowNames()
        {
            var names = GetDimensions();
            if (names == null) return new List<string>();

            var rowNames = names[0];
            if (rowNames == null) return new List<string>();
            
            var result = rowNames.ToCharacterVector().ToList();
            return result;
        }

        public IList<string> GetColumnNames()
        {
            var names = GetDimensions();
            if (names == null) return new List<string>();

            var columnNames = names[1];
            if (columnNames == null) return new List<string>();

            var result = columnNames.ToCharacterVector().ToList();
            return result;
        }

        private GenericVector GetDimensions()
        {
            var dimnamesSymbol = GetDimNames();
            var dimnames = GetAttribute(dimnamesSymbol);
            if (dimnames == null) return null;

            return dimnames.ToGenericVector();
        }

        public T[,] ToArray()
        {
            return GetMatrixValues<T>(_type);
        }
    }
}

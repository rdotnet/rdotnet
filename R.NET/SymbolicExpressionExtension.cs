using System;
using System.Linq;
using System.Security.Permissions;
using RDotNet.Internals;

namespace RDotNet
{
	/// <summary>
	/// Provides extension methods for <see cref="SymbolicExpression"/>.
	/// </summary>
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	public static class SymbolicExpressionExtension
	{
		/// <summary>
		/// Gets whether the specified expression is list.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <returns><c>True</c> if the specified expression is list.</returns>
		public static bool IsList(this SymbolicExpression expression)
		{
			if (expression == null)
			{
				throw new ArgumentNullException();
			}
			return expression.Engine.Proxy.Rf_isList((IntPtr)expression);
		}

		/// <summary>
		/// Converts the specified expression to a GenericVector.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <returns>The GenericVector. Returns <c>null</c> if the specified expression is not vector.</returns>
		public static GenericVector AsList(this SymbolicExpression expression)
		{
			if (!expression.IsVector())
			{
				return null;
			}
			return new GenericVector(expression.Engine, (IntPtr)expression);
		}

		/// <summary>
		/// Converts the specified expression to a DataFrame.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <returns>The DataFrame. Returns <c>null</c> if the specified expression is not vector.</returns>
		public static DataFrame AsDataFrame(this SymbolicExpression expression)
		{
			if (!expression.IsVector())
			{
				return null;
			}
			return new DataFrame(expression.Engine, (IntPtr)expression);
		}

		/// <summary>
		/// Gets whether the specified expression is vector.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <returns><c>True</c> if the specified expression is vector.</returns>
		public static bool IsVector(this SymbolicExpression expression)
		{
			if (expression == null)
			{
				throw new ArgumentNullException();
			}
			return expression.Engine.Proxy.Rf_isVector((IntPtr)expression);
		}

		/// <summary>
		/// Converts the specified expression to a DynamicVector.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <returns>The DynamicVector. Returns <c>null</c> if the specified expression is not vector.</returns>
		public static DynamicVector AsVector(this SymbolicExpression expression)
		{
			if (expression == null)
			{
				throw new ArgumentNullException();
			}
			IntPtr coerced = expression.Engine.Proxy.Rf_coerceVector((IntPtr)expression, expression.Type);
			return new DynamicVector(expression.Engine, coerced);
		}

		/// <summary>
		/// Converts the specified expression to a LogicalVector.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <returns>The LogicalVector. Returns <c>null</c> if the specified expression is not vector.</returns>
		public static LogicalVector AsLogical(this SymbolicExpression expression)
		{
			if (!expression.IsVector())
			{
				return null;
			}
			IntPtr coerced = expression.Engine.Proxy.Rf_coerceVector((IntPtr)expression, SymbolicExpressionType.LogicalVector);
			return new LogicalVector(expression.Engine, coerced);
		}

		/// <summary>
		/// Converts the specified expression to an IntegerVector.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <returns>The LogicalVector. Returns <c>null</c> if the specified expression is not vector.</returns>
		public static IntegerVector AsInteger(this SymbolicExpression expression)
		{
			if (!expression.IsVector())
			{
				return null;
			}
			IntPtr coerced = expression.Engine.Proxy.Rf_coerceVector((IntPtr)expression, SymbolicExpressionType.IntegerVector);
			return new IntegerVector(expression.Engine, coerced);
		}

		/// <summary>
		/// Converts the specified expression to a NumericVector.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <returns>The LogicalVector. Returns <c>null</c> if the specified expression is not vector.</returns>
		public static NumericVector AsNumeric(this SymbolicExpression expression)
		{
			if (!expression.IsVector())
			{
				return null;
			}
			IntPtr coerced = expression.Engine.Proxy.Rf_coerceVector((IntPtr)expression, SymbolicExpressionType.NumericVector);
			return new NumericVector(expression.Engine, coerced);
		}

		/// <summary>
		/// Converts the specified expression to a CharacterVector.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <returns>The LogicalVector. Returns <c>null</c> if the specified expression is not vector.</returns>
		public static CharacterVector AsCharacter(this SymbolicExpression expression)
		{
			if (!expression.IsVector())
			{
				return null;
			}
			IntPtr coerced = expression.Engine.Proxy.Rf_coerceVector((IntPtr)expression, SymbolicExpressionType.CharacterVector);
			return new CharacterVector(expression.Engine, coerced);
		}

		/// <summary>
		/// Converts the specified expression to a ComplexVector.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <returns>The LogicalVector. Returns <c>null</c> if the specified expression is not vector.</returns>
		public static ComplexVector AsComplex(this SymbolicExpression expression)
		{
			if (!expression.IsVector())
			{
				return null;
			}
			IntPtr coerced = expression.Engine.Proxy.Rf_coerceVector((IntPtr)expression, SymbolicExpressionType.ComplexVector);
			return new ComplexVector(expression.Engine, coerced);
		}

		/// <summary>
		/// Converts the specified expression to a RawVector.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <returns>The LogicalVector. Returns <c>null</c> if the specified expression is not vector.</returns>
		public static RawVector AsRaw(this SymbolicExpression expression)
		{
			if (!expression.IsVector())
			{
				return null;
			}
			IntPtr coerced = expression.Engine.Proxy.Rf_coerceVector((IntPtr)expression, SymbolicExpressionType.RawVector);
			return new RawVector(expression.Engine, coerced);
		}

		/// <summary>
		/// Gets whether the specified expression is matrix.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <returns><c>True</c> if the specified expression is matrix.</returns>
		public static bool IsMatrix(this SymbolicExpression expression)
		{
			if (expression == null)
			{
				throw new ArgumentNullException();
			}
			return expression.Engine.Proxy.Rf_isMatrix((IntPtr)expression);
		}

		/// <summary>
		/// Converts the specified expression to a LogicalMatrix.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <returns>The LogicalMatrix. Returns <c>null</c> if the specified expression is not vector.</returns>
		public static LogicalMatrix AsLogicalMatrix(this SymbolicExpression expression)
		{
			if (!expression.IsVector())
			{
				return null;
			}

			int rowCount = 0;
			int columnCount = 0;

			if (expression.IsMatrix())
			{
				if (expression.Type == SymbolicExpressionType.LogicalVector)
				{
					return new LogicalMatrix(expression.Engine, (IntPtr)expression);
				}
				else
				{
					rowCount = expression.Engine.Proxy.Rf_nrows((IntPtr)expression);
					columnCount = expression.Engine.Proxy.Rf_ncols((IntPtr)expression);
				}
			}

			if (columnCount == 0)
			{
				rowCount = expression.Engine.Proxy.Rf_length((IntPtr)expression);
				columnCount = 1;
			}

			IntPtr coerced = expression.Engine.Proxy.Rf_coerceVector((IntPtr)expression, SymbolicExpressionType.LogicalVector);
			IntegerVector dim = new IntegerVector(expression.Engine, new int[] { rowCount, columnCount });
			SymbolicExpression dimSymbol = expression.Engine.GetPredefinedSymbol(NativeMethods.RDimSymbolName);
			var matrix = new LogicalMatrix(expression.Engine, coerced);
			matrix.SetAttribute(dimSymbol, dim);
			return matrix;
		}

		/// <summary>
		/// Converts the specified expression to an IntegerMatrix.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <returns>The IntegerMatrix. Returns <c>null</c> if the specified expression is not vector.</returns>
		public static IntegerMatrix AsIntegerMatrix(this SymbolicExpression expression)
		{
			if (!expression.IsVector())
			{
				return null;
			}

			int rowCount = 0;
			int columnCount = 0;

			if (expression.IsMatrix())
			{
				if (expression.Type == SymbolicExpressionType.IntegerVector)
				{
					return new IntegerMatrix(expression.Engine, (IntPtr)expression);
				}
				else
				{
					rowCount = expression.Engine.Proxy.Rf_nrows((IntPtr)expression);
					columnCount = expression.Engine.Proxy.Rf_ncols((IntPtr)expression);
				}
			}

			if (columnCount == 0)
			{
				rowCount = expression.Engine.Proxy.Rf_length((IntPtr)expression);
				columnCount = 1;
			}

			IntPtr coerced = expression.Engine.Proxy.Rf_coerceVector((IntPtr)expression, SymbolicExpressionType.IntegerVector);
			IntegerVector dim = new IntegerVector(expression.Engine, new int[] { rowCount, columnCount });
			SymbolicExpression dimSymbol = expression.Engine.GetPredefinedSymbol(NativeMethods.RDimSymbolName);
			var matrix = new IntegerMatrix(expression.Engine, coerced);
			matrix.SetAttribute(dimSymbol, dim);
			return matrix;
		}

		/// <summary>
		/// Converts the specified expression to a NumericMatrix.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <returns>The NumericMatrix. Returns <c>null</c> if the specified expression is not vector.</returns>
		public static NumericMatrix AsNumericMatrix(this SymbolicExpression expression)
		{
			if (!expression.IsVector())
			{
				return null;
			}

			int rowCount = 0;
			int columnCount = 0;

			if (expression.IsMatrix())
			{
				if (expression.Type == SymbolicExpressionType.NumericVector)
				{
					return new NumericMatrix(expression.Engine, (IntPtr)expression);
				}
				else
				{
					rowCount = expression.Engine.Proxy.Rf_nrows((IntPtr)expression);
					columnCount = expression.Engine.Proxy.Rf_ncols((IntPtr)expression);
				}
			}

			if (columnCount == 0)
			{
				rowCount = expression.Engine.Proxy.Rf_length((IntPtr)expression);
				columnCount = 1;
			}

			IntPtr coerced = expression.Engine.Proxy.Rf_coerceVector((IntPtr)expression, SymbolicExpressionType.NumericVector);
			IntegerVector dim = new IntegerVector(expression.Engine, new int[] { rowCount, columnCount });
			SymbolicExpression dimSymbol = expression.Engine.GetPredefinedSymbol(NativeMethods.RDimSymbolName);
			var matrix = new NumericMatrix(expression.Engine, coerced);
			matrix.SetAttribute(dimSymbol, dim);
			return matrix;
		}

		/// <summary>
		/// Converts the specified expression to a CharacterMatrix.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <returns>The CharacterMatrix. Returns <c>null</c> if the specified expression is not vector.</returns>
		public static CharacterMatrix AsCharacterMatrix(this SymbolicExpression expression)
		{
			if (!expression.IsVector())
			{
				return null;
			}

			int rowCount = 0;
			int columnCount = 0;

			if (expression.IsMatrix())
			{
				if (expression.Type == SymbolicExpressionType.CharacterVector)
				{
					return new CharacterMatrix(expression.Engine, (IntPtr)expression);
				}
				else
				{
					rowCount = expression.Engine.Proxy.Rf_nrows((IntPtr)expression);
					columnCount = expression.Engine.Proxy.Rf_ncols((IntPtr)expression);
				}
			}

			if (columnCount == 0)
			{
				rowCount = expression.Engine.Proxy.Rf_length((IntPtr)expression);
				columnCount = 1;
			}

			IntPtr coerced = expression.Engine.Proxy.Rf_coerceVector((IntPtr)expression, SymbolicExpressionType.CharacterVector);
			IntegerVector dim = new IntegerVector(expression.Engine, new int[] { rowCount, columnCount });
			SymbolicExpression dimSymbol = expression.Engine.GetPredefinedSymbol(NativeMethods.RDimSymbolName);
			var matrix = new CharacterMatrix(expression.Engine, coerced);
			matrix.SetAttribute(dimSymbol, dim);
			return matrix;
		}

		/// <summary>
		/// Converts the specified expression to a ComplexMatrix.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <returns>The ComplexMatrix. Returns <c>null</c> if the specified expression is not vector.</returns>
		public static ComplexMatrix AsComplexMatrix(this SymbolicExpression expression)
		{
			if (!expression.IsVector())
			{
				return null;
			}

			int rowCount = 0;
			int columnCount = 0;

			if (expression.IsMatrix())
			{
				if (expression.Type == SymbolicExpressionType.ComplexVector)
				{
					return new ComplexMatrix(expression.Engine, (IntPtr)expression);
				}
				else
				{
					rowCount = expression.Engine.Proxy.Rf_nrows((IntPtr)expression);
					columnCount = expression.Engine.Proxy.Rf_ncols((IntPtr)expression);
				}
			}

			if (columnCount == 0)
			{
				rowCount = expression.Engine.Proxy.Rf_length((IntPtr)expression);
				columnCount = 1;
			}

			IntPtr coerced = expression.Engine.Proxy.Rf_coerceVector((IntPtr)expression, SymbolicExpressionType.ComplexVector);
			IntegerVector dim = new IntegerVector(expression.Engine, new int[] { rowCount, columnCount });
			SymbolicExpression dimSymbol = expression.Engine.GetPredefinedSymbol(NativeMethods.RDimSymbolName);
			var matrix = new ComplexMatrix(expression.Engine, coerced);
			matrix.SetAttribute(dimSymbol, dim);
			return matrix;
		}

		/// <summary>
		/// Converts the specified expression to a RawMatrix.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <returns>The RawMatrix. Returns <c>null</c> if the specified expression is not vector.</returns>
		public static RawMatrix AsRawMatrix(this SymbolicExpression expression)
		{
			if (!expression.IsVector())
			{
				return null;
			}

			int rowCount = 0;
			int columnCount = 0;

			if (expression.IsMatrix())
			{
				if (expression.Type == SymbolicExpressionType.RawVector)
				{
					return new RawMatrix(expression.Engine, (IntPtr)expression);
				}
				else
				{
					rowCount = expression.Engine.Proxy.Rf_nrows((IntPtr)expression);
					columnCount = expression.Engine.Proxy.Rf_ncols((IntPtr)expression);
				}
			}

			if (columnCount == 0)
			{
				rowCount = expression.Engine.Proxy.Rf_length((IntPtr)expression);
				columnCount = 1;
			}

			IntPtr coerced = expression.Engine.Proxy.Rf_coerceVector((IntPtr)expression, SymbolicExpressionType.RawVector);
			IntegerVector dim = new IntegerVector(expression.Engine, new int[] { rowCount, columnCount });
			SymbolicExpression dimSymbol = expression.Engine.GetPredefinedSymbol(NativeMethods.RDimSymbolName);
			var matrix = new RawMatrix(expression.Engine, coerced);
			matrix.SetAttribute(dimSymbol, dim);
			return matrix;
		}
	}
}

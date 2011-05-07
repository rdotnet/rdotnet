using System;
using System.Runtime.Serialization;
using RDotNet.Internals;

namespace RDotNet
{
	/// <summary>
	/// Thrown when an engine comes to an error.
	/// </summary>
	[Serializable]
	public class ParseException : ApplicationException
	{
		private const string StatusFieldName = "status";
		private readonly ParseStatus status;
		/// <summary>
		/// The error.
		/// </summary>
		public ParseStatus Status
		{
			get
			{
				return status;
			}
		}

		private const string ErrorStatementFieldName = "errorStatement";
		private readonly string errorStatement;
		/// <summary>
		/// The statement caused the error.
		/// </summary>
		public string ErrorStatement
		{
			get
			{
				return errorStatement;
			}
		}

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public ParseException()
			: base()
		{
			// This does not internally occur. See Parse.h in R_HOME/include/R_ext/Parse.h
			this.status = ParseStatus.Null;
			this.errorStatement = null;
		}

		/// <summary>
		/// Creates a new instance with the specified error.
		/// </summary>
		/// <param name="status">The error.</param>
		/// <param name="errorStatement">The error statement.</param>
		public ParseException(ParseStatus status, string errorStatement)
			: base()
		{
			this.status = status;
			this.errorStatement = errorStatement;
		}

		protected ParseException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.status = (ParseStatus)info.GetValue(StatusFieldName, typeof(ParseStatus));
			this.errorStatement = info.GetString(ErrorStatementFieldName);
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(StatusFieldName, status);
			info.AddValue(ErrorStatementFieldName, errorStatement);
		}
	}
}

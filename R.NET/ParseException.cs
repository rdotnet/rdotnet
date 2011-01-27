using System;
using System.Runtime.Serialization;
using RDotNet.Internals;

namespace RDotNet
{
	public class ParseException : ApplicationException
	{
		private const string StatusFieldName = "status";
		private readonly ParseStatus status;
		public ParseStatus Status
		{
			get
			{
				return status;
			}
		}

		private const string ErrorStatementFieldName = "errorStatement";
		private readonly string errorStatement;
		public string ErrorStatement
		{
			get
			{
				return errorStatement;
			}
		}


		public ParseException()
			: base()
		{
			// This does not internally occur. See Parse.h in R_HOME/include/R_ext/Parse.h
			this.status = ParseStatus.Null;
			this.errorStatement = null;
		}

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

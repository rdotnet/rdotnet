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

		public ParseException()
			: base()
		{
			// This does not internally occur. See Parse.h in R_HOME/include/R_ext/Parse.h
			this.status = ParseStatus.Null;
		}

		public ParseException(ParseStatus status)
		{
			this.status = status;
		}

		protected ParseException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.status = (ParseStatus)info.GetValue(StatusFieldName, typeof(ParseStatus));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(StatusFieldName, status);
		}
	}
}

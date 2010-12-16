using System;

namespace RDotNet.Internals
{
	public enum ParseStatus
	{
		Null,
		OK,
		Incomplete,
		Error,
		EOF,

		#region Original Definitions
		[Obsolete("Use ParseStatus.Null instead.")]
		PARSE_NULL = Null,
		[Obsolete("Use ParseStatus.OK instead.")]
		PARSE_OK = OK,
		[Obsolete("Use ParseStatus.Incomplete instead.")]
		PARSE_INCOMPLETE = Incomplete,
		[Obsolete("Use ParseStatus.Error instead.")]
		PARSE_ERROR = Error,
		[Obsolete("Use ParseStatus.EOF instead.")]
		PARSE_EOF = EOF,
		#endregion
	}
}

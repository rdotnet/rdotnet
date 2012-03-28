using System;

namespace RDotNet.Internals
{
	/// <summary>
	/// Specifies output mode.
	/// </summary>
	[Flags]
	public enum OutputMode
	{
		/// <summary>
		/// No option.
		/// </summary>
		None = 0x0,

		/// <summary>
		/// Quiet mode.
		/// </summary>
		Quiet = 0x1,

		/// <summary>
		/// Slave mode.
		/// </summary>
		Slave = 0x2,

		/// <summary>
		/// Verbose mode.
		/// </summary>
		Verbose = 0x4,
	}
}

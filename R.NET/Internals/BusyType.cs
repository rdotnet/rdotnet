#if WINDOWS
namespace RDotNet.Internals
{
	/// <summary>
	/// Type of R's working.
	/// </summary>
	public enum BusyType
	{
		/// <summary>
		/// Terminated states of business.
		/// </summary>
		None = 0,
		/// <summary>
		/// Embarks on an extended computation
		/// </summary>
		ExtendedComputation = 1,
	}
}
#endif

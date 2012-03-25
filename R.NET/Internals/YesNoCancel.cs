namespace RDotNet.Internals
{
	/// <summary>
	/// User's decision.
	/// </summary>
	public enum YesNoCancel
	{
		/// <summary>
		/// User agreed.
		/// </summary>
		Yes = 1,

		/// <summary>
		/// User disagreed.
		/// </summary>
		No = -1,

		/// <summary>
		/// User abandoned to answer.
		/// </summary>
		Cancel = 0,
	}
}

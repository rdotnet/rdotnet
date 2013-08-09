namespace RDotNet.Internals
{
	/// <summary>
	/// Specifies the restore action.
	/// </summary>
	public enum StartupRestoreAction
	{
		/// <summary>
		/// Not restoring.
		/// </summary>
		NoRestore = 0,

		/// <summary>
		/// Restoring.
		/// </summary>
		Restore = 1,

		/// <summary>
		/// The default value.
		/// </summary>
		Default = 2,
	}

	/// <summary>
	/// Specifies the save action.
	/// </summary>
	public enum StartupSaveAction
	{
		/// <summary>
		/// The default value.
		/// </summary>
		Default = 2,

		/// <summary>
		/// No saving.
		/// </summary>
		NoSave = 3,

		/// <summary>
		/// Saving.
		/// </summary>
		Save = 4,

		/// <summary>
		/// Asking user.
		/// </summary>
		Ask = 5,

		/// <summary>
		/// Terminates without any actions.
		/// </summary>
		Suicide = 6,
	}
}

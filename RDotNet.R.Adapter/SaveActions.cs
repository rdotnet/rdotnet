namespace RDotNet.R.Adapter
{
   public enum StartupRestoreAction
   {
      NoRestore = 0,
      Restore = 1,
      Default = 2,
   }

   public enum StartupSaveAction
   {
      Default = 2,
      NoSave = 3,
      Save = 4,
      Ask = 5,
      Suicide = 6,
   }
}

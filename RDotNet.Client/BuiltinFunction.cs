namespace RDotNet.Client
{
    public class BuiltinFunction : Function, IBuiltinFunction
    {
      public BuiltinFunction(IRSafeHandle handle)
         : base(handle)
      { }
    }
}

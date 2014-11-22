namespace RDotNet.Client
{
   public class SpecialFunction : Function, ISpecialFunction
   {
      public SpecialFunction(IRSafeHandle handle)
         : base(handle)
      { }
   }
}

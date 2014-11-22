using RDotNet.R.Adapter;

namespace RDotNet.Client
{
   public class DynamicVector : Vector<object>, IDynamicVector
   {
      public DynamicVector(IRSafeHandle handle)
         : base(handle, SymbolicExpressionType.Any)
      { }

       public Factor ToFactor()
       {
           return new Factor(Handle);
       }
   }
}

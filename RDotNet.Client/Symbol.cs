namespace RDotNet.Client
{
   public class Symbol : SymbolicExpression
   {
       public Symbol(IRSafeHandle handle)
            : base(handle)
       { }

       public string PrintName
       {
           get { return GetPrintName(); }
           set { SetPrintName(value); }
       }

       public new SymbolicExpression GetInternal()
       {
           return base.GetInternal();
       }

       public new SymbolicExpression GetValue()
       {
           return base.GetValue();
       }
   }
}

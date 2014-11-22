namespace RDotNet.Client
{
   public class Language : SymbolicExpression
   {
       public Language(IRSafeHandle handle)
           : base(handle)
       { }

       public PairList GetFunctionCall()
       {
           return GetLanguageList();
       }
   }
}

using System.Diagnostics;

namespace RDotNet.Diagnostics
{
   [DebuggerDisplay("{Value,nq}")]
   internal class NotQuotedStringDisplay
   {
      [DebuggerBrowsable(DebuggerBrowsableState.Never)]
      internal string Value { get; set; }
   }
}

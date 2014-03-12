using RDotNet.Internals;
using System;
using System.Runtime.Serialization;

namespace RDotNet
{
   /// <summary>
   /// Thrown when an engine comes to an error.
   /// </summary>
   [Serializable]
   public class ParseException : Exception
   // (http://msdn.microsoft.com/en-us/library/vstudio/system.applicationexception%28v=vs.110%29.aspx) 
   // "If you are designing an application that needs to create its own exceptions, 
   // you are advised to derive custom exceptions from the Exception class"
   {
      private const string StatusFieldName = "status";

      private const string ErrorStatementFieldName = "errorStatement";
      private readonly string errorStatement;
      private readonly ParseStatus status;

      /// <summary>
      /// Creates a new instance.
      /// </summary>
      private ParseException()
         : this(ParseStatus.Null, "", "") 
         // This does not internally occur. See Parse.h in R_HOME/include/R_ext/Parse.h
      { }

      /// <summary>
      /// Creates a new instance with the specified error.
      /// </summary>
      /// <param name="status">The error status</param>
      /// <param name="errorStatement">The statement that failed to be parsed</param>
      /// <param name="errorMsg">The error message given by the native R engine</param>
      public ParseException(ParseStatus status, string errorStatement, string errorMsg)
         : base(MakeErrorMsg(status, errorStatement, errorMsg))
      {
         this.status = status;
         this.errorStatement = errorStatement;
      }

      private static string MakeErrorMsg(ParseStatus status, string errorStatement, string errorMsg)
      {
         return string.Format("Status {2} for {0} : {1}", errorStatement, errorMsg, status);
      }

      protected ParseException(SerializationInfo info, StreamingContext context)
         : base(info, context)
      {
         this.status = (ParseStatus)info.GetValue(StatusFieldName, typeof(ParseStatus));
         this.errorStatement = info.GetString(ErrorStatementFieldName);
      }

      /// <summary>
      /// The error.
      /// </summary>
      public ParseStatus Status
      {
         get { return this.status; }
      }

      /// <summary>
      /// The statement caused the error.
      /// </summary>
      public string ErrorStatement
      {
         get { return this.errorStatement; }
      }

      public override void GetObjectData(SerializationInfo info, StreamingContext context)
      {
         base.GetObjectData(info, context);
         info.AddValue(StatusFieldName, this.status);
         info.AddValue(ErrorStatementFieldName, this.errorStatement);
      }
   }
}
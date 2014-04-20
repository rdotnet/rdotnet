using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace RDotNet
{
   /// <summary>
   /// A vector of S expressions
   /// </summary>
   [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
   public class ExpressionVector : Vector<Expression>
   {
      /// <summary>
      /// Creates a new instance for an expression vector.
      /// </summary>
      /// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
      /// <param name="coerced">The pointer to an expression vector.</param>
      internal ExpressionVector(REngine engine, IntPtr coerced)
         : base(engine, coerced)
      { }

      /// <summary>
      /// Gets/sets the expression for an index
      /// </summary>
      /// <param name="index">index value</param>
      /// <returns>The Expression at a given index.</returns>
      public override Expression this[int index]
      {
         get
         {
            if (index < 0 || Length <= index)
            {
               throw new ArgumentOutOfRangeException();
            }
            using (new ProtectedPointer(this))
            {
               return GetValue(index);
            }
         }
         set
         {
            if (index < 0 || Length <= index)
            {
               throw new ArgumentOutOfRangeException();
            }
            using (new ProtectedPointer(this))
            {
               SetValue(index, value);
            }
         }
      }

      /// <summary>
      /// Gets an array representation of a vector of SEXP in R. Note that the implementation cannot be particularly "fast" in spite of the name.
      /// </summary>
      /// <returns></returns>
      protected override Expression[] GetArrayFast()
      {
         var res = new Expression[this.Length];
         for (int i = 0; i < res.Length; i++)
            res[i] = GetValue(i);
         return res;
      }

      private Expression GetValue(int index)
      {
         int offset = GetOffset(index);
         IntPtr pointer = Marshal.ReadIntPtr(DataPointer, offset);
         return new Expression(Engine, pointer);
      }

      private void SetValue(int index, Expression value)
      {
         int offset = GetOffset(index);
         Marshal.WriteIntPtr(DataPointer, offset, (value ?? Engine.NilValue).DangerousGetHandle());
      }

      /// <summary>
      /// Efficient initialisation of R vector values from an array representation in the CLR
      /// </summary>
      protected override void SetVectorDirect(Expression[] values)
      {
         for (int i = 0; i < values.Length; i++)
            SetValue(i, values[i]);
      }

      /// <summary>
      /// Gets the size of a pointer in byte.
      /// </summary>
      protected override int DataSize
      {
         get { return Marshal.SizeOf(typeof(IntPtr)); }
      }
   }
}
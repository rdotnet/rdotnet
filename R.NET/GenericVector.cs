using RDotNet.Dynamic;
using RDotNet.Internals;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace RDotNet
{
   /// <summary>
   /// A generic list. This is also known as list in R.
   /// </summary>
   [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
   public class GenericVector : Vector<SymbolicExpression>
   {
      /// <summary>
      /// Creates a new empty GenericVector with the specified length.
      /// </summary>
      /// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
      /// <param name="length">The length.</param>
      public GenericVector(REngine engine, int length)
         : base(engine, engine.GetFunction<Rf_allocVector>()(SymbolicExpressionType.List, length))
      { }

      /// <summary>
      /// Creates a new GenericVector with the specified values.
      /// </summary>
      /// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
      /// <param name="list">The values.</param>
      public GenericVector(REngine engine, IEnumerable<SymbolicExpression> list)
         : base(engine, SymbolicExpressionType.List, list)
      { }

      /// <summary>
      /// Creates a new instance for a list.
      /// </summary>
      /// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
      /// <param name="coerced">The pointer to a list.</param>
      protected internal GenericVector(REngine engine, IntPtr coerced)
         : base(engine, coerced)
      { }

      /// <summary>
      /// Gets or sets the element at the specified index.
      /// </summary>
      /// <param name="index">The zero-based index of the element to get or set.</param>
      /// <returns>The element at the specified index.</returns>
      public override SymbolicExpression this[int index]
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

      private SymbolicExpression GetValue(int index)
      {
         int offset = GetOffset(index);
         IntPtr pointer = Marshal.ReadIntPtr(DataPointer, offset);
         return new SymbolicExpression(Engine, pointer);
      }

      private void SetValue(int index, SymbolicExpression value)
      {
         int offset = GetOffset(index);
         Marshal.WriteIntPtr(DataPointer, offset, (value ?? Engine.NilValue).DangerousGetHandle());
      }

      /// <summary>
      /// Efficient conversion from R vector representation to the array equivalent in the CLR
      /// </summary>
      /// <returns>Array equivalent</returns>
      protected override SymbolicExpression[] GetArrayFast()
      {
         var res = new SymbolicExpression[this.Length];
         for (int i = 0; i < res.Length; i++)
            res[i] = GetValue(i);
         return res;
      }

      /// <summary>
      /// Efficient initialisation of R vector values from an array representation in the CLR
      /// </summary>
      protected override void SetVectorDirect(SymbolicExpression[] values)
      {
         for (int i = 0; i < values.Length; i++)
            SetValue(i, values[i]);
      }

      /// <summary>
      /// Gets the size of each item in this vector
      /// </summary>
      protected override int DataSize
      {
         get { return Marshal.SizeOf(typeof(IntPtr)); }
      }

      /// <summary>
      /// Converts into a <see cref="RDotNet.Pairlist"/>.
      /// </summary>
      /// <returns>The pairlist.</returns>
      public Pairlist ToPairlist()
      {
         return new Pairlist(Engine, this.GetFunction<Rf_VectorToPairList>()(handle));
      }

      /// <summary>
      /// returns a new ListDynamicMeta for this Generic Vector
      /// </summary>
      /// <param name="parameter"></param>
      /// <returns></returns>
      public override DynamicMetaObject GetMetaObject(System.Linq.Expressions.Expression parameter)
      {
         return new ListDynamicMeta(parameter, this);
      }
   }
}
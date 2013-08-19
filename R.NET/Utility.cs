using System;

namespace RDotNet
{
   internal static class Utility
   {
      public static T[] AddFirst<T>(T value, T[] array)
      {
         if (array == null)
         {
            return new[] { value };
         }
         var newArray = new T[array.Length + 1];
         newArray[0] = value;
         Array.Copy(array, 0, newArray, 1, array.Length);
         return newArray;
      }

      internal static bool CheckNil(this REngine engine, IntPtr pointer)
      {
         return engine.NilValue.DangerousGetHandle() == pointer;
      }

      internal static bool CheckUnbound(this REngine engine, IntPtr pointer)
      {
         return engine.UnboundValue.DangerousGetHandle() == pointer;
      }
   }
}
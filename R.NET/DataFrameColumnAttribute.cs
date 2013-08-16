using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDotNet
{
   /// <summary>
   /// Represents a column of certain data frames.
   /// </summary>
   [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
   public class DataFrameColumnAttribute : Attribute
   {
      private static readonly string[] Empty = new string[0];

      private readonly int index;
      /// <summary>
      /// Gets the index.
      /// </summary>
      public int Index
      {
         get { return this.index; }
      }

      private string name;
      /// <summary>
      /// Gets or sets the name.
      /// </summary>
      public string Name
      {
         get { return this.name; }
         set
         {
            if (this.index < 0 && value == null)
            {
               throw new ArgumentNullException("value", "Name must not be null when Index is not defined.");
            }
            this.name = value;
         }
      }

      /// <summary>
      /// Initializes a new instance by name.
      /// </summary>
      /// <param name="name">The name.</param>
      public DataFrameColumnAttribute(string name)
      {
         if (name == null)
         {
            throw new ArgumentNullException("name");
         }
         this.name = name;
         this.index = -1;
      }

      /// <summary>
      /// Initializes a new instance by index.
      /// </summary>
      /// <param name="name">The index.</param>
      public DataFrameColumnAttribute(int index)
      {
         if (index < 0)
         {
            throw new ArgumentOutOfRangeException("index");
         }
         this.name = null;
         this.index = index;
      }

      internal int GetIndex(string[] names)
      {
         return Index >= 0 ? Index : Array.IndexOf(names ?? Empty, Name);
      }
   }
}

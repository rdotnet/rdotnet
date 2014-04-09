using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RDotNet
{
   internal delegate void Map(DataFrameRow from, object to);

   /// <summary>
   /// Indicates the class with the attribute represents rows of certain data frames.
   /// </summary>
   [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
   public class DataFrameRowAttribute : Attribute
   {
      private readonly Dictionary<Type, Map> cache;

      /// <summary>
      /// Initializes a new instance.
      /// </summary>
      public DataFrameRowAttribute()
      {
         this.cache = new Dictionary<Type, Map>();
      }

      internal TRow Convert<TRow>(DataFrameRow row)
         where TRow : class, new()
      {
         var rowType = typeof(TRow);
         Map map;
         if (!this.cache.TryGetValue(rowType, out map))
         {
            map = CreateMap(rowType);
            this.cache.Add(rowType, map);
         }
         var result = Activator.CreateInstance(rowType);
         map(row, result);
         return (TRow)result;
      }

      private static Map CreateMap(Type rowType)
      {
         var tuples = (from property in rowType.GetProperties()
                       let attribute = (DataFrameColumnAttribute)property.GetCustomAttributes(typeof(DataFrameColumnAttribute), true).SingleOrDefault()
                       where attribute != null
                       select Tuple.Create(attribute, property.GetSetMethod())).ToArray();
         return (from, to) => Map(from, to, tuples);
      }

      private static void Map(DataFrameRow from, object to, Tuple<DataFrameColumnAttribute, MethodInfo>[] tuples)
      {
         var names = from.DataFrame.ColumnNames;
         foreach (var t in tuples)
         {
            var attribute = t.Item1;
            var setter = t.Item2;
            var index = attribute.GetIndex(names);
            setter.Invoke(to, new object[] { from.GetInnerValue(index) });
         }
      }
   }
}
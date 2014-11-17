using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RDotNet.Client
{
   internal delegate void Map(DataFrameRow from, object to);

   [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
   public class DataFrameRowAttribute : Attribute
   {
      private readonly Dictionary<Type, Map> _cache = new Dictionary<Type, Map>();

      internal TRow Convert<TRow>(DataFrameRow row)
         where TRow : class, new()
      {
         var rowType = typeof(TRow);
         Map map;
         if (!_cache.TryGetValue(rowType, out map))
         {
            map = CreateMap(rowType);
            _cache.Add(rowType, map);
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

      private static void Map(DataFrameRow from, object to, IEnumerable<Tuple<DataFrameColumnAttribute, MethodInfo>> tuples)
      {
         var names = from.DataFrame.GetColumnNames().ToArray();
         foreach (var t in tuples)
         {
            var attribute = t.Item1;
            var setter = t.Item2;
            var index = attribute.GetIndex(names);
            setter.Invoke(to, new[] { from.GetInnerValue(index) });
         }
      }
   }
}

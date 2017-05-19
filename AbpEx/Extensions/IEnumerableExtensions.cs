using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Application.Services;

namespace Abp.Extensions
{
    public static class IEnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> elements, Action<T> action)
        {
            foreach (var element in elements)
            {
                action(element);
            }
        }

        public static IReadOnlyList<T> ToDropdownTree<T>(this IEnumerable<T> source, string defaultText = "Please select one") where T : class, INode<T>
        {
            var selectList = new List<T>();
            //add a default item ; eg : <option value="">Please select one</option>
            if (!string.IsNullOrEmpty(defaultText))
            {
                var constructorInfo = typeof(T).GetConstructor(Type.EmptyTypes);
                if (constructorInfo != null)
                {
                    var t = (T)constructorInfo.Invoke(null);
                    t.GetType().GetProperty("Id").SetValue(t, null);
                    t.GetType().GetProperty("Name").SetValue(t, defaultText);
                    selectList.Add(t);
                }
            }
            
            
            foreach (var item in source.Where(x => !x.ParentId.HasValue))
            {
                selectList.Add(item);
                AddChildNode(source, selectList, item.Id);
            }

            return selectList;
        }

        private static void AddChildNode<T>(IEnumerable<T> source, ICollection<T> list, int? id, int deep = 1, string padding = "") where T : class, INode<T>
        {
            var items = source.Where(x => x.ParentId == id);
            foreach (var item in items)
            {
                padding = "";
                for (var j = 0; j < deep; j++)
                {
                    padding += "　";
                }
                item.Name = padding + "|--" + item.Name;
                list.Add(item);
                AddChildNode(source, list, item.Id, deep + 1, padding);
            }
        }
    }
}

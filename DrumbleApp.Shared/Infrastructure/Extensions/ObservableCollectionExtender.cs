using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DrumbleApp.Shared.Infrastructure.Extensions
{
    public static class ObservableCollectionExtender
    {
        public static void AddRange<TSource>(this ObservableCollection<TSource> source, IEnumerable<TSource> items)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            if (source == null)
                throw new ArgumentNullException("source");

            foreach (var item in items)
            {
                source.Add(item);
            }
        }
    }
}

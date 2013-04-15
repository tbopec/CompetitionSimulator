using System;
using System.Collections.Generic;
using System.Linq;

namespace RoboCoP.Helpers
{
    public static class ObservableExtensions
    {
        public static IObservable<TSource> RepeatWhile<TSource>(this IObservable<TSource> source,
                                                                Func<bool> condition)
        {
            if(source == null)
                throw new ArgumentNullException("source");
            if(condition == null)
                throw new ArgumentNullException("condition");

            return ProduceWhile(source, condition).Concat();
        }

        private static IEnumerable<IObservable<TSource>> ProduceWhile<TSource>(IObservable<TSource> source, Func<bool> condition)
        {
            do {
                yield return source;
            } while(condition());
        }
    }
}
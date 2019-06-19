using System;
using System.Collections.Generic;
using System.Linq;

//Copied from code posted by Dave Jellison at:
//http://www.experts-exchange.com/Programming/Languages/.NET/LINQ/A_3007-A-Custom-Recursive-LINQ-Extension.html

namespace NPS.AKRO.ThemeManager.Extensions
{
    public static class LinqExtensions
    {
        public static IEnumerable<T> Recurse<T>
            (
                this T root,
                Func<T, IEnumerable<T>> findChildren
            )
            where T : class
        {
            yield return root;

            foreach (var child in
                findChildren(root)
                    .SelectMany(node => Recurse(node, findChildren))
                    .TakeWhile(child => child != null))
            {
                yield return child;
            }
        }

        // ReSharper disable once UnusedMember.Global
        public static IEnumerable<T> WithProgressReporting<T>(this  IEnumerable<T> sequence, Action<int> reportProgress)
        {
            if (sequence == null) { throw new ArgumentNullException(nameof(sequence)); }
            //Counting the sequence may require enumerating the sequence, which may take as long as the
            //action we are hoping to monitor the progress of.  Oh well.
            // ReSharper disable PossibleMultipleEnumeration
            return sequence.WithProgressReporting(sequence.Count(), reportProgress);
            // ReSharper restore PossibleMultipleEnumeration
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static IEnumerable<T> WithProgressReporting<T>(this IEnumerable<T> sequence, long itemCount, Action<int> reportProgress)
        {
            if (sequence == null) { throw new ArgumentNullException(nameof(sequence)); }

            int completed = 0;
            foreach (var item in sequence)
            {
                yield return item;

                completed++;
                reportProgress((int)(((double)completed / itemCount) * 100));
            }
        }

    }
}

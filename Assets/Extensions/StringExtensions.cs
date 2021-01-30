using System.Collections.Generic;
using System.Linq;

namespace StreamBallUltimate.Assets.Extensions
{
    public static class StringExtensions
    {
        public static IEnumerable<string> SplitToLines(this string stringToSplit, int maximumLineLength)
        {
            var words = stringToSplit.Split(' ').Concat(new[] { "" }).ToList();
            return
                words
                    .Skip(1)
                    .Aggregate(
                        words.Take(1).ToList(),
                        (a, w) =>
                        {
                            var last = a.Last();
                            while (last.Length > maximumLineLength)
                            {
                                a[a.Count - 1] = last.Substring(0, maximumLineLength);
                                last = last.Substring(maximumLineLength);
                                a.Add(last);
                            }
                            var test = last + " " + w;
                            if (test.Length > maximumLineLength)
                            {
                                a.Add(w);
                            }
                            else
                            {
                                a[a.Count - 1] = test;
                            }
                            return a;
                        });
        }
    }
}

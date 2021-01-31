using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace XPInc.Hackathon.Framework.Extensions
{
    public static class GeneralExtensions
    {
        public static IEnumerable<IEnumerable<T>> ToChunks<T>(this IEnumerable<T> enumerable, int chunkSize)
        {
            int itemsReturned = 0;
            var list = enumerable.ToList();
            int count = list.Count;

            while (itemsReturned < count)
            {
                int currentChunkSize = Math.Min(chunkSize, count - itemsReturned);

                yield return list.GetRange(itemsReturned, currentChunkSize);

                itemsReturned += currentChunkSize;
            }
        }

        public static string ToJson<T>(this T source)
        {
            return JsonConvert.SerializeObject(source);
        }

        public static T FromJson<T>(this string source)
        {
            var converter = new ExpandoObjectConverter();
            return JsonConvert.DeserializeObject<T>(source, converter);
        }

        public static string ToQueryString<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
        {
            var nvc = new NameValueCollection();

            foreach (var item in dictionary) // transform the dictionary into a name value collection object
            {
                nvc.Add(item.Key.ToString(), item.Value.ToString());
            }

            var array = (
                from key in nvc.AllKeys
                from value in nvc.GetValues(key)
                select string.Format(
                    "{0}={1}",
                    WebUtility.UrlEncode(value),
                    WebUtility.UrlEncode(key))
                ).ToArray();

            return string.Join('&', array);
        }

        public static IOptions<T> GetOptions<T>(this IServiceCollection services) where T : class, new()
        {
            var provider = services.BuildServiceProvider();

            return provider.GetRequiredService<IOptions<T>>();
        }
    }
}

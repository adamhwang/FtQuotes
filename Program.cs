using CsvHelper;
using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FtQuotes
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var symbol = args[0];
            var req = new ApiRequest()
            {
                elements = new List<ApiRequestElement>()
                {
                    new ApiRequestElement()
                    {
                        Symbol = symbol
                    }
                }
            };

            var ohlcs = new Dictionary<DateTime, OHLC>();
            IFlurlResponse resp;

            do
            {
                resp = await "https://markets.ft.com/data/chartapi/series".PostJsonAsync(req);
                var result = await resp.GetJsonAsync<ApiResponse>();
                var prices = result.Elements.Single(e => e.Type == "price");

                if (result.Dates == null) break;

                var adds = Zip(
                    result.Dates,
                    prices.ComponentSeries.Single(p => p.Type == "Open").Values,
                    prices.ComponentSeries.Single(p => p.Type == "High").Values,
                    prices.ComponentSeries.Single(p => p.Type == "Low").Values,
                    prices.ComponentSeries.Single(p => p.Type == "Close").Values
                ).Where(add => !ohlcs.ContainsKey(add.Date.Date)).ToList();

                if (!adds.Any()) break;

                foreach (var add in adds)
                {
                    ohlcs.TryAdd(add.Date.Date, add);
                }

                req.endOffsetDays = (int)(DateTime.UtcNow - result.Dates.Min()).TotalDays - 1;
            }
            while (resp.StatusCode == (int)HttpStatusCode.OK);

            using (var writer = new StreamWriter($"{symbol}.{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(ohlcs.Values.OrderBy(o => o.Date));
            }
        }

        static IEnumerable<OHLC> Zip(IList<DateTime> dates, IList<decimal> opens, IList<decimal> highs, IList<decimal> lows, IList<decimal> closes)
        {
            if (new[] { dates.Count, opens.Count, highs.Count, lows.Count, closes.Count }.Distinct().Count() > 1)
            {
                throw new Exception("Missing data");
            }

            using (var date = dates.GetEnumerator())
            using (var open = opens.GetEnumerator())
            using (var high = highs.GetEnumerator())
            using (var low = lows.GetEnumerator())
            using (var close = closes.GetEnumerator())
            {
                while (date.MoveNext() && open.MoveNext() && high.MoveNext() && low.MoveNext() && close.MoveNext())
                    yield return new OHLC()
                    {
                        Date = date.Current,
                        Open = open.Current,
                        High = high.Current,
                        Low = low.Current,
                        Close = close.Current,
                    };
            }
        }
    }
}

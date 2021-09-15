using System.Collections.Generic;

namespace FtQuotes
{
    public class ApiRequestElement
    {
        public string Type { get; set; } = "price";
        public string Symbol { get; set; }
    }

    public class ApiRequest
    {
        public int days { get; set; } = 1825;
        public bool dataNormalized { get; set; } = false;
        public string dataPeriod { get; set; } = "Day";
        public int dataInterval { get; set; } = 1;
        public bool realtime { get; set; } = false;
        public string yFormat { get; set; } = "0.###";
        public string timeServiceFormat { get; set; } = "JSON";
        public int? endOffsetDays { get; set; }
        public string returnDateType { get; set; } = "ISO8601";
        public List<ApiRequestElement> elements { get; set; }
    }
}

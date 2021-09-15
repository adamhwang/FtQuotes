using System;
using System.Collections.Generic;

namespace FtQuotes
{
    public class ApiResponseElementComponent
    {
        public string Type { get; set; }
        public List<decimal> Values { get; set; }
    }

    public class ApiResponseElement
    {
        public string Label { get; set; }
        public string Type { get; set; }
        public string CompanyName { get; set; }
        public string IssueType { get; set; }
        public string Symbol { get; set; }
        public int Status { get; set; }
        public int UtcOffsetMinutes { get; set; }
        public string ExchangeId { get; set; }
        public string Currency { get; set; }
        public List<ApiResponseElementComponent> ComponentSeries { get; set; }
    }

    public class ApiResponse
    {
        public List<DateTime> Dates { get; set; }
        public int Status { get; set; }
        public string StatusString { get; set; }
        public List<ApiResponseElement> Elements { get; set; }
    }


}

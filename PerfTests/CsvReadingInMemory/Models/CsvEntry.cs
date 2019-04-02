using System.Runtime.Serialization;

namespace PerfTests
{
    public class CsvEntry
    {
        [DataMember(Name = "Client IP")]
        public string IP1 { get; set; }

        [DataMember(Name = "Client IP Col2")]

        public string IP2 { get; set; }

        public string Col3 { get; set; }
    }
}
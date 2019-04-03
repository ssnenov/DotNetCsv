using CsvHelper.Configuration;
using PerfTests;

namespace DotNetCsv
{
    class CsvHelperEntryMapping : ClassMap<CsvEntry>
    {
        public CsvHelperEntryMapping()
        {
            Map(x => x.IP1).Name("Client IP");
            Map(x => x.IP2).Name("Client IP Col2");
        }
    }
}
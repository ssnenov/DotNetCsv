using TinyCsvParser.Mapping;

namespace PerfTests
{
    public class TinyCsvEntryMapping : CsvMapping<CsvEntry>
    {
        public TinyCsvEntryMapping() : base()
        {
            MapProperty(0, x => x.IP1);
            MapProperty(1, x => x.IP2);
            MapProperty(2, x => x.Col3);
        }
    }
}
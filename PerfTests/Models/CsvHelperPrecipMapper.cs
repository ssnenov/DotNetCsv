using CsvHelper.Configuration;
using TinyCsvParser.Mapping;

namespace PerfTests
{
    public class CsvHelperPrecipMapper : ClassMap<Precip>
    {
        public CsvHelperPrecipMapper()
        {
            Map(x => x.Wban).Index(0);
            Map(x => x.YearMonthDay).Index(1);
            Map(x => x.Hour).Index(2);
        }
    }
}
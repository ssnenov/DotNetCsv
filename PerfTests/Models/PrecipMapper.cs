using TinyCsvParser.Mapping;

namespace PerfTests
{
    public class PrecipMapper : CsvMapping<Precip>
    {
        public PrecipMapper() : base()
        {
            MapProperty(0, x => x.Wban);
            MapProperty(1, x => x.YearMonthDay);
            MapProperty(2, x => x.Hour);
            // MapProperty(3, x => x.Precipitation);
            // MapProperty(4, x => x.PrecipitationFlag);
        }
    }
}
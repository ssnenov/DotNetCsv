using System;
using BenchmarkDotNet.Attributes;
using DotNetCsv;
using TinyCsvParser;

namespace PerfTests
{
    [MemoryDiagnoser]
    [HardwareCounters]
    public class CsvReading_InMemory
    {
        static BasicCsvReader basicCsvReader = new BasicCsvReader();
        static CsvReader<CsvEntry> csvReader = new CsvReader<CsvEntry>();
        static CsvParserOptions csvParserOptions = new CsvParserOptions(true, ',');
        static CsvReaderOptions csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });
        static TinyCsvEntryMapping csvMapper = new TinyCsvEntryMapping();
        static TinyCsvParser.CsvParser<CsvEntry> tinyParser = new TinyCsvParser.CsvParser<CsvEntry>(csvParserOptions, csvMapper);

        const string csv = @"Client IP,Client IP Col2,Col3
			127.0.0.1, 127.0.0.1, 1234
			127.0.0.1, 127.0.0.1, 1234
			127.0.0.1, 127.0.0.1, 1234
			127.0.0.1, 127.0.0.1, 1234
			127.0.0.1, 127.0.0.1, 1234
			127.0.0.1, 127.0.0.1, 1234
			127.0.0.1, 127.0.0.1, 1234
			127.0.0.1, 127.0.0.1, 1234
			127.0.0.1, 127.0.0.1, 1234
			127.0.0.1, 127.0.0.1, 1234
			127.0.0.1, 127.0.0.1, 1234
			127.0.0.1, 127.0.0.1, 1234
			127.0.0.1, 127.0.0.1, 1234
			127.0.0.1, 127.0.0.1, 1234
			127.0.0.1, 127.0.0.1, 1234
			127.0.0.1, 127.0.0.1, 1234
			127.0.0.1, 127.0.0.1, 1234
			127.0.0.1, 127.0.0.1, 1234
			127.0.0.1, 127.0.0.1, 1234
			127.0.0.1, 127.0.0.1, 1234
			127.0.0.1, 127.0.0.1, 1234
			127.0.0.1, 127.0.0.1, 1234
			127.0.0.1, 127.0.0.1, 1234
			127.0.0.1, 127.0.0.1, 1234
			127.0.0.1, 127.0.0.1, 1234
			'127.0.0.1', '127.0.0.1', 1234";

        [Benchmark]
        public void DotNetCsvReader_BasicCsvReader_Foreach()
        {
            foreach (var item in basicCsvReader.ReadFromString(csv)) { }
        }

        [Benchmark]
        public void DotNetCsvReader_CsvReader_Foreach()
        {
            foreach (var item in csvReader.ReadFromString(csv)) { }
        }

        [Benchmark]
        public void TinyCsvParser_Foreach()
        {
            foreach (var item in tinyParser.ReadFromString(csvReaderOptions, csv)) { }
        }
    }
}
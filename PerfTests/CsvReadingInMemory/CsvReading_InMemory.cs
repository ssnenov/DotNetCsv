using System;
using System.IO;
using System.Linq;
using BenchmarkDotNet.Attributes;
using DotNetCsv;
using LumenWorks.Framework.IO.Csv;
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
        public void DotNetCsvReader_BasicCsvReader()
        {
            basicCsvReader.ReadFromString(csv).ToArray();
        }

        [Benchmark]
        public void DotNetCsvReader_CsvReader()
        {
            csvReader.ReadFromString(csv).ToArray();
        }

        [Benchmark]
        public void TinyCsvParser()
        {
            tinyParser.ReadFromString(csvReaderOptions, csv).ToArray();
        }

        [Benchmark]
        public void LumenWorksCsvReader()
        {
            new CsvReader(new StringReader(csv)).ToArray();
        }

        [Benchmark]
        public void CsvHelper()
        {
            var reader = new CsvHelper.CsvReader(new StringReader(csv));
            reader.Configuration.RegisterClassMap<CsvHelperEntryMapping>();
            reader.GetRecords<CsvEntry>().ToArray();
        }
    }
}
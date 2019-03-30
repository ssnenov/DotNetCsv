using System;
using System.Linq;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Attributes;
using DotNetCsv;
using TinyCsvParser;
using TinyCsvParser.Mapping;
using System.Runtime.Serialization;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Text;

namespace PerfTests
{
    class Program
    {
        static void Main(string[] args)
        {
            // Console.Read();
            // var binFolder = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            // string fullName = binFolder.Parent.Parent.Parent.GetFiles("*.txt").First().FullName;
            // BasicCsvReader basicCsvReader = new BasicCsvReader();
            // CsvReader<Precip> csvReader = new CsvReader<Precip>();
            // var st = Stopwatch.StartNew();
            // foreach (var item in basicCsvReader.Read(fullName))
            // {
            // }
            // st.Stop();
            // System.Console.WriteLine(st.Elapsed);

            // st.Restart();
            // foreach (var item in csvReader.Read(fullName))
            // {
            // }
            // st.Stop();
            // System.Console.WriteLine(st.Elapsed);

            // CsvParserOptions csvParserOptions = new CsvParserOptions(true, ',');
            // CsvReaderOptions csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });
            // PrecipMapper csvMapper = new PrecipMapper();
            // TinyCsvParser.CsvParser<Precip> tinyParser = new TinyCsvParser.CsvParser<Precip>(csvParserOptions, csvMapper);
            // st.Restart();
            // foreach (var item in tinyParser.ReadFromFile(fullName, Encoding.ASCII))
            // {
            // }
            // st.Stop();

            // System.Console.WriteLine(st.Elapsed);

            BenchmarkRunner.Run<DotNetCsvReader>();
        }
    }

    class PrecipMapper : CsvMapping<Precip>
    {
        public PrecipMapper() : base()
        {
            MapProperty(0, x => x.Wban);
            MapProperty(1, x => x.YearMonthDay);
            MapProperty(2, x => x.Hour);
            MapProperty(3, x => x.Precipitation);
            MapProperty(4, x => x.PrecipitationFlag);
        }
    }
    class Precip
    {
        public string Wban { get; set; }
        public string YearMonthDay { get; set; }

        public int Hour { get; set; }

        public double Precipitation { get; set; }

        public double PrecipitationFlag { get; set; }
    }

    [MemoryDiagnoser]
    [HardwareCounters]
    public class DotNetCsvReader
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

    public class CsvEntry
    {
        [DataMember(Name = "Client IP")]
        public string IP1 { get; set; }

        [DataMember(Name = "Client IP Col2")]

        public string IP2 { get; set; }

        public string Col3 { get; set; }
    }

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
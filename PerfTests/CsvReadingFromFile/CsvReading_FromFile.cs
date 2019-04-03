using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using BenchmarkDotNet.Attributes;
using DotNetCsv;
using LumenWorks.Framework.IO.Csv;
using TinyCsvParser;
using TinyCsvParser.Mapping;

namespace PerfTests
{
    [MemoryDiagnoser]
    [HardwareCounters]
    [InProcessAttribute]
    public class CsvReading_FromFile
    {
        private static DirectoryInfo binFolder = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
        private static string csvFilePath = binFolder.Parent.Parent.Parent.GetFiles("*.txt")[0].FullName;

        static BasicCsvReader basicCsvReader = new BasicCsvReader();
        static CsvReader<Precip> csvReader = new CsvReader<Precip>();
        static CsvParserOptions csvParserOptions = new CsvParserOptions(true, ',');
        static PrecipMapper csvMapper = new PrecipMapper();
        static TinyCsvParser.CsvParser<Precip> tinyParser = new TinyCsvParser.CsvParser<Precip>(csvParserOptions, csvMapper);

        [Benchmark]
        public void DotNetCsvReader_BasicCsvReader()
        {
            basicCsvReader.Read(csvFilePath).ToArray();
        }

        [Benchmark]
        public void DotNetCsvReader_CsvReader()
        {
            csvReader.Read(csvFilePath).ToArray();
        }

        [Benchmark]
        public void TinyCsvParser()
        {
            tinyParser.ReadFromFile(csvFilePath, Encoding.UTF8).ToArray();
        }

        [Benchmark]
        public void LumenWorksCsvReader()
        {
            new CsvReader(new StreamReader(csvFilePath)).ToArray();
        }
    }
}
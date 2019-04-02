using System.Linq;
using BenchmarkDotNet.Running;
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
            BenchmarkRunner.Run<CsvReading_InMemory>();
            BenchmarkRunner.Run<CsvReading_FromFile>();
        }
    }
}
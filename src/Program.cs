using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCsv
{
    class Program
    {
        class TestModel
        {
            [DataMember(Name = "Client IP")]
            public string ClientIP { get; set; }

            [DataMember(Name = "Client IP Col2")]
            public string ClientIPCol2 { get; set; }

            public int Col3 { get; set; }
        }

        static void Main(string[] args)
        {
            var binFolder = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            CsvReader<TestModel> csvReader = new CsvReader<TestModel>();

            var result = csvReader.Read(binFolder.Parent.Parent.Parent.GetFiles("*.csv").First().FullName);
            foreach (var item in result)
            {
                Console.WriteLine($"IP: {item.ClientIP} | IP2: {item.ClientIPCol2} | Col3: {item.Col3}");
            }

            Console.Read();
        }
    }
}
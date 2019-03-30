using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DotNetCsv
{
    public static class ICsvReaderExtensions
    {
        public static IEnumerable<T> Read<T>(this ICsvReader<T> reader, string filePath) => reader.Read(new StreamReader(filePath));

        public static IEnumerable<T> Read<T>(this ICsvReader<T> reader, string filePath, Encoding encoding) => reader.Read(new StreamReader(filePath, encoding));

        public static IEnumerable<T> Read<T>(this ICsvReader<T> reader, Stream fileStream) => reader.Read(new StreamReader(fileStream));

        public static IEnumerable<T> Read<T>(this ICsvReader<T> reader, Stream fileStream, Encoding encoding) => reader.Read(new StreamReader(fileStream, encoding));

        public static IEnumerable<T> ReadFromString<T>(this ICsvReader<T> reader, string content) => reader.Read(new StringReader(content));
    }
}
using System.Collections.Generic;
using System.IO;

namespace DotNetCsv
{
    public interface ICsvReader<T>
    {
        IEnumerable<T> Read(TextReader textReader);
    }
}
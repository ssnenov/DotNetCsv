# DotNetCsv #

DotNetCsv is a lightweight csv reader that supports [RFC4180](https://tools.ietf.org/html/rfc4180). In addition to it's portability it is ultra fast and outperforms the most popular .net csv readers.

## Benchmarks
### Small CSV - in memory string (27 rows)
Intel Core i5 CPU M 480 2.67GHz, 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=2.2.105
  [Host]     : .NET Core 2.2.3 (CoreCLR 4.6.27414.05, CoreFX 4.6.27414.05), 64bit RyuJIT
  DefaultJob : .NET Core 2.2.3 (CoreCLR 4.6.27414.05, CoreFX 4.6.27414.05), 64bit RyuJIT

|                         Method |      Mean |     Error |    StdDev | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
|------------------------------- |----------:|----------:|----------:|------------:|------------:|------------:|--------------------:|
| DotNetCsvReader_BasicCsvReader |  15.01 us | 0.0767 us | 0.0717 us |      2.9602 |           - |           - |             4.57 KB |
|      DotNetCsvReader_CsvReader |  19.02 us | 0.0778 us | 0.0690 us |      3.6926 |           - |           - |             5.68 KB |
|                  TinyCsvParser | 103.41 us | 1.4511 us | 1.2864 us |     36.9873 |           - |           - |            20.25 KB |
|            LumenWorksCsvReader |  41.36 us | 0.2998 us | 0.2503 us |     17.0898 |           - |           - |            26.31 KB |


### Large CSV from file (1.722 million rows, 36.4 MB)
Intel Core i5 CPU M 480 2.67GHz, 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=2.2.105
  [Host] : .NET Core 2.2.3 (CoreCLR 4.6.27414.05, CoreFX 4.6.27414.05), 64bit RyuJIT

Job=InProcess  Toolchain=InProcessToolchain  

|                                 Method |    Mean |    Error |   StdDev | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
|--------------------------------------- |--------:|---------:|---------:|------------:|------------:|------------:|--------------------:|
| DotNetCsvReader_BasicCsvReader | 1.638 s | 0.0110 s | 0.0103 s |  46000.0000 |  23000.0000 |   4000.0000 |           250.33 MB |
|      DotNetCsvReader_CsvReader | 1.720 s | 0.0079 s | 0.0074 s |  46000.0000 |  23000.0000 |   4000.0000 |           250.33 MB |
|                  TinyCsvParser | 7.054 s | 0.1270 s | 0.1126 s | 335000.0000 |  86000.0000 |   5000.0000 |           501.53 MB |

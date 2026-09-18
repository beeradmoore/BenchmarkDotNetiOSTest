using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;

namespace BenchmarkDotNetiOSTest;

[MemoryDiagnoser(false)]
public class BenchmarkTests
{
    SHA256 _sha256 = SHA256.Create();
    MD5 _md5 = MD5.Create();
    byte[] _data;

    [Params(1000, 10000)]
    public int N;

    [GlobalSetup]
    public void Setup()
    {
        _data = new byte[N];
        new Random(42).NextBytes(_data);
    }

    [Benchmark]
    public byte[] Sha256() => _sha256.ComputeHash(_data);

    [Benchmark]
    public byte[] Md5() => _md5.ComputeHash(_data);
}
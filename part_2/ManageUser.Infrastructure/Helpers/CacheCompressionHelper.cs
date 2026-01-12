using System.IO.Compression;
using System.Text.Json;
using System.Text;
namespace ManageUser.Infrastructure.Helpers;

public static class CacheCompressionHelper
{
    public static byte[] Compress<T>(T data)
    {
        var json = JsonSerializer.Serialize(data);
        var bytes = Encoding.UTF8.GetBytes(json);

        using var output = new MemoryStream();
        using (var gzip = new GZipStream(output, CompressionLevel.Optimal))
        {
            gzip.Write(bytes, 0, bytes.Length);
        }

        return output.ToArray();
    }

    public static T Decompress<T>(byte[] compressedData)
    {
        using var input = new MemoryStream(compressedData);
        using var gzip = new GZipStream(input, CompressionMode.Decompress);
        using var output = new MemoryStream();

        gzip.CopyTo(output);
        var json = Encoding.UTF8.GetString(output.ToArray());

        return JsonSerializer.Deserialize<T>(json)!;
    }
}

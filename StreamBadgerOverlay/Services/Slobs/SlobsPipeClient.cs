using System;
using System.IO.Pipes;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SlobsSpike
{
    public class SlobsPipeClient : IDisposable
    {
        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
        
        private readonly NamedPipeClientStream _clientStream;

        public SlobsPipeClient()
        {
            _clientStream = new NamedPipeClientStream("slobs");
        }

        public Task ConnectAsync() => _clientStream.ConnectAsync();

        public Task<string> RunAsync(SlobsRequest request)
        {
            var jsonRpc = JsonSerializer.Serialize(request, SerializerOptions);
            return RunAsync(jsonRpc);
        }

        public async Task<T> RunAsync<T>(SlobsRequest request)
        {
            await JsonSerializer.SerializeAsync(_clientStream, request, SerializerOptions);
            _clientStream.WriteByte((byte)'\n');

            var result = await ReadResponseAsync();
            var response = JsonSerializer.Deserialize<SlobsResponse<T>>(result, SerializerOptions);
            
            return response.Result;
        }

        public async Task<string> RunAsync(string jsonRpc)
        {
            var bytes = Encoding.UTF8.GetBytes(jsonRpc + '\n');

            await _clientStream.WriteAsync(bytes, 0, bytes.Length);

            return await ReadResponseAsync();
        }

        private async Task<string> ReadResponseAsync()
        {
            var stringBuilder = new StringBuilder();

            var buffer = new byte[4 * 1024];

            int length = 4 * 1024;

            while (length == 4 * 1024)
            {
                length = await _clientStream.ReadAsync(buffer);
                var chunk = Encoding.UTF8.GetString(buffer, 0, length);
                stringBuilder.Append(chunk);
            }

            return stringBuilder.ToString();
        }

        public async Task<T> RunAsync<T>(string jsonRpc)
        {
            var json = await RunAsync(jsonRpc);
            var result = JsonSerializer.Deserialize<SlobsResponse<T>>(json, SerializerOptions);
            return result.Result;
        }

        public void Dispose()
        {
            _clientStream.Dispose();
        }
    }
}
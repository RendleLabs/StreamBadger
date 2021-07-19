using System.Threading;

namespace SlobsSpike
{
    public class SlobsRequest
    {
        private static int NextId;

        public SlobsRequest(string method, string resource, params object[] args)
        {
            Method = method;
            Params = new()
            {
                Resource = resource,
                Args = args is {Length: > 0} ? args : null,
            };
        }
        
        public string Jsonrpc => "2.0";
        public int Id { get; } = Interlocked.Increment(ref NextId);
        public string Method { get; }
        public RequestParams Params { get; }

        public class RequestParams
        {
            public string Resource { get; set; }
            public object[]? Args { get; set; }
        }
    }
}
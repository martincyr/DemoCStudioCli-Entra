namespace CsAgentClient
{
    internal sealed class HttpTracingHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            Console.WriteLine($"[TRACING] HTTP {request.Method} {request.RequestUri}");

            foreach (var header in request.Headers)
            {
                Console.WriteLine($"[TRACING] {header.Key}: {string.Join(", ", header.Value)}");
            }

            if (request.Content is not null)
            {
                foreach (var header in request.Content.Headers)
                {
                    Console.WriteLine($"[TRACING] {header.Key}: {string.Join(", ", header.Value)}");
                }
            }

            var response = await base.SendAsync(request, cancellationToken);

            Console.WriteLine($"[TRACING] Response: {(int)response.StatusCode} {response.StatusCode}");

            return response;
        }
    }
}
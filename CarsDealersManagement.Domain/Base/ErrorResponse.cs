using System.Net;

namespace CarsDealersManagement.Domain.Base
{
    public class ErrorResponse
    {
        public string Title { get; init; } = default!;

        public string Description { get; init; } = default!;

        public HttpStatusCode Status { get; init; }
    }
}

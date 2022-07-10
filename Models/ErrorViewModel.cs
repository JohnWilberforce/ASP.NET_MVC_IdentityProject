namespace IdentityFromScratch.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public Exception? exception { get; set; }

        public string? Message { get; set; }

        public ErrorViewModel(Exception? ex=null, string? message = null)
        {
            exception = ex;
            Message = message;
        }
    }
}
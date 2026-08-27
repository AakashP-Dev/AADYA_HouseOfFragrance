namespace AADYA_HouseOfFragrance.Models
{
    /// <summary>
    /// ViewModel used for the Error page to display detailed error information.
    /// Captures the Request ID from the current HTTP request for debugging purposes.
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// The unique identifier for the current HTTP request that triggered the error.
        /// This ID can be used for logging and debugging purposes.
        /// </summary>
        public string? RequestId { get; set; }

        /// <summary>
        /// Indicates whether a Request ID is available and should be displayed on the error page.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
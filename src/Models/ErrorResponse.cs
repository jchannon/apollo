namespace Apollo.Models
{
    /// <summary>
    /// API error response model
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// Gets or sets error type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets title
        /// </summary>
        public string Title { get; set; }
    }
}

namespace reminder_schedule_backend.Helpers
{
    public class ApiResponse <T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public static ApiResponse<T> Ok(T data, string message = "Berhasil")
        {
            return new ApiResponse<T>
            {
                Data = data,
                Message = message,
                Success = true
            };

        }

        public static ApiResponse<T> Fail(string message)

        {
           return new ApiResponse<T>
            {
                Data = default,
                Message = message,
                Success = false
            };
        }


    }
}

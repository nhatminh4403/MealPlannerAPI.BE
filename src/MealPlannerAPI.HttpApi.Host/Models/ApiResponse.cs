namespace MealPlannerAPI.Models
{
    public class ApiResponse
    {
        public bool Success { get; }
        public string Message { get; }

        public ApiResponse(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }

    public class ApiResponse<T> : ApiResponse
    {
        public T Data { get; }

        public ApiResponse(bool success, string message, T data)
            : base(success, message)
        {
            Data = data;
        }
    }
}

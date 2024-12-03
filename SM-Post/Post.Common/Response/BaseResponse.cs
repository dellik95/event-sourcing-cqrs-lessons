namespace Post.Common.Response
{
    public class BaseResponse<T>
    {
        public T Result { get; }

        public string Message { get; }

        public bool IsSuccessful => string.IsNullOrWhiteSpace(Message);

        public BaseResponse()
        {

        }

        private BaseResponse(string message)
        {
            this.Message = message;
        }

        private BaseResponse(T result, string message = "")
        {
            Result = result;
            Message = message;
        }

        public static BaseResponse<T> OkResult(T result, string message = "") => new(result, message);
        public static BaseResponse<T> Failure(Exception ex) => new(ex.Message);


        public static implicit operator BaseResponse<T>(T value) => new(value);

        public static implicit operator BaseResponse<T>(Exception ex) => new(ex.Message);
    }
}

public class ResultDTO<T> where T : class
{
    public T data { get; set; }
    public bool Success { get; set; }
}
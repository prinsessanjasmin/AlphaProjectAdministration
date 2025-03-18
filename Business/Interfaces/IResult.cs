namespace Business.Interfaces;

public interface IResult
{
    bool Success { get; }
    int StatusCode { get; }
    string? ErrorMessage { get; } 
}


public interface IResult<T> : IResult
{
    T? Data { get; }
}
//Got these lines from Chat GPT 4o to be able to easily access the data when using the CreatedResult.
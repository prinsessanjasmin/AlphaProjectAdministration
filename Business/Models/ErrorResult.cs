namespace Business.Models;

public class ErrorResult : Result
{
    public ErrorResult(int statusCode, string errormessage)
    {
        Success = false;
        StatusCode = statusCode;
        ErrorMessage = errormessage;
    }
}
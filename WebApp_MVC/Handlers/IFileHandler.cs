namespace WebApp_MVC.Handlers;

public interface IFileHandler
{
    Task<string> UploadFileAsync(IFormFile file); 
}

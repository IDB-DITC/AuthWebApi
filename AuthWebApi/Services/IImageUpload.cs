
namespace AuthWebApi.Services
{
  public interface IImageUpload
  {
    Task<string?> UploadFile(IFormFile file, CancellationToken C);
  }
}
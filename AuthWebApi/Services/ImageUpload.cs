using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthWebApi.Services
{
  public class ImageUpload(IWebHostEnvironment host) : IImageUpload
  {

    public async Task<string?> UploadFile(IFormFile file, CancellationToken C)
    {
      if (file == null) return null;

      string fileName = $@"\images\{file.FileName}";

      string uploadPath = $"{host.WebRootPath}{fileName}";

      using var stream = File.Create(uploadPath);

      await file.CopyToAsync(stream, C);





      return fileName;

    }

  }
}

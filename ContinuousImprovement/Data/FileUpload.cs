using BlazorInputFile;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ContinuousImprovement.Data
{
   
    public class FileUpload : IFileUpload
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public FileUpload(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task Upload1(IFileListEntry file)
        {
            var path = Path.Combine(_webHostEnvironment.ContentRootPath, "UploadedFiles",file.Name);
            var memoryStream = new MemoryStream();
            await file.Data.CopyToAsync(memoryStream);

            using(FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                memoryStream.WriteTo(fileStream);
            }
        }

        public async Task Upload(IFileListEntry file,string fileName, string beforeOrAfter)
        {
            if (file != null && file.Size > 0)
            {
                var imagePath = @"\upload\images\"+beforeOrAfter;
                var uploadPath = _webHostEnvironment.WebRootPath + imagePath;
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                var fullPath = Path.Combine(uploadPath, fileName+".jpg");
                using (FileStream fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
                {
                    await file.Data.CopyToAsync(fileStream);
                }

            }
        }
    }
}

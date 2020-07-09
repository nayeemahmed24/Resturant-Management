using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Services.FileUploadService
{
    public class FileUploadService : IFileUploadService
    {
        private IWebHostEnvironment _webHostEnvironment;
        private IStoragePathService _storagePathService;
        private string _rootPath;

        public FileUploadService(IWebHostEnvironment webHostEnvironment, IStoragePathService storagePathService)
        {
            _webHostEnvironment = webHostEnvironment;
            _rootPath = _webHostEnvironment.ContentRootPath;
            _storagePathService = storagePathService;
        }

        public async Task<string> UploadSingleFile(IFormFile file, string imagePath)
        {
            string storagePath = _storagePathService.SetFilePath(imagePath);
            string fullFileName = GenerateUniqueName(file);

            string fullpath = Path.Combine(storagePath, fullFileName);

            if (file.Length > 0)
            {
                using (var fileStream = new FileStream(fullpath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                    fileStream.Flush();
                    return Path.Combine(imagePath, fullFileName);
                }
            }
            else
            {
                return null;
            }

        }

        public string GenerateUniqueName(IFormFile formFile)
        {
            string filename = Path.GetFileNameWithoutExtension(formFile.FileName);
            string extension = Path.GetExtension(formFile.FileName);

            string uniqueName = DateTime.Now.ToString("yymmssff") + extension;
            string fullFileName = filename + "-" + uniqueName;

            return fullFileName;
        }
    }

    public interface IFileUploadService
    {
        public Task<string> UploadSingleFile(IFormFile file, string imagePath);
    }
}

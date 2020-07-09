using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Model;

namespace Services.FileUploadService
{
    public class StoragePathService : IStoragePathService
    {
        private IWebHostEnvironment _webHostEnvironment;
        private readonly string _rootPath;
        private IRoutes _routes;

        public StoragePathService(IWebHostEnvironment webHostEnvironment, IRoutes routes)
        {
            _webHostEnvironment = webHostEnvironment;
            _rootPath = _webHostEnvironment.ContentRootPath;
            _routes = routes;
        }

        public string SetFilePath(string indexPath)
        {
            string filePath = Path.Combine(_rootPath, _routes.StaticDir, indexPath);
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            return filePath;
        }
        public string GetCurrentPath()
        {
            return _rootPath;
        }
    }

    public interface IStoragePathService
    {
        public string SetFilePath(string filePath);
        public string GetCurrentPath();
    }
}

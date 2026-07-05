using GYMSystem.BLL.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GYMSystem.BLL.Services.AttachmentService
{
    public class AttachmentService : IAttachmentService
    {
       
        private readonly long maxfileSize = 5 * 1024 * 1024;
        private readonly ILogger<AttachmentService> logger;
        private readonly IWebHostEnvironment env;
        private readonly string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };

        public AttachmentService(ILogger<AttachmentService> logger,IWebHostEnvironment env)
        {
            this.logger = logger;
            this.env = env;
        }

        public async Task<Result<string>> UploadAsync(Stream fileStream, string fileName, string folderName, CancellationToken ct = default)
        {
            if (fileStream == null || !fileStream.CanRead) return Result<string>.Fail("not found file stream");

            if (string.IsNullOrWhiteSpace(fileName)) return Result<string>.Fail("not found file name");
            if (fileStream.Length > maxfileSize)
            {
                logger.LogError("File Rejected :File Too Large");
                return Result<string>.Fail("file size is too large");
            }
            var extension = Path.GetExtension(fileName);
            if (string.IsNullOrWhiteSpace(extension) || !allowedExtensions.Contains(extension))
            {
                logger.LogError("File Rejected :File Extension Not Allowed");
                return Result<string>.Fail("file extension is not allowed");
            }

            var folderPath = Path.Combine(env.ContentRootPath, folderName);
            Directory.CreateDirectory(folderPath);
            var storageFilenName = $"{Guid.NewGuid()}{fileName}";
            var filePath = Path.Combine(folderPath, storageFilenName);

            try
            {
                using var fS = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                await fileStream.CopyToAsync(fS, ct);
                return Result<string>.Ok(storageFilenName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"File Upload Failed: {ex.Message}");
                return Result<string>.Fail("file upload failed");
            }
        }

        public bool Delete(string fileName, string folderName)
        {
            var fullpath = Path.Combine(env.ContentRootPath, folderName, fileName);

            if (!File.Exists(fullpath)) return false;
            try 
            { 
                File.Delete(fullpath);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"File Delete Failed: {ex.Message}");
                return false;
            }

        }

        public (Stream Stream, string ContentType)? GetFile(string fileName, string folderName)
        {
            if (string.IsNullOrWhiteSpace(fileName)||string.IsNullOrWhiteSpace(folderName)) return null;
            var fullpath = Path.Combine(env.ContentRootPath, folderName,fileName);
            
            if (!File.Exists(fullpath)) return null;
            try
            {
                var stream = new FileStream(fullpath, FileMode.Open, FileAccess.Read);
                var contentType = GetContentType(fullpath);
                return (stream, contentType);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"File Get Failed: {ex.Message}");
                return null;
            }

        }

        private static string GetContentType(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };
        }
    }
}

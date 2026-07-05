using GYMSystem.BLL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GYMSystem.BLL.Services.AttachmentService
{
    public interface IAttachmentService
    {
        Task<Result<string>> UploadAsync(Stream fileStream, string fileName, string folderName, CancellationToken ct = default);
        bool Delete(string fileName, string folderName);
        (Stream Stream, string ContentType)? GetFile(string fileName, string folderName);
    }
}

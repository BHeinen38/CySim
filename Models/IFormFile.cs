using System;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CySim.Models
{
    public interface IFormFile
    {
        string ContentType { get; }
        string ContentDisposition { get; }
        IHeaderDictionary Headers { get; }
        long Length { get; }
        string Name { get; }
        string FileName { get; }
        Stream OpenReadStream();
        void CopyTo(Stream target);
        Task CopyToAsync(Stream target, CancellationToken? cancellationToken = null);
    }
}


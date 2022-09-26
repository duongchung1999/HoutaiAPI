using Furion;
using Furion.ClayObject;
using Furion.DynamicApiController;
using Furion.VirtualFileServer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Controllers.v2 {
    [ApiDescriptionSettings("Version2@2")]
    [Route("api/v2/[controller]")]
     class StaticResourcesController : IDynamicApiController {
        public static string BasePath = Environment.CurrentDirectory + @"\static-resources\";
        private readonly IFileProvider _physicalFileProvider;

        public StaticResourcesController(Func<FileProviderTypes, object, IFileProvider> fileProviderResolve) {
            // 解析物理文件系统
            _physicalFileProvider = fileProviderResolve(FileProviderTypes.Physical, BasePath);
        }

        /// <summary>
        /// 文件下载
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        [HttpGet, NonUnify]
        public IActionResult FileDownload(string filePath) {
            var fileName = Path.GetFileName(filePath);
            var path = Path.Combine(BasePath, filePath);
            return new FileStreamResult(new FileStream(path, FileMode.Open), "application/octet-stream") { FileDownloadName = fileName };
        }
       
        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> UploadFile( IFormFile file) {
            var fileName = Guid.NewGuid().ToString("N") + file.FileName;
            var savePath = Path.Combine(BasePath, fileName);

            var path = Path.GetDirectoryName(savePath);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            using (var stream = File.Create(savePath)) {
                await file.CopyToAsync(stream);
            }
            var request = App.HttpContext.Request;
            return new { resourceUrl = $"{request.Scheme}://{request.Host}/static-resources/{savePath.Replace(BasePath, "").Replace("\\", "/")}" };
        }
    }
}

using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Epsic.Info3e.Mays.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public FileController(IWebHostEnvironment environment) : base()
        {
            _environment = environment;
        }

        [HttpPost("upload")]
        /// <summary>
        /// Saves a file to disc.
        /// </summary>
        /// <param name="file">File to save to the disc</param>
        /// <param name="filename">Name of the file to save</param>
        /// <returns>True if the file was saved, false otherwise</returns>
        public async Task<bool> SaveFileAsync(IFormFile file, string filename = null)
        {
            try
            {
                if (filename == Path.GetFileNameWithoutExtension(filename))
                {
                    filename += $".{file.FileName.Split('.').Last()}";
                }

                filename ??= file.FileName;

                var filepath = $"{_environment.WebRootPath}\\Assets\\";

                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }

                var extension = file.FileName.Split('.').Last();
                filename = Path.GetFileNameWithoutExtension(filename);
                filename = Regex.Replace(filename, "/[^a-z0-9A-Z]+/g", "-");

                if (System.IO.File.Exists($"{filepath}{filename}.{extension}"))
                {
                    var suffix = 0;
                    while (System.IO.File.Exists($"{filepath}{filename}-{suffix}.{extension}"))
                    {
                        suffix++;
                    }

                    filename += $"-{suffix}";
                }

                using FileStream fs = System.IO.File.Create($"{filepath}{filename}.{extension}");
                await file.CopyToAsync(fs);
                fs.Flush();

                return true;
            }
            catch (Exception e)
            {
                //_logger.LogError(e);
                return false;
            }
        }
    }
}

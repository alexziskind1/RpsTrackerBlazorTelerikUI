using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http.Headers;
using System.Threading.Tasks;


namespace RPS.Web.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UploadController : ControllerBase
    {
        public IWebHostEnvironment HostingEnvironment { get; set; }

        public UploadController(IWebHostEnvironment hostingEnvironment)
        {
            HostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        public async Task<IActionResult> Save(IFormFile file) // must match SaveField which defaults to "files"
        {
            if (file != null)
            {
                try
                {
                    var fileContent = ContentDispositionHeaderValue.Parse(file.ContentDisposition);

                    // Some browsers send file names with full path.
                    // We are only interested in the file name.
                    var fileName = Path.GetFileName(fileContent.FileName.ToString().Trim('"'));
                    var physicalPath = Path.Combine(HostingEnvironment.WebRootPath, fileName);

                    // Implement security mechanisms here - prevent path traversals,
                    // check for allowed extensions, types, size, content, viruses, etc.
                    // This sample always saves the file to the root and is not sufficient for a real application.

                    using (var fileStream = new FileStream(physicalPath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                }
                catch
                {
                    // Implement error handling here, this example merely indicates an upload failure.
                    Response.StatusCode = 500;
                    await Response.WriteAsync("some error message"); // custom error message
                }
            }

            // Return an empty string message in this case
            return new EmptyResult();
        }
    }
}

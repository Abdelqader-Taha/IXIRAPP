using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace IXIR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttachmentsController : ControllerBase
    {
        private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

        public AttachmentsController()
        {
            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }
        }

        [HttpPost("upload")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UploadFiles([FromForm] string language, IFormFileCollection files)
        {
            // Validate and normalize the selected language
            var supportedLanguages = new List<string> { "en", "ar", "ku" };
            if (string.IsNullOrEmpty(language) || !supportedLanguages.Contains(language.ToLower()))
            {
                language = "en"; 
            }

            language = language.ToLower();
            string responseMessage;

            var messages = new Dictionary<string, Dictionary<string, string>>()
            {
                { "en", new Dictionary<string, string> {
                    { "no_files", "No files uploaded." },
                    { "file_size_exceeded", "File size exceeds the 10MB limit." },
                    { "upload_success", "Files uploaded successfully." }
                }},
                { "ar", new Dictionary<string, string> {
                    { "no_files", "لم يتم تحميل أي ملفات." },
                    { "file_size_exceeded", "حجم الملف يتجاوز الحد الأقصى البالغ 10 ميجابايت." },
                    { "upload_success", "تم تحميل الملفات بنجاح." }
                }},
                { "ku", new Dictionary<string, string> {
                    { "no_files", "هیچ فایلێک بار نەکرا." },
                    { "file_size_exceeded", "قەبارەی فایلەکە زیاترە لە 10MB." },
                    { "upload_success", "فایلەکان بە سەرکەوتوویی بارکران." }
                }},
            };

            if (files == null || files.Count == 0)
            {
                responseMessage = messages[language]["no_files"];
                return BadRequest(responseMessage);
            }

            var filePaths = new List<string>();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    if (file.Length > 10 * 1024 * 1024)
                    {
                        responseMessage = messages[language]["file_size_exceeded"];
                        return BadRequest(responseMessage);
                    }

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(_storagePath, fileName);

                    // Save the file to the server
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    filePaths.Add(filePath); // Add the file path to the list
                }
            }

            // Return the file paths as a comma-separated string
            responseMessage = messages[language]["upload_success"];
            return Ok(new { message = responseMessage, filePaths });
        }
    }
}

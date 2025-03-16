using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace IXIR.Controllers
{
    [Authorize(Roles = "Admin,DataEntry")]
    [Route("api/[controller]")]
    [ApiController]
    public class AttachmentsController : ControllerBase
    {
        private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        private readonly List<string> _allowedExtensions = new List<string> { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
        private readonly List<string> _allowedMimeTypes = new List<string> { "image/jpeg", "image/png", "image/gif", "image/bmp", "image/webp" };

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
                    { "invalid_file_type", "Only image files (JPG, PNG, GIF, BMP, WEBP) are allowed." },
                    { "upload_success", "Files uploaded successfully." }
                }},
                { "ar", new Dictionary<string, string> {
                    { "no_files", "لم يتم تحميل أي ملفات." },
                    { "file_size_exceeded", "حجم الملف يتجاوز الحد الأقصى البالغ 10 ميجابايت." },
                    { "invalid_file_type", "يُسمح فقط بتحميل الصور (JPG, PNG, GIF, BMP, WEBP)." },
                    { "upload_success", "تم تحميل الملفات بنجاح." }
                }},
                { "ku", new Dictionary<string, string> {
                    { "no_files", "هیچ فایلێک بار نەکرا." },
                    { "file_size_exceeded", "قەبارەی فایلەکە زیاترە لە 10MB." },
                    { "invalid_file_type", "تەنها وێنەکان (JPG, PNG, GIF, BMP, WEBP) ڕێگەپێدراوە." },
                    { "upload_success", "وێنەکان بەسەرکەوتوویی بارکرا." }
                }},
            };

            if (files == null || files.Count == 0)
            {
                responseMessage = messages[language]["no_files"];
                return BadRequest(responseMessage);
            }

            var fileUrls = new List<string>();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    // Validate file size (10MB max)
                    if (file.Length > 10 * 1024 * 1024)
                    {
                        responseMessage = messages[language]["file_size_exceeded"];
                        return BadRequest(responseMessage);
                    }

                    // Validate file extension
                    var fileExtension = Path.GetExtension(file.FileName).ToLower();
                    if (!_allowedExtensions.Contains(fileExtension))
                    {
                        responseMessage = messages[language]["invalid_file_type"];
                        return BadRequest(responseMessage);
                    }

                    // Validate MIME type
                    if (!_allowedMimeTypes.Contains(file.ContentType.ToLower()))
                    {
                        responseMessage = messages[language]["invalid_file_type"];
                        return BadRequest(responseMessage);
                    }

                    // Generate unique file name and save it
                    var fileName = Guid.NewGuid().ToString() + fileExtension;
                    var filePath = Path.Combine(_storagePath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Add only the relative path
                    fileUrls.Add($"/uploads/{fileName}");
                }
            }

            responseMessage = messages[language]["upload_success"];
            return Ok(new { message = responseMessage, fileUrls });
        }
    }
}

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
        public async Task<IActionResult> UploadFiles([FromForm] string? language, IFormFileCollection files)
        {
            // Set default language to English if not provided
            language = language?.ToLower() ?? "en";
            string responseMessage;

            // Language-specific messages
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
                { "kr", new Dictionary<string, string> {
                    { "no_files", "업로드된 파일이 없습니다." },
                    { "file_size_exceeded", "파일 크기가 10MB 제한을 초과합니다." },
                    { "upload_success", "파일이 성공적으로 업로드되었습니다." }
                }},
            }; 

            if (files == null || files.Count == 0)
            {
                responseMessage = messages.ContainsKey(language) ? messages[language]["no_files"] : messages["en"]["no_files"];
                return BadRequest(responseMessage);
            }

            var filePaths = new List<string>();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    // Check the file size (max 10MB)
                    if (file.Length > 10 * 1024 * 1024)
                    {
                        responseMessage = messages.ContainsKey(language) ? messages[language]["file_size_exceeded"] : messages["en"]["file_size_exceeded"];
                        return BadRequest(responseMessage);
                    }

                    // Generate a unique file name
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
            responseMessage = messages.ContainsKey(language) ? messages[language]["upload_success"] : messages["en"]["upload_success"];
            return Ok(new { message = responseMessage, filePaths });
        }
    }
}

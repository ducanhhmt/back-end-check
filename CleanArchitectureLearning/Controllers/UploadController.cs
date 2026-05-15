using CleanArchitecture.Application.Model;
using CleanArchitecture.Domain.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitectureLearning.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class UploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;
        private readonly ILogger<UploadController> _logger;

        private static readonly HashSet<string> AllowedExtensions =
            new(StringComparer.OrdinalIgnoreCase)
            { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp" };

        public UploadController(
            IWebHostEnvironment env,
            IConfiguration config,
            ILogger<UploadController> logger)
        {
            _env = env;
            _config = config;
            _logger = logger;
        }

        [HttpPost("Upload")]
        public async Task<IActionResult> UploadImages([FromForm] UploadImageRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (request.Files == null || request.Files.Count == 0)
                return BadRequest(new { message = "Vui lòng chọn ít nhất 1 file." });

            try
            {
                var paths = await SaveFilesAsync(request.Files, request.Category);
                return Ok(new
                {
                    message = $"Upload thành công {paths.Count} ảnh.",
                    totalFiles = paths.Count,
                    filePaths = paths
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ── PRIVATE ───────────────────────────────────────────────────────
        private async Task<List<string>> SaveFilesAsync(
            List<IFormFile> files, UploadType category)
        {
            var savedPaths = new List<string>();
            var today = DateTime.Now.ToString("yyyy-MM-dd");
            var categoryFolder = category.ToString().ToLower();
            var rootFolder = _config["UploadSettings:RootFolder"]!;

            var absolutePath = Path.Combine(
                _env.ContentRootPath, rootFolder, categoryFolder, today);

            Directory.CreateDirectory(absolutePath);

            foreach (var file in files)
            {
                ValidateFile(file);
                var fullFilePath = Path.Combine(absolutePath, file.FileName.ToString());

                await using var stream = new FileStream(fullFilePath, FileMode.Create);
                await file.CopyToAsync(stream);
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var urlPath = $"{baseUrl}/files/{categoryFolder}/{today}/{file.FileName.ToString()}";
                savedPaths.Add(urlPath);
                _logger.LogInformation("Uploaded: {Path}", urlPath);
            }

            return savedPaths;
        }

        private void ValidateFile(IFormFile file)
        {
            var maxSize = _config.GetValue<long>("UploadSettings:MaxFileSizeBytes");

            if (file.Length == 0)
                throw new ArgumentException($"File '{file.FileName}' rỗng.");

            if (file.Length > maxSize)
                throw new ArgumentException(
                    $"File '{file.FileName}' vượt quá {maxSize / 1024 / 1024}MB.");

            var ext = Path.GetExtension(file.FileName);
            if (!AllowedExtensions.Contains(ext))
                throw new ArgumentException($"'{file.FileName}' không phải định dạng ảnh hợp lệ.");
        }
    }
}

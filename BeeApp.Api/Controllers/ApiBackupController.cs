using BeeApp.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace BeeApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BackupController : ControllerBase
    {
        private readonly BackupService _backupService;
        private readonly IConfiguration _config;

        public BackupController(BackupService backupService, IConfiguration config)
        {
            _backupService = backupService;
            _config = config;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBackup()
        {
            //TODO: for test purposes
            var requestKey = Request.Headers["X-Api-Key"].ToString();
            var configuredKey = _config["Backup:ApiKey"];

            if (string.IsNullOrWhiteSpace(configuredKey) || requestKey != configuredKey)
            {
                return Unauthorized("Invalid API key");
            }

            var backup = await _backupService.CreateBackupAsync();

            if (backup.Success)
                return Ok(new { success = true, file = backup.FileName });
            else
                return StatusCode(500, new { success = false, message = backup.Message });
        }
    }
}

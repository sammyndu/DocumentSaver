using DocumentSaver.Data.Entities;
using DocumentSaver.Models;
using DocumentSaver.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DocumentSaver.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : BaseController
    {
        private readonly ILogService _logService;

        public LogsController(ILogService logService)
        {
            _logService = logService;
        }

        // GET: api/<LogsController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = new Result<List<Log>>();
            List<Log> response;

            try
            {
                response = await _logService.GetLogs();
                result.Content = response;
            }
            catch (Exception ex)
            {
                result.Error = PopulateError(500, ex.Message, "Server Error");
            }
            
            return Ok(result);
        }
    }
}

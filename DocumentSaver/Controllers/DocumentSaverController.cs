using DocumentSaver.Authorization;
using DocumentSaver.Data;
using DocumentSaver.Data.Entities;
using DocumentSaver.Models;
using DocumentSaver.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DocumentSaver.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentSaverController : BaseController
    {
        private readonly AppDbContext _db;

        private ILogService _logService;
        public DocumentSaverController(AppDbContext db, ILogService logService)
        {
            _db = db;
            _logService = logService;
        }
        // GET: api/<DocumentSaverController>
        [Authorize(Role.Admin, Role.Report)]
        //[AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            User user;
            List<DocumentInfo> documents;
            var result = new Result<List<DocumentInfo>>();
            try
            {
                user = GetAuthorizedUser();

                await _logService.AddLog(user.Username, "Reports");

                documents = _db.DocumentInfo.ToList();
                result.Content = documents;

            } catch(Exception ex)
            {
                result.Error = PopulateError(500, ex.Message, "Server Error");
            }

            return Ok(result);
        }

        // POST api/<DocumentSaverController>
        [Authorize(Role.Admin, Role.Scan)]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DocumentInfo data)
        {
            User user;
            var result = new Result<DocumentInfo>();

            try
            {
                user = GetAuthorizedUser();
                await _logService.AddLog(user.Username, "Add Scan");

                data.DateModified = DateTime.Now;
                data.DateSubmitted = DateTime.Now;
                data.DocumentId = data.DocumentId.ToUpper();
                data.DocumentName = data.DocumentName.Trim();
                _db.DocumentInfo.Add(data);
                _db.SaveChanges();

                result.Content = data;
            }
            catch (Exception ex)
            {
                result.Error = PopulateError(500, ex.Message, "Server Error");
            }

            return Ok(result);

        }

        [Authorize(Role.Admin, Role.Search)]
        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] SearchDocument data)
        {
            User user;
            var response = new Result<List<DocumentInfo>>();

            try
            {
                user = GetAuthorizedUser();
                await _logService.AddLog(user.Username, "Search");

                var query = _db.DocumentInfo.AsQueryable();
                if(!String.IsNullOrEmpty(data.DocumentId))
                {
                    query = query.Where(x => x.DocumentId == data.DocumentId);
                }

                if(!String.IsNullOrEmpty(data.DocumentName))
                {
                    query = query.Where(x => x.DocumentName.Trim().ToLower() == data.DocumentName.Trim().ToLower());
                }

                if(data.Date.HasValue)
                {
                    query = query.Where(x => x.DateSubmitted.Date == data.Date.Value.Date || x.DateModified.Date == data.Date.Value.Date);
                }

                var result = query.ToList();
                response.Content = result;
            }
            catch (Exception ex)
            {
                response.Error = PopulateError(500, ex.Message, "Server Error");
            }

            return Ok(response);

        }

        // PUT api/<DocumentSaverController>/5
        [Authorize(Role.Admin, Role.Scan)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(long id, [FromBody] DocumentInfo data)
        {
            User user;
            var result = new Result<DocumentInfo>();

            try
            {
                user = GetAuthorizedUser();
                await _logService.AddLog(user.Username, "Update Scan");

                var entity = _db.DocumentInfo.Where(x => x.Id == id).FirstOrDefault();

                entity.DocumentName = data.DocumentName;
                entity.DocumentContent = !String.IsNullOrEmpty(data.DocumentContent) ? data.DocumentContent : entity.DocumentContent;
                entity.DateModified = DateTime.Now;
                _db.DocumentInfo.Update(entity);
                _db.SaveChanges();

                result.Content = entity;
                    
            } catch(Exception ex)
            {
                result.Error = PopulateError(500, ex.Message, "Server Error");
            }
            
            return Ok(data);
        }

        // DELETE api/<DocumentSaverController>/5
        [Authorize(Role.Admin, Role.Report)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            User user;
            var result = new Result<DocumentInfo>();
            DocumentInfo entity;
            try
            {
                user = GetAuthorizedUser();
                await _logService.AddLog(user.Username, "Delete");

                entity = _db.DocumentInfo.Where(x => x.Id == id).FirstOrDefault();
                if (entity == null)
                {
                    result.Error = PopulateError(400, "Document does not exist", "Error");
                    return BadRequest("Document does not exist");
                }
                _db.DocumentInfo.Remove(entity);
                _db.SaveChanges();

            }
            catch (Exception ex)
            {
                result.Error = PopulateError(500, ex.Message, "Server Error");
            }
            return Ok(result);
        }

        private User GetAuthorizedUser()
        {
            var currentUser = (User)HttpContext.Items["User"];

            return currentUser;
        }
    }
}

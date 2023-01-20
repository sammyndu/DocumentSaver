using DocumentSaver.Authorization;
using DocumentSaver.Data;
using DocumentSaver.Data.Entities;
using DocumentSaver.Models;
using DocumentSaver.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        [Authorize(Role.Admin, Role.Report, Role.Scan)]
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
                if (user.Role == Role.Scan)
                {
                    documents = _db.DocumentInfo.AsNoTracking().Where(x => x.CreatedBy == user.Username).Select(x => new DocumentInfo
                    {
                        DocumentId = x.DocumentId,
                        DocumentName = x.DocumentName,
                        Case = x.Case,
                        Id = x.Id,
                        CreatedBy = x.CreatedBy,
                        DateModified = x.DateModified,
                        DateSubmitted = x.DateSubmitted,
                        FormDate = x.FormDate
                    }).ToList();
                } 
                else
                {
                    documents = _db.DocumentInfo.AsNoTracking().Select(x => new DocumentInfo
                    {
                        DocumentId = x.DocumentId,
                        DocumentName = x.DocumentName,
                        Case = x.Case,
                        Id = x.Id,
                        CreatedBy = x.CreatedBy,
                        DateModified = x.DateModified,
                        DateSubmitted = x.DateSubmitted,
                        FormDate = x.FormDate
                    }).ToList();
                    
                }
                result.Content = documents;

                await _logService.AddLog(user.Username, "Reports");

            } catch(Exception ex)
            {
                result.Error = PopulateError(500, ex.Message, "Server Error");
            }

            return Ok(result);
        }

        [Authorize(Role.Admin)]
        //[AllowAnonymous]
        [HttpGet("[action]")]
        public async Task<IActionResult> NewReport()
        {
            User user;
            List<DocumentInfo> documents;
            var result = new Result<List<DocumentInfo>>();
            try
            {
                user = GetAuthorizedUser();

                documents = _db.DocumentInfo.AsNoTracking().Where(x => x.New == true).Select(x => new DocumentInfo
                {
                    DocumentId = x.DocumentId,
                    DocumentName = x.DocumentName,
                    Case = x.Case,
                    Id = x.Id,
                    CreatedBy = x.CreatedBy,
                    DateModified = x.DateModified,
                    DateSubmitted = x.DateSubmitted,
                    FormDate = x.FormDate,
                    New = x.New,
                }).ToList();

                result.Content = documents;

                await _logService.AddLog(user.Username, "New Reports");

            }
            catch (Exception ex)
            {
                result.Error = PopulateError(500, ex.Message, "Server Error");
            }

            return Ok(result);
        }

        [Authorize(Role.Admin)]
        //[AllowAnonymous]
        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetDocumentContent(long id)
        {
            User user;
            DocumentInfo document;
            var result = new Result<DocumentInfo>();
            try
            {
                user = GetAuthorizedUser();

                var docEntity = _db.DocumentInfo.AsNoTracking().Where(x => x.Id == id).FirstOrDefault();

                document = new DocumentInfo
                {
                   DocumentContent = docEntity.DocumentContent ?? "",
                };

                if(document != null)
                {
                    docEntity.New = false;
                    _db.DocumentInfo.Update(docEntity);
                    _db.SaveChanges();
                }

                result.Content = document;

                await _logService.AddLog(user.Username, "Viewed Document");

            }
            catch (Exception ex)
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

                if (_db.DocumentInfo.Where(x => x.DocumentId == data.DocumentId).FirstOrDefault() != null)
                {
                    result.Error = PopulateError(400, "Document Id already exists", "Error");
                    return StatusCode(400, result); ;
                }

                data.DateModified = DateTime.Now;
                data.DateSubmitted = DateTime.Now;
                data.CreatedBy = user.Username;
                data.DocumentId = data.DocumentId.ToUpper();
                data.DocumentName = data.DocumentName.Trim();
                _db.DocumentInfo.Add(data);
                _db.SaveChanges();

                
                await _logService.AddLog(user.Username, "Add Scan");

                result.Content = data;
            }
            catch (Exception ex)
            {
                result.Error = PopulateError(500, ex.Message, "Server Error");
                return StatusCode(500, result);
            }

            return Ok(result);

        }

        [Authorize(Role.Admin, Role.Search, Role.Scan)]
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
                    query = query.Where(x => x.DocumentId.Trim().ToLower() == data.DocumentId.Trim().ToLower());
                }

                if(!String.IsNullOrEmpty(data.DocumentName))
                {
                    query = query.Where(x => x.DocumentName.Trim().ToLower() == data.DocumentName.Trim().ToLower());
                }

                if (!String.IsNullOrEmpty(data.UserName))
                {
                    query = query.Where(x => x.CreatedBy.Trim().ToLower() == data.UserName.Trim().ToLower());
                }

                if (data.Date.HasValue)
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

        [AllowAnonymous]
        [HttpGet("[action]/{id}")]
        public IActionResult CheckDocumentId(string id)
        {
            User user;
            var result = new Result<bool>();
            DocumentInfo entity;
            try
            {
                //user = GetAuthorizedUser();
                //await _logService.AddLog(user.Username, "Delete");

                entity = _db.DocumentInfo.Where(x => x.DocumentId == id).FirstOrDefault();
                if (entity == null)
                {
                    result.Content = false;
                }
                else
                {
                    result.Content = true;
                }

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

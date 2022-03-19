using DocumentSaver.Data;
using DocumentSaver.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DocumentSaver.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentSaverController : ControllerBase
    {
        private readonly AppDbContext _db;
        public DocumentSaverController(AppDbContext db)
        {
            _db = db;
        }
        // GET: api/<DocumentSaverController>
        [HttpGet]
        public IActionResult Get()
        {
            var documents = _db.DocumentInfo.ToList();
            return Ok(documents);
        }

        // GET api/<DocumentSaverController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<DocumentSaverController>
        [HttpPost]
        public IActionResult Post([FromBody] DocumentInfo data)
        {
            try
            {
                data.DateModified = DateTime.Now;
                data.DateSubmitted = DateTime.Now;
                data.DocumentId = data.DocumentId.ToUpper();
                data.DocumentName = data.DocumentName.Trim();
                _db.DocumentInfo.Add(data);
                _db.SaveChanges();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
           
        }


        [HttpPost("search")]
        public IActionResult Search([FromBody] SearchDocument data)
        {
            try
            {
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
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }

        }

        // PUT api/<DocumentSaverController>/5
        [HttpPut("{id}")]
        public IActionResult Put(long id, [FromBody] DocumentInfo data)
        {
            var entity = _db.DocumentInfo.Where(x => x.Id == id).FirstOrDefault();

            entity.DocumentName = data.DocumentName;
            entity.DocumentContent = String.IsNullOrEmpty(data.DocumentContent) ? data.DocumentContent : entity.DocumentContent;
            entity.DateModified = DateTime.Now;
            _db.DocumentInfo.Update(entity);
            _db.SaveChanges();
            return Ok(data);
        }

        // DELETE api/<DocumentSaverController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var entity = _db.DocumentInfo.Where(x => x.Id == id).FirstOrDefault();
            if(entity != null)
            {
                _db.DocumentInfo.Remove(entity);
                _db.SaveChanges();
                return Ok(entity);
            }
            return BadRequest("Document doe not exist");
        }
    }
}

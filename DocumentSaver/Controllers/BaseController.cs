using DocumentSaver.Models;
using Microsoft.AspNetCore.Mvc;

namespace DocumentSaver.Controllers
{
    [Route("api/[controller]")]
    //[Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {

        public BaseController()
        {

        }

        internal Error PopulateError(int code, string message, string type)
        {
            return new Error()
            {
                Code = code,
                Message = message,
                Type = type
            };
        }
    }

}

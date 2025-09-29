using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ovia.Models;
using Org.BouncyCastle.Security;

namespace Ovia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {

        private readonly MomEntity momDb;
        public ReportsController(MomEntity _momDb)
        {
            momDb = _momDb;
        }








    }
}

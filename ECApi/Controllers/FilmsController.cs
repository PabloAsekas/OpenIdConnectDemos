using System.Collections.Generic;
using System.Linq;
using ECApi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("ECApiPolicy")]
    public class FilmsController : ControllerBase
    {
        private readonly CinemaDbContext context;

        // GET api/values
        public FilmsController(CinemaDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Film>> Get()
        {
            return context.Films.OrderByDescending(f=>f.ImdbScore).Take(50).ToList();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<Film> Get(int id)
        {
            return context.Films.Find(id);
        }
    }
}

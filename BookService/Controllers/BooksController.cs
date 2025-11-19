using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BookService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly ILogger<BooksController> _logger;
        private readonly IConfiguration _config;
        private List<String> _books;

        public BooksController(ILogger<BooksController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;

            _books = new List<String>();
            _books.Add("Snowy nights 1");
            _books.Add("Snowy nights 2");
            _books.Add("Snowy nights 3");
        }

        [HttpGet]
        public int Get()
        {
            return Random.Shared.Next(-20, 55);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            if (id < 0 || id >= _books.Count)
            {
                return NotFound();  // Return 404 if the book is not found
            }

            return Ok(new { message = _books[id] });  // Correct way to return an anonymous object with "message" property
        }

        [HttpGet("/all")]
        public IActionResult GetAll()
        {
            return Ok(new { message = _books });  // Correct way to return an anonymous object with "message" property
        }

    }
}

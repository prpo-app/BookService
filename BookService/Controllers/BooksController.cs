using BookService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BookService.Controllers
{
    [Route("/book")]
    [ApiController]
    [Authorize]
    public class BooksController : ControllerBase
    {
        private readonly ILogger<BooksController> _logger;
        private readonly IConfiguration _config;

        private AppDbContext _context;

        public BooksController(ILogger<BooksController> logger, IConfiguration config, AppDbContext context)
        {
            _logger = logger;
            _config = config;
            _context = context;
        }
        
        /// <summary>
        /// Returns all books, testing purposes.
        /// </summary>
        /// <returns></returns>
        [HttpGet("all")]
        [AllowAnonymous]
        public IActionResult GetAllBooks()
        {
            var books = _context.Books.ToList();
            return Ok(books);
        }

        /// <summary>
        /// Returns a book with id "id".
        /// </summary>
        /// <param name="id">id of the book</param>
        /// <response code="200">Book exists.</response>
        /// <response code="404">Book with given id doesn't exist.</response>
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public IActionResult GetBook(int id)
        {
            var book = _context.Books.Find(id);

            if(book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        /// <summary>
        /// Returns list of books, starts at "index", default: 0 and returns "limit" books, default: 5.
        /// </summary>
        /// <param name="offset">starting index</param>
        /// <param name="limit">maximum number of books returned</param>
        /// <param name="author">Optional filter: returns only books by this author.</param>
        /// <param name="genre">Optional filter: returns only books of this genre.</param>
        /// <response code="200">List of books.</response>
        /// <response code="400">Invalid pagination parameters.</response>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetBooks(
            [FromQuery] int offset = 0, 
            [FromQuery] int limit = 5,
            [FromQuery] string? author = null,
            [FromQuery] string? genre = null)
        {
            if (offset < 0 || limit <= 0)
                return BadRequest("Invalid pagination parameters.");

            var query = _context.Books.AsQueryable();


            if (!string.IsNullOrWhiteSpace(author))
                query = query.Where(b => b.Author.ToLower() == author.ToLower());

            if (!string.IsNullOrWhiteSpace(genre))
                query = query.Where(b => b.Genre.ToLower() == genre.ToLower());

            var books = await query
                .OrderBy(b => b.Title)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();

            return Ok(books);
        }

        /// <summary>
        /// Creates a new book. Admin user only.
        /// </summary>
        /// <param name="dto">Book data (title, author, genre).</param>
        /// <returns></returns>
        /// <response code="201">Book created.</response>
        /// <response code="400">Missing parameters.</response>
        /// <response code="403">Access denied.</response>
        /// <response code="409">Book already exists.</response>
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult AddBook([FromBody] CreateBook dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title) ||
                string.IsNullOrWhiteSpace(dto.Author) ||
                string.IsNullOrWhiteSpace(dto.Genre))
            {
                return BadRequest("All parameters required.");
            }

            bool exists = _context.Books.Any(b =>
                b.Title == dto.Title &&
                b.Author == dto.Author &&
                b.Genre == dto.Genre
            );

            if (exists)
            {
                return Conflict("Book already exists.");
            }

            var book = new Book { Title = dto.Title, Author = dto.Author, Genre = dto.Genre};
            _context.Books.Add(book);
            _context.SaveChanges();

            //return Created();
            return CreatedAtAction(
                nameof(GetBook),
                new { id = book.Id },
                book
            );
        }


        /// <summary>
        /// Removes the book with id "id" from the list. Admin user only.
        /// </summary>
        /// <param name="id">Id of removed book.</param>
        /// <response code="204">Book deleted.</response>
        /// <response code="404">Book with given id doesn't exist.</response>
        /// <response code="403">Access denied.</response>
        [HttpDelete("{id:int}")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult DeleteBook(int id)
        {
            Book? book = (from b in _context.Books where b.Id == id select b).FirstOrDefault();
            if(book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            _context.SaveChanges();

            return NoContent();
        }

    }
}

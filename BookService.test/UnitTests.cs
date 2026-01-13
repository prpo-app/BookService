using BookService.Controllers;
using BookService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;
using Xunit;

namespace BookService.Tests.Controllers
{
    public class BooksControllerTests
    {
        private readonly Mock<ILogger<BooksController>> _mockLogger;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<AppDbContext> _mockContext;
        private readonly BooksController _controller;
        private readonly List<Book> _mockBooks;

        public BooksControllerTests()
        {
            _mockLogger = new Mock<ILogger<BooksController>>();
            _mockConfig = new Mock<IConfiguration>();
            _mockContext = new Mock<AppDbContext>(
                new DbContextOptions<AppDbContext>());

            _mockBooks = new List<Book>
            {
                new Book { Id = 1, Title = "Book One", Author = "Author A", Genre = "Fiction" },
                new Book { Id = 2, Title = "Book Two", Author = "Author A", Genre = "Science" },
                new Book { Id = 3, Title = "Book Three", Author = "Author B", Genre = "Fiction" },
                new Book { Id = 4, Title = "Book Four", Author = "Author C", Genre = "Fantasy" },
                new Book { Id = 5, Title = "Book Five", Author = "Author D", Genre = "Science" }
            };

            _mockContext
                .Setup(c => c.Books)
                .ReturnsDbSet(_mockBooks);

            _mockContext
                .Setup(c => c.Books.Find(It.IsAny<object[]>()))
                .Returns<object[]>(ids => _mockBooks.FirstOrDefault(b => b.Id == (int)ids[0]));

            _controller = new BooksController(
                _mockLogger.Object,
                _mockConfig.Object,
                _mockContext.Object);
        }

        #region GetAllBooks Tests

        [Fact]
        public void GetAllBooks_ReturnsAllBooks()
        {
            var result = _controller.GetAllBooks();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var books = Assert.IsAssignableFrom<IEnumerable<Book>>(okResult.Value);
            Assert.Equal(5, books.Count());
        }

        #endregion

        #region GetBook Tests

        [Fact]
        public void GetBook_WithValidId_ReturnsBook()
        {
            var bookId = 1;

            var result = _controller.GetBook(bookId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var book = Assert.IsType<Book>(okResult.Value);
            Assert.Equal(bookId, book.Id);
            Assert.Equal("Book One", book.Title);
        }

        [Fact]
        public void GetBook_WithZeroId_ReturnsNotFound()
        {
            var result = _controller.GetBook(0);

            Assert.IsType<NotFoundResult>(result);
        }

        #endregion

        #region AddBook Tests

        [Fact]
        public void AddBook_WithMissingAuthor_ReturnsBadRequest()
        {
            var invalidDto = new CreateBook
            {
                Title = "Title",
                Author = "", 
                Genre = "Genre"
            };

            var controllerWithAdmin = CreateControllerWithAdminUser();

            var result = controllerWithAdmin.AddBook(invalidDto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("All parameters required.", badRequestResult.Value);
        }

        #endregion

        #region DeleteBook Tests

        [Fact]
        public void DeleteBook_WithValidId_ReturnsNoContent()
        {
            var bookId = 1;
            var controllerWithAdmin = CreateControllerWithAdminUser();

            var result = controllerWithAdmin.DeleteBook(bookId);

            Assert.IsType<NoContentResult>(result);
            _mockContext.Verify(c => c.Books.Remove(It.Is<Book>(b => b.Id == bookId)), Times.Once);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Fact]
        public void DeleteBook_WithZeroId_ReturnsNotFound()
        {
           
            var controllerWithAdmin = CreateControllerWithAdminUser();
            var result = controllerWithAdmin.DeleteBook(0);

            Assert.IsType<NotFoundResult>(result);
        }

        #endregion 

        #region Helper Methods

        private BooksController CreateControllerWithAdminUser()
        {
            var controller = new BooksController(
                _mockLogger.Object,
                _mockConfig.Object,
                _mockContext.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(ClaimTypes.Name, "admin@test.com")
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            return controller;
        }

        #endregion

    }
}

using Microsoft.AspNetCore.Mvc;
using MediatR;
using BooksLibrary.Application.App.Books.Queries;
using BooksLibrary.Application.App.Books.Commands;

namespace BooksLibrary.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IMediator _mediator;
        public BooksController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("page-number{pageNumber}/page-size{pageSize}")]
        public async Task<IActionResult> GetAllBooksPaginated(int pageNumber, int pageSize) 
        {
            var result = await _mediator.Send(new GetPagedBooks { PageNumber = pageNumber, PageSize = pageSize});

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(int id)
        {
            var result = await _mediator.Send(new GetBookById { Id = id});

            if(result == null)
            {
                return NotFound($"Book with id {id} not found");
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddBook(CreateBook createBook)
        {
            var result = await _mediator.Send(createBook);

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditBook(int id, [FromBody] UpdateBook updateBook)
        {
            if(id != updateBook.Id)
            {
                return BadRequest();
            }

            var result = await _mediator.Send(updateBook);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id, [FromBody] DeleteBook deleteBook)
        {
            if (id != deleteBook.Id)
            {
                return BadRequest();
            }

            await _mediator.Send(deleteBook);
            
            return Ok();
        }
    }
}

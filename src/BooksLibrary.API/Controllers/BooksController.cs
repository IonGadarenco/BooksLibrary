using Microsoft.AspNetCore.Mvc;
using MediatR;
using BooksLibrary.Application.App.Books.Queries;
using BooksLibrary.Application.App.Books.Commands;
using BooksLibrary.Application.Common.Models;
using BooksLibrary.Application.App.Books.DTOs;

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

        [HttpGet("paged")]
        public async Task<IActionResult> GetAllBooksPaginated([FromQuery] PagedRequest pagedRequest) 
        {
            var result = await _mediator.Send(new GetPagedBooksQuery { PagedRequest = pagedRequest});

            return Ok(result);
        }

        [HttpGet("{bookId}")]
        public async Task<IActionResult> GetBookById( [FromRoute] int bookId)
        {
            var result = await _mediator.Send(new GetBookByIdQuery { Id = bookId});

            if(result == null)
            {
                return NotFound($"Book with id {bookId} not found");
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddBook( [FromBody] CreateBookCommand createBookCommand)
        {
            var result = await _mediator.Send(createBookCommand);

            return Ok(result);
        }

        [HttpPut("{bookId}")]
        public async Task<IActionResult> EditBook([FromRoute] int bookId, [FromBody] UpdateBookCommandDto updateBookCommandDto)
        {
            var updateBookCommand = new UpdateBookCommand
            {
                Id = bookId,
                Title = updateBookCommandDto.Title,
                Description = updateBookCommandDto.Description,
                PublisherId = updateBookCommandDto.PublisherId,
                
            };
            var result = await _mediator.Send(updateBookCommand);

            return Ok(result);
        }

        [HttpDelete("{bookId}")]
        public async Task<IActionResult> DeleteBook( [FromRoute] int bookId)
        {
            await _mediator.Send(new DeleteBookCommand { Id = bookId});
            
            return Ok();
        }
    }
}

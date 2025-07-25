﻿using Microsoft.AspNetCore.Mvc;
using MediatR;
using BooksLibrary.Application.App.Books.Queries;
using BooksLibrary.Application.App.Books.Commands;
using BooksLibrary.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using BooksLibrary.Application.App.Books.Commands.DTOs;
using BooksLibrary.Application.Common.Abstractions;
using BooksLibrary.API.Controllers.Dtos;
using BooksLibrary.Application.App.Likes.Command;
using BooksLibrary.Application.App.Reservations.Command;
using BooksLibrary.Application.App.Reviews.Command;
using BooksLibrary.Application.App.Reviews.DTOs;
using Microsoft.EntityFrameworkCore;
using BooksLibrary.Application.App.Likes.DTOs;
using BooksLibrary.Application.App.Likes.Queries;
using AutoMapper;

namespace BooksLibrary.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IAzureBlobService _azureBlobService;
        private readonly ILogger<BooksController> _logger;
        private readonly IMapper _mapper;
        public BooksController(
            IMediator mediator, 
            ILogger<BooksController> logger,
            IAzureBlobService azureBlobService,
            IMapper mapper)
        {
            _mediator = mediator;
            _logger = logger;
            _azureBlobService = azureBlobService;
            _mapper = mapper;
        }

        //[Authorize(Roles = "admin, user, vip")]
        [HttpPost("paged")]
        public async Task<ActionResult<PaginatedResult<BookListDto>>> GetAllBooksPaginated([FromBody] PagedRequest pagedRequest)
        {
            var result = await _mediator.Send(new GetPagedBooksQuery { PagedRequest = pagedRequest });
            return Ok(result);
        }

        //[Authorize(Roles = "admin, user, vip")]
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

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> AddBook( [FromForm] BookApiRequestDto dto)
        {
            var cmd = _mapper.Map<CreateBookCommand>(dto);
            var result = await _mediator.Send(cmd);
            return Ok(result);
        }

        [Authorize(Roles = "admin")]
        [HttpPut]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> EditBook([FromForm] UpdateBookCommandDto dto)
        {
            var cmd = _mapper.Map<UpdateBookCommand>(dto);
            await _mediator.Send(cmd);
            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{bookId}")]
        public async Task<IActionResult> DeleteBook( [FromRoute] int bookId)
        {
            await _mediator.Send(new DeleteBookCommand { Id = bookId});
            
            return Ok();
        }

        [Authorize(Roles = "admin, user, vip")]
        [HttpPost("{bookId}/like")]
        public async Task<IActionResult> ToggleLike([FromRoute] int bookId)
        {
            var command = new ToggleLikeCommand { BookId = bookId };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("{bookId}/reserve")]
        public async Task<IActionResult> ReserveBook([FromRoute] int bookId)
        {
            var command = new ReserveBookCommand { BookId = bookId };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("{bookId}/reserve")]
        public async Task<IActionResult> CancelReservation([FromRoute] int bookId)
        {
            var command = new CancelReservationCommand { BookId = bookId };
            await _mediator.Send(command);
            return NoContent();
        }

        [Authorize]
        [HttpPost("{bookId}/reviews")]
        public async Task<IActionResult> AddReview([FromRoute] int bookId, [FromBody] AddReviewDto reviewDto)
        {
            var command = new AddReviewCommand
            {
                BookId = bookId,
                Comment = reviewDto.Comment
            };

            var result = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetBookById), new { bookId = bookId }, result);
        }

        [Authorize]
        [HttpGet("likes")]
        public Task<List<LikedBookDto>> GetLikedBooks()
        {
            var result = _mediator.Send(new GetLikedBookQuery());

            return result;
        }
 
    }
}

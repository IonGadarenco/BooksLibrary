using BooksLibrary.Application.App.Auth.Sync.Command;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BooksLibrary.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("sync")]
        public async Task<IActionResult> SyncUser()
        {
            var result = await _mediator.Send(new SyncUserCommand());
            return Ok(result);
        }
    }
}

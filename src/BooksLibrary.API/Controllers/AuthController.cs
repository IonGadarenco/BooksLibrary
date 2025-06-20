using BooksLibrary.Application.App.Auth.Login.Command;
using BooksLibrary.Application.App.Auth.Login.DTOs;
using BooksLibrary.Application.App.Auth.Register.Command;
using BooksLibrary.Application.App.Auth.Register.DTOs;
using MediatR;
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
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var command = new RegisterCommand
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                RoleName = registerDto.RoleName
            };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var command = new LoginCommand
            {
                Email = loginDto.Email,
                LastName = loginDto.LastName
            };
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}


using AutoMapper;
using BooksLibrary.Application.App.Auth.Abstractions;
using BooksLibrary.Application.App.Auth.AuthResponse.DTOs;
using BooksLibrary.Application.Commun.Abstractions;
using BooksLibrary.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace BooksLibrary.Application.App.Auth.Login.Command
{
    public class LoginCommand : IRequest<AuthResponseDto>
    {
        public string Email { get; set; }
        public string LastName { get; set; }
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto> 
    {
        private readonly IRepository<User> _userRepo;
        private readonly ILogger<LoginCommandHandler> _logger;
        private readonly IJwtService _jwtService;
        public LoginCommandHandler(
            IRepository<User> userRepo, 
            ILogger<LoginCommandHandler> logger, 
            IJwtService jwtService) 
        {
            _userRepo = userRepo;
            _logger = logger;
            _jwtService = jwtService;
        }

        public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var userQueryable = await _userRepo.GetAllAsync();
            var user = await userQueryable
                .FirstOrDefaultAsync(u => 
                u.Email == request.Email && 
                u.LastName == request.LastName, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("Login failed. User not found: {Email}", request.Email);
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            var roleName = user.Role;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, roleName.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var identity = new ClaimsIdentity(claims);
            var token = _jwtService.WriteToken(_jwtService.CreateSecurityToken(identity));

            return new AuthResponseDto
            {
                Token = token,
                Email = user.Email,
                Role = roleName,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }

    }
}

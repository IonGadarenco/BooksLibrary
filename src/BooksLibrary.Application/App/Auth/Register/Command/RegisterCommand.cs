

using BooksLibrary.Application.App.Auth.Abstractions;
using BooksLibrary.Application.App.Auth.AuthResponse.DTOs;
using BooksLibrary.Application.Commun.Abstractions;
using BooksLibrary.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace BooksLibrary.Application.App.Auth.Register.Command
{
    public class RegisterCommand : IRequest<AuthResponseDto>
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RoleName { get; set; }
    }

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
    {
        private readonly IRepository<User> _userRepo;
        private readonly IRepository<Role> _roleRepo;
        private readonly IJwtService _jwtService;
        private readonly ILogger<RegisterCommandHandler> _logger;

        public RegisterCommandHandler(
            IRepository<User> userRepo,
            IRepository<Role> roleRepo,
            IJwtService jwtService,
            ILogger<RegisterCommandHandler> logger)
        {
            _userRepo = userRepo;
            _roleRepo = roleRepo;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            // Verificăm dacă utilizatorul există deja
            var users = await _userRepo.GetAllAsync();
            var existingUser = await users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (existingUser != null)
            {
                _logger.LogWarning("Register failed. User already exists: {Email}", request.Email);
                throw new InvalidOperationException("User already exists.");
            }

            // Căutăm rolul
            var role = await _roleRepo.GetByNameAsync(request.RoleName);
            if (role == null)
            {
                role = new Role { RoleName = request.RoleName };
                await _roleRepo.AddAsync(role);
            }

            // Creăm utilizatorul
            var newUser = new User
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Roles = new List<Role> { role }
            };

            await _userRepo.AddAsync(newUser);

            // JWT
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, newUser.Email),
                new Claim(ClaimTypes.NameIdentifier, newUser.Id.ToString()),
                new Claim(ClaimTypes.Role, role.RoleName)
            };

            var identity = new ClaimsIdentity(claims);
            var token = _jwtService.WriteToken(_jwtService.CreateSecurityToken(identity));

            return new AuthResponseDto
            {
                Token = token,
                Email = newUser.Email,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                RoleName = role.RoleName
            };
        }
    }
}

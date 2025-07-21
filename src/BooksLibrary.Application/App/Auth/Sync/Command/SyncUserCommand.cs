using System.Security.Claims;
using AutoMapper;
using BooksLibrary.Application.App.Auth.AuthResponse.DTOs;
using BooksLibrary.Application.Commun.Abstractions;
using BooksLibrary.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BooksLibrary.Application.App.Auth.Sync.Command
{
    public class SyncUserCommand : IRequest<AuthResponseDto>
    {
    }

    public class SyncUserCommandHandler : IRequestHandler<SyncUserCommand, AuthResponseDto>
    {
        private readonly IRepository<User> _userRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<SyncUserCommandHandler> _logger;
        private readonly IMapper _mapper;

        public SyncUserCommandHandler(
            IRepository<User> userRepo,
            IHttpContextAccessor httpContextAccessor,
            ILogger<SyncUserCommandHandler> logger,
            IMapper mapper)
        {
            _userRepo = userRepo;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<AuthResponseDto> Handle(SyncUserCommand request, CancellationToken cancellationToken)
        {
            var userClaimsPrincipal = _httpContextAccessor.HttpContext.User;

            var auth0Id = userClaimsPrincipal.FindFirst("sub")?.Value ?? 
                          userClaimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var email = userClaimsPrincipal.FindFirst("email")?.Value ??
                        userClaimsPrincipal.FindFirst(ClaimTypes.Email)?.Value;

            var firstName = userClaimsPrincipal.FindFirst("given_name")?.Value;
            var lastName = userClaimsPrincipal.FindFirst("family_name")?.Value; 

            firstName ??= "Unknown";
            lastName ??= "Unknown";

            var roleString = userClaimsPrincipal.FindFirst("https://bookslibrary.com/role")?.Value ??
                             userClaimsPrincipal.FindFirst("role")?.Value;
            roleString ??= Roles.user.ToString();

            if (string.IsNullOrWhiteSpace(auth0Id))
            {
                _logger.LogWarning("Auth0 User ID (sub) claim is missing or empty.");
                throw new UnauthorizedAccessException("Invalid token. Auth0 User ID is required.");
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                _logger.LogWarning("Email claim is missing for Auth0 ID: {Auth0Id}. Using generated email.", auth0Id);
                email = $"no-email-{auth0Id}@example.com";
            }

            if (!Enum.TryParse<Roles>(roleString, ignoreCase: true, out var role))
            {
                _logger.LogWarning("Could not parse role string '{RoleString}'. Defaulting to 'user'.", roleString);
                role = Roles.user;
            }

            var existingUser = await _userRepo.GetByAuth0IdAsync(auth0Id);

            if (existingUser != null)
            {
                bool updated = false;
                if (existingUser.FirstName != firstName) { existingUser.FirstName = firstName; updated = true; }
                if (existingUser.LastName != lastName) { existingUser.LastName = lastName; updated = true; }
                if (existingUser.Email != email) { existingUser.Email = email; updated = true; }
                if (existingUser.Role != role) { existingUser.Role = role; updated = true; }

                if (updated)
                {
                    _logger.LogInformation("Updating existing user profile for Auth0 ID: {Auth0Id}", auth0Id);
                    await _userRepo.UpdateAsync(existingUser); 
                }

                _logger.LogInformation("User already exists: {Auth0Id} (Email: {Email}, Name: {FirstName} {LastName}, Role: {Role})",
                                auth0Id, existingUser.Email, existingUser.FirstName, existingUser.LastName, existingUser.Role.ToString());
                return _mapper.Map<AuthResponseDto>(existingUser);
            }

            var newUser = new User
            {
                Auth0Id = auth0Id,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Role = role
            };

            await _userRepo.AddAsync(newUser);

            _logger.LogInformation("New user synced: {Auth0Id} (Email: {Email}, Name: {FirstName} {LastName}, Role: {Role})",
                            auth0Id, email, firstName, lastName, role.ToString());

            return _mapper.Map<AuthResponseDto>(newUser);
        }
    }
}
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NoteAPI.Entities;
using NoteAPI.Exceptions;
using NoteAPI.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace NoteAPI.Services
{
    public interface IAccountService
    {
        void RegisterUser(RegisterUserDto dto);
        string GenerateJwt(LoginDto dto);
        void UpdateAccount(UpdateUserDto dto);
        void UpdatePassword(UpdatePasswordDto dto);
        UserDto GetUserData();
    }

    public class AccountService : IAccountService
    {
        private readonly DatabaseContext _dbContext;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly AuthenticationSettings _authenticationSettings;
        private readonly IUserContextService _userContextService;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountService> _logger;

        public AccountService(DatabaseContext dbContext, IPasswordHasher<User> passwordHasher, AuthenticationSettings authenticationSettings,
            IUserContextService userContextService, IMapper mapper, ILogger<AccountService> logger)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _authenticationSettings = authenticationSettings;
            _userContextService = userContextService;
            _mapper = mapper;
            _logger = logger;
        }

        public void RegisterUser(RegisterUserDto dto)
        {
            var newUser = new User()
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                DateOfBirth = dto.DateOfBirth,
                RoleId = 2
            };

            var hashedPassword = _passwordHasher.HashPassword(newUser, dto.Password);

            newUser.PasswordHash = hashedPassword;
            _dbContext.Users.Add(newUser);
            _dbContext.SaveChanges();

            _logger.LogWarning($"[AccountService] Account created - {dto.Email} !");
        }

        public string GenerateJwt(LoginDto dto)
        {
            var user = _dbContext.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Email == dto.Email);

            if (user is null)
            {
                throw new BadRequestException("Invalid email or password");
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                throw new BadRequestException("Invalid email or password");
            }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                new Claim(ClaimTypes.Role, $"{user.Role.Name}"),
            };

            if (!string.IsNullOrEmpty(user.FirstName) && !string.IsNullOrEmpty(user.LastName))
            {
                claims.Add(new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"));
            }

            if (user.DateOfBirth.Value > DateTime.MinValue)
            {
                claims.Add(new Claim("DateOfBirth", user.DateOfBirth.Value.ToString("yyyy-MM-dd")));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(_authenticationSettings.JwtExpireDays);

            var token = new JwtSecurityToken(_authenticationSettings.JwtIssuer
                , _authenticationSettings.JwtIssuer
                , claims, expires: expires
                , signingCredentials: cred);

            var tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.WriteToken(token);
        }

        public void UpdateAccount (UpdateUserDto dto)
        {
            var user = _dbContext
                       .Users
                       .FirstOrDefault(u => u.ID == _userContextService.GetUserID);

            if (user is null)
            {
                throw new NotFoundException("User not found");
            }

            user = _mapper.Map(dto, user);

            _dbContext.Update(user);
            _dbContext.SaveChanges();

            _logger.LogWarning($"[AccountService] Account updated - {dto.Email} !");
        }

        public void UpdatePassword(UpdatePasswordDto dto)
        {
            var user = _dbContext
                       .Users
                       .FirstOrDefault(u => u.ID == _userContextService.GetUserID);

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.OldPassword);

            if (result == PasswordVerificationResult.Failed)
            {
                throw new BadRequestException("Invalid password");
            }

            var hashedPassword = _passwordHasher.HashPassword(user, dto.Password);
            user.PasswordHash = hashedPassword;

            _dbContext.Update(user);
            _dbContext.SaveChanges();

            _logger.LogWarning($"[AccountService] Password changed - {user.Email} !");
        }

        public UserDto GetUserData()
        {
            var user = _dbContext
                       .Users
                       .FirstOrDefault(u => u.ID == _userContextService.GetUserID);

            if (user is null)
            {
                throw new NotFoundException("User not found");
            }

            var userDto = new UserDto()
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = (DateTime)user.DateOfBirth
            };

            return userDto;
        }
    }
}
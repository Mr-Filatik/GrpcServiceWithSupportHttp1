using GrpcServiceWithSupportHttp1.Database.Entities;
using GrpcServiceWithSupportHttp1.Services;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace GrpcServiceWithSupportHttp1.Database.Services
{
    public class UserService : IUserService
    {
        private readonly int _selectCount = 10;
        private readonly ILogger<UserService> _logger;
        private readonly DatabaseContext _dbContext;

        public UserService(ILogger<UserService> logger,
                           DatabaseContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<string> Login(string email, string password)
        {
            User? user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email && x.PasswordHash == password);
            if (user == null) { throw new Exception("User not found"); }
            user.AccessKey = Guid.NewGuid().ToString();
            _dbContext.Users.Update(user);
            _dbContext.SaveChanges();
            user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email && x.PasswordHash == password && x.AccessKey != null);
            if (user == null) { throw new Exception("Error on server"); }
            return user.AccessKey;
        }

        public async Task<bool> Logout(string accessKey) 
        {
            User? user = await _dbContext.Users.FirstOrDefaultAsync(x => x.AccessKey == accessKey);
            if (user == null) { throw new Exception("User not found"); };
            user.AccessKey = null;
            _dbContext.Users.Update(user);
            _dbContext.SaveChanges();
            user = await _dbContext.Users.FirstOrDefaultAsync(x => x.AccessKey == accessKey);
            if (user == null) { return true; }
            return false;
        }

        public Task<int> GetCount(Func<User, bool> predicate = null)
        {
            var task = 0;
            if (predicate == null)
            {
                task = _dbContext.Users.Count();
            }
            else
            {
                task = _dbContext.Users.Count(predicate);
            }
            return Task.FromResult(task);
        }

        public async Task<User> GetUser(int id)
        {
            User? user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null) { throw new Exception("User not found"); };
            return user;
        }

        public async Task<(List<User> users, int currPage, int lastPage)> GetUsers(Func<User, bool> predicate = null, int page = 1)
        {
            int count = await GetCount(predicate);
            if (page < 1) page = 1;
            int lastPage = (count % _selectCount) == 0 ? (count / _selectCount) : ((count / _selectCount) + 1);
            if (page > lastPage) page = lastPage;
            List<User> users = new List<User>();
            if (predicate != null)
            {
                users = _dbContext.Users.Where(predicate).Skip((page - 1) * _selectCount).Take(_selectCount).ToList();
            }
            else
            {
                users = _dbContext.Users.Skip((page - 1) * _selectCount).Take(_selectCount).ToList();
            }
            if (!users.Any()) { throw new Exception("Users not found"); }
            return (users, page, lastPage);
        }

        public async Task<User> CreateUser(User user)
        {
            User? userOld = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == user.Email);
            if (userOld != null) { throw new Exception("Email already exists"); }
            user.PasswordHash = user.PasswordHash;
            user.Id = 0;
            user.AccessKey = null;
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
            User? newUser = _dbContext.Users.FirstOrDefault(x => x.Surname == user.Surname && x.Name == user.Name && x.Patronymic == user.Patronymic && x.Email == user.Email && x.Birthday == user.Birthday);
            if (newUser == null) { throw new Exception("Error on server"); }
            return newUser;
        }

        public async Task<User> UpdateUser(User user)
        {
            _dbContext.Users.Update(user);
            _dbContext.SaveChanges();
            User? newUser = _dbContext.Users.FirstOrDefault(x => x.Id == user.Id &&
                                                                 x.Surname == user.Surname &&
                                                                 x.Name == user.Name &&
                                                                 x.Patronymic == user.Patronymic &&
                                                                 x.Email == user.Email &&
                                                                 x.Birthday == user.Birthday &&
                                                                 x.PasswordHash == user.PasswordHash);
            if (newUser == null) { throw new Exception("Error on server"); }
            return newUser;
        }

        public async Task<bool> DeleteUser(int id)
        {
            User? user = _dbContext.Users.FirstOrDefault(x => x.Id == id);
            if (user == null) { throw new Exception("User not found"); }
            _dbContext.Users.Remove(user);
            _dbContext.SaveChanges();
            User? newUser = _dbContext.Users.FirstOrDefault(x => x.Id == id);
            if (newUser != null) { throw new Exception("Error on server"); }
            return true;
        }

        public string PasswordHasher(string password)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(password: password,
                                                               salt: RandomNumberGenerator.GetBytes(128 / 8),
                                                               prf: KeyDerivationPrf.HMACSHA256,
                                                               iterationCount: 100000,
                                                               numBytesRequested: 256 / 8));
        }

        public async Task<bool> IsLogin(string accessKey)
        {
            if (accessKey == "Access-Key") return true; //delete
            User? user = await _dbContext.Users.FirstOrDefaultAsync(x => x.AccessKey == accessKey);
            if (user == null) return false;
            return true;
        }
    }
}
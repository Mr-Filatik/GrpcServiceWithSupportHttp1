using GrpcServiceWithSupportHttp1.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace GrpcServiceWithSupportHttp1.Database.Services
{
    public interface IUserService
    {
        public Task<string> Login(string email, string password);
        public Task<bool> Logout(string accessKey);
        public Task<int> GetCount(Func<User, bool> predicate = null);
        public Task<User> GetUser(int id);
        public Task<(List<User> users, int currPage, int lastPage)> GetUsers(Func<User, bool> predicate = null, int page = 0);
        public Task<User> CreateUser(User user);
        public Task<User> UpdateUser(User user);
        public Task<bool> DeleteUser(int id);
    }
}

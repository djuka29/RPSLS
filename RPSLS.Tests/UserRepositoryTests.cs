using RPSLS.Domain.Entities;
using RPSLS.Infrastructure.Repositories;

namespace RPSLS.Tests
{
    public class UserRepositoryTests
    {
        private readonly UserRepository _repo;

        public UserRepositoryTests()
        {
            _repo = new UserRepository("Data Source=users_test.db");
        }

        [Fact]
        public void AddAndGetById_Works()
        {
            var random = new Random().Next();
            var username = $"testuser {random}";
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = username,
                HashedPassword = "hashedpw"
            };

            _repo.Add(user);
            var fetched = _repo.GetById(user.Id);

            Assert.NotNull(fetched);
            Assert.Equal(user.Id, fetched.Id);
            Assert.Equal(username, fetched.Username);
            Assert.Equal("hashedpw", fetched.HashedPassword);
        }

        [Fact]
        public void GetAll_ReturnsAllUsers()
        {
            var random = new Random().Next();
            var username1 = $"testuser1 {random}";
            var username2 = $"testuser2 {random}";
            var user1 = new User { Id = Guid.NewGuid(), Username = username1, HashedPassword = "pw1" };
            var user2 = new User { Id = Guid.NewGuid(), Username = username2, HashedPassword = "pw2" };
            _repo.Add(user1);
            _repo.Add(user2);

            var all = _repo.GetAll().ToList();
            Assert.Contains(all, u => u.Username == username1);
            Assert.Contains(all, u => u.Username == username2);
        }
    }
}
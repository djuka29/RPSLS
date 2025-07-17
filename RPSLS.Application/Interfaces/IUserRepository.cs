using RPSLS.Domain.Entities;

namespace RPSLS.Application.Interfaces;

public interface IUserRepository
{
    void Add(User user);
    User? GetById(Guid id); 
    User? GetByUsername(string username);
    IEnumerable<User> GetAll();
}
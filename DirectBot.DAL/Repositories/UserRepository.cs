using DirectBot.Core.Models;
using DirectBot.Core.Repositories;
using DirectBot.DAL.Data;

namespace DirectBot.DAL.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<User> GetAll()
    {
        return _context.Users.ToList();
    }

    public void Add(User entity)
    {
        _context.Add(entity);
        _context.SaveChanges();
    }

    public void Delete(User entity)
    {
        _context.Remove(entity);
        _context.SaveChanges();
    }

    public void Update(User entity)
    {
        _context.SaveChanges();
    }

    public User? Get(long id)
    {
        return _context.Users.FirstOrDefault(user => user.Id == id);
    }
    
    public int GetCount()
    {
        return _context.Users.Count();
    }
}
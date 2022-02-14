using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DirectBot.Core.Interfaces;
using DirectBot.DAL.Data;
using DirectBot.DAL.Mapper;
using DirectBot.DAL.Models;
using DirectBot.DAL.Repositories;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace DirectBot.Tests;

public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; } = null!;
}

[TestFixture]
public class UserRepositoryTests
{
    private UserRepository _userRepository;

    [SetUp]
    public void SetUp()
    {
        var dbContextMock = new DbContextMock<TestDbContext>(new DbContextOptionsBuilder<TestDbContext>().Options);
        var usersDbSetMock = dbContextMock.CreateDbSetMock(x => x.Users, GetTestUsers());
        var mappingConfig = new MapperConfiguration(mc => { mc.AddProfile(new MappingProfile()); });
        var mock = new Mock<ApplicationDbContext>();
        mock.Setup(repo => repo.Users).Returns(usersDbSetMock.Object);
        _userRepository = new UserRepository(mock.Object, mappingConfig.CreateMapper());
    }

    [Test]
    public async Task RepositoryAvoidsCyclicalDependencies()
    {
        var user = await _userRepository.GetAsync(1);
        
        Assert.AreEqual(user.CurrentWork, user.CurrentWork.User);
    }


    private IEnumerable<User> GetTestUsers()
    {
        var user = new User {Id = 1};
        var work = new Work {Id = 1, User = user};
        user.CurrentWork = work;
        var users = new List<User>
        {
            user
        };
        return users;
    }
}
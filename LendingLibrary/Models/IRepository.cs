using System;
namespace LendingLibrary.Models
{
    public interface IRepository
    {
        IApplicationDbContext Db { get; }
        IApplicationUserManager Manager { get; }
    }
}

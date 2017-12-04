using System;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LendingLibrary.Models
{
    public class Repository : IRepository
    {
        public IApplicationDbContext Db { get; protected set; }
        public IApplicationUserManager Manager { get; protected set; }

        public Repository()
        {
            var context = new ApplicationDbContext();
            Db = context;
            Manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
        }

        public Repository(IApplicationDbContext db, IApplicationUserManager manager) {
            this.Db = db;
            this.Manager = manager;
        }
    }
}

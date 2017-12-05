/*
 * LendingLibrary - An online private bookshelf catalog and sharing application
 * Copyright (C) 2017 Robert Corrigan
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as
 * published by the Free Software Foundation, either version 3 of the
 * License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Affero General Public License for more details.
 * 
 * You should have received a copy of the GNU Affero General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using LendingLibrary.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace LendingLibrary.Controllers
{
    public class BaseController : Controller
    {
        protected IApplicationDbContext db;
        protected IRepository repo;

        public BaseController()
        {
            repo = new Repository();
            db = repo.Db;
        }

        public BaseController(IApplicationDbContext db, IApplicationUserManager manager)
        {
            repo = new Repository(db, manager);
            this.db = repo.Db;
        }

        protected async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await repo.GetUserByIdAsync(User.Identity.GetUserId());
        }

        protected ApplicationUser GetCurrentUser()
        {
            return repo.GetUserById(User.Identity.GetUserId());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
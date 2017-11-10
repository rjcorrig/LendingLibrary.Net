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
        protected IApplicationUserManager manager;

        public BaseController()
        {
            var context = new ApplicationDbContext();
            db = context;
            manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
        }

        public BaseController(IApplicationDbContext db, IApplicationUserManager manager)
        {
            this.db = db;
            this.manager = manager;
        }

        protected async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await manager.FindByIdAsync(User.Identity.GetUserId());
        }

        protected ApplicationUser GetCurrentUser()
        {
            return manager.FindById(User.Identity.GetUserId());
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
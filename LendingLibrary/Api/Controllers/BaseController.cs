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
using System.Threading.Tasks;
using System;
using System.Web.Http;

namespace LendingLibrary.Api.Controllers
{
    public class BaseController : ApiController
    {
        protected IApplicationDbContext db;
        protected IRepository repo;
        protected Func<string> GetUserId;

        public BaseController(IApplicationDbContext db)
        {
            repo = new Repository(db);
            this.db = repo.Db;
        }

        public BaseController(IApplicationDbContext db, Func<string> getUserId)
        {
            repo = new Repository(db);
            this.db = repo.Db;

            GetUserId = getUserId;
        }

        protected async Task<ApplicationUser> GetCurrentUserAsync()
        {
            if (GetUserId != null)
            {
                return await repo.GetUserByIdAsync(GetUserId());
            }
            else
            {
                return await repo.GetUserByIdAsync(User.Identity.GetUserId());
            }
        }

        protected ApplicationUser GetCurrentUser()
        {
            if (GetUserId != null)
            {
                return repo.GetUserById(GetUserId());
            }
            else
            {
                return repo.GetUserById(User.Identity.GetUserId());
            }
        }

        protected string GetCurrentUserId()
        {
            if (GetUserId != null)
            {
                return GetUserId();
            }
            else
            {
                return User.Identity.GetUserId();
            }
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
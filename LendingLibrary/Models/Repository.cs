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

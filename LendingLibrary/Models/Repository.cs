﻿/*
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
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
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

        public Repository(IApplicationDbContext db, IApplicationUserManager manager)
        {
            this.Db = db;
            this.Manager = manager;
        }

        #region User
        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            return await Manager.FindByIdAsync(userId);
        }

        public ApplicationUser GetUserById(string userId)
        {
            return Manager.FindById(userId);
        }
        #endregion

        #region Book
        public async Task<Book> GetBookByIdAsync(int? id)
        {
            return await Db.Books.FirstOrDefaultAsync(b => b.ID == id);
        }

        public async Task<IEnumerable<Book>> GetBooksByOwnerId(string userId)
        {
            return await Db.Books.Where(b => b.Owner.Id == userId).ToListAsync();
        }

        public Book Add(Book book)
        {
            return Db.Books.Add(book);
        }

        public Book Remove(Book book)
        {
            return Db.Books.Remove(book);
        }
        #endregion

        #region DbContext
        public async Task<int> SaveAsync()
        {
            return await Db.SaveChangesAsync();
        }

        public void SetModified(object entity)
        {
            Db.Entry(entity).State = EntityState.Modified;
        }
        #endregion
    }
}

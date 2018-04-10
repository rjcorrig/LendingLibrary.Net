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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LendingLibrary.Models
{
    public interface IRepository
    {
        IApplicationDbContext Db { get; }

        #region User
        Task<ApplicationUser> GetUserByIdAsync(string userId);
        ApplicationUser GetUserById(string userId);
		Task<ApplicationUserName> GetUserNameByIdAsync(string userId);

        IQueryable<ApplicationUser> GetUsersUnknownToUser(string userId, int skip, int take);

        #endregion

        #region Book
        Task<Book> GetBookByIdAsync(int? id);
        Task<IEnumerable<Book>> GetBooksByOwnerIdAsync(string userId);

        Book Add(Book book);
        Book Remove(Book book);
        #endregion

        #region Friendship
        IQueryable<Friendship> GetFriendshipsByUserId(string userId);
        IQueryable<Friendship> GetFriendshipsAwaitingApprovalByUserId(string userId);

        Task<Friendship> GetFriendshipBetweenUserIdsAsync(string userId, string friendId);
        Task<FriendshipWithNames> GetFriendshipWithNamesBetweenUserIdsAsync(string userId, string friendId);

        Friendship Add(Friendship friendship);
        Friendship Remove(Friendship friendship);
        #endregion

        #region DbContext
        Task<int> SaveAsync();
        void SetModified(object entity);
        #endregion
    }
}

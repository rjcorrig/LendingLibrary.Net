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

using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using LendingLibrary.Models;
using LendingLibrary.Utils.Extensions;

namespace LendingLibrary.Auth
{
    /// <summary>
    /// Returns an Unauthorized error object as per Microsoft Web API Guidelines
    /// </summary>
    public class ApiAuthorize : AuthorizeAttribute
    {
		protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
		{
			var wrappedError = new UnauthorizedApiError("You must log in to access the API").Wrap();

            actionContext.Response = actionContext.Request.CreateErrorResponse(
                HttpStatusCode.Unauthorized, wrappedError);
		}
	}
}

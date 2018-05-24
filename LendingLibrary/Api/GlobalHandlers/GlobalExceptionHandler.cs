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
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;
using LendingLibrary.Api.Models;
using LendingLibrary.Api.Utils.Extensions;

namespace LendingLibrary.Api.GlobalHandlers
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
        {
            var exception = context.Exception;
            var apiError = new InternalServerApiError(exception);

            // Catches an unhandled exception thrown anywhere in Web Api and wraps it in
            // a WrappedApiError object
            context.Result = new ResponseMessageResult(
                context.Request.CreateErrorResponse(
                    HttpStatusCode.InternalServerError, apiError
                )
            );

            return Task.FromResult(0);
        }
    }
}

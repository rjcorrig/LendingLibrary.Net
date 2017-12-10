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

using NUnit.Framework;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using LendingLibrary.Controllers;

namespace LendingLibrary.Tests.Controllers
{
    [TestFixture()]
    public class BooksControllerTests
    {
        [Test()]
        public async Task TestDetailsView()
        {
            // TODO: Fix
            var controller = new BooksController();
            var result = await controller.Details(32) as ViewResult;
            Assert.AreEqual("Details", result.ViewName);
        }
    }
}

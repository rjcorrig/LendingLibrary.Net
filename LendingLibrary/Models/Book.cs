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

using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace LendingLibrary.Models
{
    public class Book
    {
        public virtual int ID { get; set; }
        [StringLength(13)]
        public virtual string ISBN { get; set; }
        [Required, StringLength(256)]
        public virtual string Title { get; set; }
        [Required, StringLength(256)]
        public virtual string Author { get; set; }
        public virtual string OwnerId { get; set; }
        public virtual ApplicationUser Owner { get; set; }
        [Range(0, 5)]
        public virtual double Rating { get; set; }
        [StringLength(25)]
        public virtual string Genre { get; set; }
    }
}

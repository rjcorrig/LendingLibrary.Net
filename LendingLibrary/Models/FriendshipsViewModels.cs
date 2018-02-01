using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LendingLibrary.Models
{
    public class ApplicationUserNameAndCity
    {
        public string Id { get; set; }
        [Display(Name = "Name")]
        public string UserName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
    }

    public class SearchForNewViewModel
    {
        public bool HasMore;
        public int PageNumber;
        public int UsersPerPage;
        public IEnumerable<ApplicationUserNameAndCity> FriendSuggestions;
    }
}

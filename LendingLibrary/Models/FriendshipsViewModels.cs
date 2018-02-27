using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

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

    public class FriendshipWithNames
    {
        public string UserId { get; set; }
        public string FriendId { get; set; }
        [Display(Name = "Request Sent")]
        public DateTime RequestSent { get; set; }
        [Display(Name = "Request Approved")]
        public DateTime? RequestApproved { get; set; }
        public string UserName { get; set; }
        public string FriendName { get; set; }
    }

    public class SearchForNewViewModel
    {
        public bool HasMore { get; set; }
        public int PageNumber { get; set; }
        [Display(Name = "Show")]
        public int UsersPerPage { get; set; }
        public IEnumerable<SelectListItem> UsersPerPageSelectList { get; set; }
        public IEnumerable<ApplicationUserNameAndCity> FriendSuggestions { get; set; }

        public SearchForNewViewModel()
        {
            UsersPerPageSelectList = new List<SelectListItem>
            {
                new SelectListItem { Text = "5", Value = "5", Selected = true },
                new SelectListItem { Text = "10", Value = "10" },
                new SelectListItem { Text = "25", Value = "25" },
                new SelectListItem { Text = "50", Value = "50" }
            };
        }
    }
}

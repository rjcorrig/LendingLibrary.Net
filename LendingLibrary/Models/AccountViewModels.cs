using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace LendingLibrary.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required, StringLength(60)]
        [Display(Name = "Given Name")]
        public string GivenName { get; set; }

        [StringLength(60)]
        [Display(Name = "Family Name")]
        public string FamilyName { get; set; }

        [StringLength(255)]
        [Display(Name = "About Me")]
        public string About { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Street Address")]
        public string Address1 { get; set; }

        [StringLength(100)]
        [Display(Name = "Street Address 2")]
        public string Address2 { get; set; }

        [Required, StringLength(50)]
        public string City { get; set; }

        [StringLength(30)]
        public string State { get; set; }

        [StringLength(10)]
        [Display(Name = "Zip or Post Code")]
        public string Postal { get; set; }

        [StringLength(30)]
        public string Country { get; set; }

        [Required]
        [Display(Name = "Date of Birth")]
        public DateTime? BirthDate { get; set; }

        public RegisterViewModel()
        {
            // Blank the birth date to eliminate nasty default value on initial view
            BirthDate = null;
        }
    }

    public class EditAccountViewModel
    {
        public string Id { get; set; }

        [Required, StringLength(60)]
        [Display(Name = "Given Name")]
        public string GivenName { get; set; }

        [StringLength(60)]
        [Display(Name = "Family Name")]
        public string FamilyName { get; set; }

        [StringLength(255)]
        [Display(Name = "About Me")]
        public string About { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Street Address")]
        public string Address1 { get; set; }

        [StringLength(100)]
        [Display(Name = "Street Address 2")]
        public string Address2 { get; set; }

        [Required, StringLength(50)]
        public string City { get; set; }

        [StringLength(30)]
        public string State { get; set; }

        [StringLength(10)]
        [Display(Name = "Zip or Post Code")]
        public string Postal { get; set; }

        [StringLength(30)]
        public string Country { get; set; }

        [Required]
        [Display(Name = "Date of Birth")]
        public DateTime BirthDate { get; set; }

        public string PhoneNumber { get; set; }

        public EditAccountViewModel()
        {

        }

        public EditAccountViewModel(ApplicationUser account)
        {
            Id = account.Id;
            GivenName = account.GivenName;
            FamilyName = account.FamilyName;
            About = account.About;
            Email = account.Email;
            Address1 = account.Address1;
            Address2 = account.Address2;
            City = account.City;
            State = account.State;
            Postal = account.Postal;
            Country = account.Country;
            BirthDate = account.BirthDate;
            PhoneNumber = account.PhoneNumber;
        }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}

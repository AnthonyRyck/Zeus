using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using WepAppServer.Data;
using WepAppServer.Services;

namespace WepAppServer.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<LoginModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
	        [Required]
	        [StringLength(20, ErrorMessage = "Le login ne doit pas faire plus de 20 caractères")]
	        [Display(Name = "Login")]
	        public string UserLogin { get; set; }
			
			[Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Mot de passe")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirmer le mot de passe")]
            [Compare("Password", ErrorMessage = "Le mot de passe et la confirmation ne sont pas identique.")]
            public string ConfirmPassword { get; set; }

			[Required]
			[Display(Name = "Type de compte")]
			public TypeAccount AccountType { get; set; }
		}

		public enum TypeAccount
		{
			Member,
			Manager
		}

		public IEnumerable<TypeAccount> TypeAccounts { get; set; } = new List<TypeAccount>() { TypeAccount.Manager, TypeAccount.Member };

		public SelectList SelectListCustom { get; set; }

		public void OnGet(string returnUrl = null)
        {
			SelectListCustom = new SelectList(TypeAccounts);
			ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser { UserName = Input.UserLogin, Email = Input.Email };
                IdentityResult result = await _userManager.CreateAsync(user, Input.Password);
				IdentityResult resultRole = await _userManager.AddToRoleAsync(user, Input.AccountType.ToString());
				
				if (result.Succeeded && resultRole.Succeeded)
                {
                    _logger.LogInformation("Utilisateur créé avec un mot de passe.");

                    //string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //string callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    //await _emailSender.SendEmailConfirmationAsync(Input.Email, callbackUrl);

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(Url.GetLocalUrl(returnUrl));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }
    }

}

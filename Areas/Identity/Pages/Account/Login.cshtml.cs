using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using WorkshopManager.Data;

namespace WorkshopManager.Areas.Identity.Pages.Account
{
    //model strony logowania dla razor pages 
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        //kontruktor obslugujacy logowanie uzytkownika, ilogger logowanie zdarzen itp
        public LoginModel(SignInManager<IdentityUser> signInManager, ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
            Input = new InputModel();
            ErrorMessage = string.Empty;
            ReturnUrl = string.Empty;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        //dane logowania
        public class InputModel
        {
            [Required(ErrorMessage = "Email jest wymagany.")]
            [EmailAddress(ErrorMessage = "Niepoprawny format adresu email.")]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Hasło jest wymagane.")]
            [DataType(DataType.Password)]
            [Display(Name = "Hasło")]
            public string Password { get; set; } = string.Empty;

            [Display(Name = "Zapamiętaj mnie")]
            public bool RememberMe { get; set; }
        }

        //wyswietlenie strony 
        public void OnGet(string? returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            ReturnUrl = returnUrl ?? Url.Content("~/");
        }

        //co sie dzieje po kliknieciu zaloguj 
        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
            
            try
            {
                if (Input == null)
                {
                    _logger.LogWarning("Model Input is null");
                    ModelState.AddModelError(string.Empty, "Błąd formularza. Spróbuj ponownie.");
                    return Page();
                }

                _logger.LogInformation("Login attempt for email: {Email}", Input.Email ?? "null");

                if (!ModelState.IsValid)
                {
                    foreach (var state in ModelState)
                    {
                        foreach (var error in state.Value.Errors)
                        {
                            _logger.LogWarning("Model validation error for {Key}: {Error}",
                                state.Key,
                                string.IsNullOrEmpty(error.ErrorMessage)
                                    ? error.Exception?.Message
                                    : error.ErrorMessage);
                        }
                    }
                    return Page();
                }

                if (string.IsNullOrEmpty(Input.Email))
                {
                    ModelState.AddModelError(nameof(Input.Email), "Email jest wymagany.");
                    return Page();
                }

                if (string.IsNullOrEmpty(Input.Password))
                {
                    ModelState.AddModelError(nameof(Input.Password), "Hasło jest wymagane.");
                    return Page();
                }

                _logger.LogDebug("Starting login attempt for user {Email}", Input.Email);

                var userManager = HttpContext.RequestServices.GetRequiredService<UserManager<IdentityUser>>();
                var user = await userManager.FindByEmailAsync(Input.Email);

                if (user == null)
                {
                    _logger.LogWarning("User not found with email {Email}", Input.Email);
                    ModelState.AddModelError(string.Empty, "Niepoprawny email lub hasło.");
                    return Page();
                }

                _logger.LogDebug("Found user with ID {UserId}, attempting login", user.Id);

                var result = await _signInManager.PasswordSignInAsync(
                    userName: Input.Email,
                    password: Input.Password,
                    isPersistent: Input.RememberMe,
                    lockoutOnFailure: false);

                _logger.LogInformation("Sign in result: {Result}, Succeeded: {Succeeded}, IsLockedOut: {IsLockedOut}, RequiresTwoFactor: {RequiresTwoFactor}",
                    result, result.Succeeded, result.IsLockedOut, result.RequiresTwoFactor);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User {Email} logged in successfully", Input.Email);
                    return LocalRedirect(ReturnUrl);
                }
                
                if (result.RequiresTwoFactor)
                {
                    _logger.LogInformation("Two-factor authentication required for user {Email}", Input.Email);
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = ReturnUrl, RememberMe = Input.RememberMe });
                }
                
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User {Email} account locked out", Input.Email);
                    return RedirectToPage("./Lockout");
                }
                
                _logger.LogWarning("Invalid login attempt for user {Email}", Input.Email);
                ModelState.AddModelError(string.Empty, "Niepoprawny email lub hasło.");
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login process");
                ModelState.AddModelError(string.Empty, "Wystąpił błąd podczas logowania. Spróbuj ponownie.");
                return Page();
            }
        }
    }
}
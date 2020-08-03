using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Amazon.AspNetCore.Identity.Cognito;
using Amazon.Extensions.CognitoAuthentication;
using WebAdvert.Web.Models.Accounts;

namespace WebAdvert.Web.Controllers
{
    public class AccountsController : Controller
    {
        private readonly CognitoSignInManager<CognitoUser> _signInManager;
        private readonly CognitoUserManager<CognitoUser> _userManager;
        private readonly CognitoUserPool _pool;

        public AccountsController(
            SignInManager<CognitoUser> signInManager,
            UserManager<CognitoUser> userManager,
            CognitoUserPool pool)
        {
            _signInManager = signInManager as CognitoSignInManager<CognitoUser>;
            _userManager = userManager as CognitoUserManager<CognitoUser>;
            _pool = pool;
        }

        [HttpGet]
        public IActionResult Signup()
        {
            var model = new SignupModel();
            return View(model);
        }

        [HttpPost]
        [ActionName(nameof(Signup))]
        public async Task<IActionResult> SignupPost(SignupModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _pool.GetUser(model.Email);
                if (user.Status != null)
                {
                    ModelState.AddModelError("UserExists", "User with this email already exists.");
                    return View(model);
                }

                user.Attributes.Add(CognitoAttribute.Name.AttributeName, model.Email);

                var createdUser = await _userManager.CreateAsync(user, model.Password).ConfigureAwait(false);

                if (createdUser.Succeeded)
                {
                    return RedirectToAction(nameof(Confirm), new ConfirmModel { Email = model.Email });
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Confirm(ConfirmModel model)
        {
            return View(model);
        }

        [HttpPost]
        [ActionName(nameof(Confirm))]
        public async Task<IActionResult> ConfirmPost(ConfirmModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email).ConfigureAwait(false);

                if (user == null)
                {
                    ModelState.AddModelError("NotFound", "A user with the given email address was not found.");
                    return View(model);
                }

                var result = await _userManager.ConfirmSignUpAsync(user, model.Code, false).ConfigureAwait(false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }

                    return View(model);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Signin()
        {
            var model = new SigninModel();
            return View(model);
        }

        [HttpPost]
        [ActionName(nameof(Signin))]
        public async  Task<IActionResult> SigninPost(SigninModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    model.RememberMe,
                    false).ConfigureAwait(false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("SigninError", "Email and password do not match.");
                }
            }

            return View(model);
        }
    }
}
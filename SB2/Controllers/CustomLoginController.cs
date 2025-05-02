using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Umbraco.Cms.Web.Common.Security;
using Umbraco.Cms.Web.Common.Models;
using Umbraco.Cms.Core.Security;

public class CustomLoginController : Controller
{
    private readonly SignInManager<MemberIdentityUser> _signInManager;
    private readonly UserManager<MemberIdentityUser> _userManager;

    public CustomLoginController(SignInManager<MemberIdentityUser> signInManager, UserManager<MemberIdentityUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginModel model)
    {
        if (!ModelState.IsValid)
            return View("Login", model);

        var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);

        if (result.Succeeded)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("SalesReps")) /*|| roles.Contains("Admin"))*/
                return Redirect("/sales");
            if (roles.Contains("CompanyProfiles"))
                return Redirect("/costumer");
        }

        ModelState.AddModelError("", "Login failed.");
        return View("Login", model);
    }
}


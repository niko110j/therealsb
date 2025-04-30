using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Cache;
using Microsoft.AspNetCore.Identity;
using SB2.Models.ViewModels;

public class MemberController : SurfaceController
{
    private readonly IMemberManager _memberManager;
    private readonly IMemberService _memberService;
    private readonly SignInManager<MemberIdentityUser> _signInManager;

    public MemberController(
        IUmbracoContextAccessor umbracoContextAccessor,
        IUmbracoDatabaseFactory databaseFactory,
        ServiceContext services,
        AppCaches appCaches,
        IMemberManager memberManager,
        IMemberService memberService,
        IProfilingLogger profilingLogger,
        IPublishedUrlProvider publishedUrlProvider,
        SignInManager<MemberIdentityUser> signInManager
        )
        : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
    {
        _memberManager = memberManager;
        _memberService = memberService;
        _signInManager = signInManager;
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return CurrentUmbracoPage();

        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, true, true);

        if (result.Succeeded)
        {
            var identityUser = await _memberManager.GetCurrentMemberAsync();
            var roles = await _memberManager.GetRolesAsync(identityUser);

            if (roles.Contains("SalesReps"))
                return Redirect("/landingpage");

            if (roles.Contains("CompanyProfiles"))
                return Redirect("/kundehub");
        }

        ModelState.AddModelError("", "Invalid login");
        return CurrentUmbracoPage();
    }
}



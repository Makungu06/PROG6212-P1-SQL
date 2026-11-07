
using Microsoft.AspNetCore.Mvc;

namespace P3_RaceDay.MVC.Controllers;

public class OrganiserController : Controller
{
    private bool IsOrganiser()
    {
        return HttpContext.Session.GetString("UserRole") == "Organiser";
    }

    private IActionResult CheckOrganiser()
    {
        if (!IsOrganiser())
        {
            return RedirectToAction("Login", "Account");
        }

        return View();
    }

    public IActionResult Index()
    {
        return CheckOrganiser();
    }

    public IActionResult Events()
    {
        return CheckOrganiser();
    }

    public IActionResult Categories()
    {
        return CheckOrganiser();
    }

    public IActionResult Enrolments()
    {
        return CheckOrganiser();
    }

    public IActionResult Results()
    {
        return CheckOrganiser();
    }
}

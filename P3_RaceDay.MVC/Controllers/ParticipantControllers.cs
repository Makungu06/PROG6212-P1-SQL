
using Microsoft.AspNetCore.Mvc;

namespace P3_RaceDay.MVC.Controllers;

public class ParticipantController : Controller
{
    private bool IsParticipant()
    {
        return HttpContext.Session.GetString("UserRole") == "Participant";
    }

    private IActionResult CheckParticipant()
    {
        if (!IsParticipant())
        {
            return RedirectToAction("Login", "Account");
        }

        return View();
    }

    public IActionResult Index()
    {
        return CheckParticipant();
    }

    public IActionResult MyEvents()
    {
        return CheckParticipant();
    }

    public IActionResult MyResults()
    {
        return CheckParticipant();
    }

    public IActionResult Profile()
    {
        return CheckParticipant();
    }
}

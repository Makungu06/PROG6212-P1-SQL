
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using P3_RaceDay.MVC.Services;
using P3_RaceDay.MVC.Controllers;

namespace P3_RaceDay.MVC.Controllers;

public class OrganiserController : Controller
{
    private readonly ApiService _apiService;

    public OrganiserController(ApiService apiService)
    {
        _apiService = apiService;
    }

    // Check whether the logged-in user is an Organiser
    private bool IsOrganiser()
    {
        return HttpContext.Session.GetString("UserRole") == "Organiser";
    }

    // Organiser Dashboard
    public IActionResult Index()
    {
        if (!IsOrganiser())
            return RedirectToAction("Login", "Account");

        return View();
    }

    // Display all race events
    public async Task<IActionResult> Events()
    {
        if (!IsOrganiser())
            return RedirectToAction("Login", "Account");

        try
        {
            var events = await _apiService.GetAsync<List<EventViewModel>>(
                "api/events");

            return View(events ?? new List<EventViewModel>());
        }
        catch (HttpRequestException ex)
        {
            ViewBag.Error = "Unable to load events: " + ex.Message;
            return View(new List<EventViewModel>());
        }
    }

    // Display the Create Event page
    public IActionResult Create()
    {
        if (!IsOrganiser())
            return RedirectToAction("Login", "Account");

        return View();
    }

    // Submit a new event to the API
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EventViewModel model)
    {
        if (!IsOrganiser())
            return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var token = HttpContext.Session.GetString("JwtToken");

            await _apiService.PostAsync<EventViewModel, System.Text.Json.JsonElement>(
                "api/events", model, token);

            TempData["SuccessMessage"] = "Event created successfully!";

            return RedirectToAction("Events");
        }
        catch (HttpRequestException ex)
        {
            ModelState.AddModelError(
                "", "Could not create event: " + ex.Message);

            return View(model);
        }
    }

    // Categories page
    public IActionResult Categories()
    {
        if (!IsOrganiser())
            return RedirectToAction("Login", "Account");

        return View();
    }

    // Enrolments page
    public IActionResult Enrolments()
    {
        if (!IsOrganiser())
            return RedirectToAction("Login", "Account");

        return View();
    }

    // Results page
    public IActionResult Results()
    {
        if (!IsOrganiser())
            return RedirectToAction("Login", "Account");

        return View();
    }
}

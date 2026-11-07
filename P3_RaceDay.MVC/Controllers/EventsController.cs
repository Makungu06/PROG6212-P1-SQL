
using Microsoft.AspNetCore.Mvc;
using P3_RaceDay.MVC.Services;

namespace P3_RaceDay.MVC.Controllers;

public class EventsController : Controller
{
    private readonly ApiService _apiService;

    public EventsController(ApiService apiService)
    {
        _apiService = apiService;
    }

    private bool IsParticipant()
    {
        return HttpContext.Session.GetString("UserRole") == "Participant";
    }

    public async Task<IActionResult> Index()
    {
        if (!IsParticipant())
            return RedirectToAction("Login", "Account");

        try
        {
            var events = await _apiService.GetAsync<List<EventViewModel>>(
                "api/events");

            return View(events ?? new List<EventViewModel>());
        }
        catch (HttpRequestException ex)
        {
            ViewBag.Error = "Could not load events: " + ex.Message;
            return View(new List<EventViewModel>());
        }
    }

    public async Task<IActionResult> Details(int id)
    {
        if (!IsParticipant())
            return RedirectToAction("Login", "Account");

        try
        {
            var eventItem = await _apiService.GetAsync<EventViewModel>(
                $"api/events/{id}");

            if (eventItem == null)
                return NotFound();

            return View(eventItem);
        }
        catch (HttpRequestException)
        {
            return NotFound();
        }
    }

    public async Task<IActionResult> Enter(int id)
    {
        if (!IsParticipant())
            return RedirectToAction("Login", "Account");

        try
        {
            var token = HttpContext.Session.GetString("JwtToken");

            await _apiService.PostAsync<object, System.Text.Json.JsonElement>(
                $"api/events/{id}/enrolments",
                new { },
                token);

            TempData["SuccessMessage"] = "You have successfully entered the event!";
            return RedirectToAction("Index", "Participant");
        }
        catch (HttpRequestException ex)
        {
            TempData["ErrorMessage"] = "Event entry failed: " + ex.Message;
            return RedirectToAction("Details", new { id });
        }
    }
}

public class EventViewModel
{
    public int EventID { get; set; }
    public string EventName { get; set; } = "";
    public DateTime EventDate { get; set; }
    public string Venue { get; set; } = "";
    public string Location { get; set; } = "";
    public string Description { get; set; } = "";
}


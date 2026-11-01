
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using P3_RaceDay.MVC.Models.DTOs;
using P3_RaceDay.MVC.Services;

namespace P3_RaceDay.MVC.Controllers;

public class AccountController : Controller
{
    private readonly ApiService _apiService;

    public AccountController(ApiService apiService)
    {
        _apiService = apiService;
    }

    // GET: Account/Login
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // POST: Account/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        try
        {
            var response = await _apiService.PostAsync
                <LoginDto, LoginResponseDto>(
                    "api/auth/login", dto);

            if (response == null ||
                string.IsNullOrWhiteSpace(response.Token) ||
                response.User == null)
            {
                ModelState.AddModelError(
                    "", "The API returned an invalid login response.");

                return View(dto);
            }

            // Store login information in Session
            HttpContext.Session.SetString(
                "JwtToken", response.Token);

            HttpContext.Session.SetString(
                "UserID", response.User.UserID.ToString());

            HttpContext.Session.SetString(
                "UserEmail", response.User.Email);

            HttpContext.Session.SetString(
                "FullName", response.User.FullName);

            HttpContext.Session.SetString(
                "UserRole", response.User.Role);

            // Redirect according to the user's role
            if (response.User.Role == "Organiser")
            {
                return RedirectToAction(
                    "Index", "Organiser");
            }

            if (response.User.Role == "Participant")
            {
                return RedirectToAction(
                    "Index", "Events");
            }

            HttpContext.Session.Clear();

            ModelState.AddModelError(
                "", "Your account has an unsupported role.");

            return View(dto);
        }
        catch (HttpRequestException ex)
        {
            ModelState.AddModelError(
                "", $"Login failed: {ex.Message}");

            return View(dto);
        }
        catch (TaskCanceledException)
        {
            ModelState.AddModelError(
                "", "The API request timed out. Please try again.");

            return View(dto);
        }
    }

    // GET: Account/Register
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    // POST: Account/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(
        RegistrationDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        if (dto.Role != "Organiser" &&
            dto.Role != "Participant")
        {
            ModelState.AddModelError(
                "Role", "Please select a valid role.");

            return View(dto);
        }

        try
        {
            await _apiService.PostAsync
                <RegistrationDto, JsonElement>(
                    "api/auth/register", dto);

            TempData["SuccessMessage"] =
                "Registration successful! You can now log in.";

            return RedirectToAction("Login");
        }
        catch (HttpRequestException ex)
        {
            ModelState.AddModelError(
                "", $"Registration failed: {ex.Message}");

            return View(dto);
        }
        catch (TaskCanceledException)
        {
            ModelState.AddModelError(
                "", "The API request timed out. Please try again.");

            return View(dto);
        }
    }

    // POST: Account/Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();

        return RedirectToAction("Login");
    }
}

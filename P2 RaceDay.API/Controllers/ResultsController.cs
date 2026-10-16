using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P2_RaceDay.API.Data;
using P2_RaceDay.API.Models;

namespace P2_RaceDay.API.Controllers;

public class ResultDto
{
    public int EntryID { get; set; }
    public TimeSpan? FinishTime { get; set; }
    public int? Position { get; set; }
    public string RaceStatus { get; set; } = "Finished";
}

public class UpdateResultDto
{
    public TimeSpan? FinishTime { get; set; }
    public int? Position { get; set; }
    public string RaceStatus { get; set; } = "Finished";
}

[ApiController]
[Route("api/results")]

public class ResultsController : ControllerBase
{

}

    
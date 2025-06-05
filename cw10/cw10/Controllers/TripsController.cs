using cw10.DTOs;
using cw10.Exceptions;
using cw10.Models;
using cw10.Services;
using Microsoft.AspNetCore.Mvc;

namespace cw10.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripsController(IDbService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> GetTripsAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var trips = await service.GetTripsAsync(page, pageSize);
            return Ok(trips);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost("{idTrip}/clients")]
    public async Task<IActionResult> AddClientTrip([FromRoute] int idt, [FromBody] ClientTripPostDto ct)
    {
        try
        {
            await service.JoinClientTripAsync(idt, ct);
            return Ok("Client added to a trip");
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(e.Message);
        }
        catch (ArgumentNullException e)
        {
            return NotFound(e.Message);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}
using cw10.Services;
using Microsoft.AspNetCore.Mvc;

namespace cw10.Controllers;


[ApiController]
[Route("api/[controller]")]
public class ClientsController(IDbService service) : ControllerBase
{
    [HttpDelete("{idClient}")]
    public async Task<IActionResult> RemoveClient(int idClient)
    {
        try
        {
            await service.DeleteClientAsync(idClient);
            return NoContent();
        }
        catch (ArgumentException e)
        {
            return NotFound(e.Message);
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(e.Message);
        }
    }
    
}
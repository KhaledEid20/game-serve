using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/[controller]")]
public class SessionController : ControllerBase
{

    private IUnitOfWork _unitOfWork;

    public SessionController(IUnitOfWork unitOfWork , ILogger<SessionController> logger)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpPost("[action]")]  
    public async Task<IActionResult> CreateSession(Session session)
    {
        var result = await _unitOfWork.Sessions.createSession(session);
        if (result.Success)
        {
            return Ok(result.Success);
        }
        return BadRequest(result.Success);
    }
    
    [HttpGet("[action]")]  
    public async Task<IActionResult> GetAllSessions()
    {
        var result = await _unitOfWork.Sessions.getAllSession();
        if (result.Success)
        {
            return Ok(result.data);
        }
        return BadRequest(result.Success);
    }
}
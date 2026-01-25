using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/[controller]")]
public class PlayerController : ControllerBase
{

    private IUnitOfWork _unitOfWork;

    public PlayerController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> SignUp(Player player)
    {
        var result = await _unitOfWork.Players.addPlayer(player);
        if (result.Success)
        {
            return Ok(result.Success);
        }
        return BadRequest(result.Success);
    }
    
    [HttpPost("[action]")]
    public async Task<IActionResult> SignIn(string id)
    {
        var result = await _unitOfWork.Players.SignInPlayer(id);
        if (result.Success)
        {
            return Ok(result.Success);
        }
        return BadRequest(result);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetAllUser()
    {
        var result = await _unitOfWork.Players.GetAllUser();
        if (result.Success)
        {
            return Ok(result.data);
        }
        return BadRequest(result);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> JoinRoom()
    {
        var result = await _unitOfWork.Players.JoinRoom();
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

}
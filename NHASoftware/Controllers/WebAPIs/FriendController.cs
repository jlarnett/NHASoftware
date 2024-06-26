﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NHA.Website.Software.ConsumableEntities.DTOs;
using NHA.Website.Software.DBContext;
using NHA.Website.Software.Entities.FriendSystem;
using NHA.Website.Software.Services.FriendSystem;
namespace NHA.Website.Software.Controllers.WebAPIs;
[Route("api/[controller]")]
[ApiController]
public class FriendController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IFriendSystem _friendSystem;
    private readonly IMapper _mapper;
    private readonly ILogger<ILogger> _logger;

    public FriendController(ApplicationDbContext context, IFriendSystem friendSystem, IMapper mapper, ILogger<ILogger> logger)
    {
        _context = context;
        _friendSystem = friendSystem;
        _mapper = mapper;
        _logger = logger;
    }

    // GET: api/FriendRequests
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FriendRequest>>> GetFriendRequests()
    {
        return await _context.FriendRequests!.ToListAsync();
    }

    // GET: api/FriendRequests/5
    [HttpGet("{id}")]
    public async Task<ActionResult<FriendRequest>> GetFriendRequest(int id)
    {
        var friendRequest = await _context.FriendRequests!.FindAsync(id);

        if (friendRequest == null)
        {
            return NotFound();
        }

        return friendRequest;
    }

    // PUT: api/FriendRequests/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutFriendRequest(int id, FriendRequest friendRequest)
    {
        if (id != friendRequest.Id)
        {
            return BadRequest();
        }

        _context.Entry(friendRequest).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!FriendRequestExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/FriendRequests
    /// <summary>
    /// Creates the friend request between the desired users.
    /// </summary>
    /// <param name="friendRequestDto">friendRequestDTO containing both sender & recipient</param>
    /// <returns></returns>
    [HttpPost("FriendRequest")]
    public async Task<ActionResult<FriendRequest>> PostFriendRequest(FriendRequestDTO friendRequestDto)
    {
        //Checks if friend request is trying to send to the same user
        if (friendRequestDto.RecipientUserId.Equals(friendRequestDto.SenderUserId))
        {
            return BadRequest(new { success = false });
        }


        friendRequestDto.Status = FriendRequestStatuses.Inprogress;
        var requestSent = await _friendSystem.SendFriendRequestAsync(friendRequestDto);
        return requestSent ? new JsonResult(new { success = true }) : new JsonResult(new { success = false });
    }

    /// <summary>
    /// Returns In progress friend requests for the supplied userId
    /// </summary>
    /// <param name="userId">recipient userId</param>
    /// <returns></returns>
    [HttpGet("PendingFriendRequest/{userId}")]
    public async Task<ActionResult<IEnumerable<FriendRequestDTO>>> PendingFriendRequest(string userId)
    {
        if (!userId.Equals(string.Empty))
        {
            return Ok(await _friendSystem.GetPendingFriendRequestsAsync(userId));
        }
        else
        {
            return BadRequest();
        }
    }

    /// <summary>
    /// Accept friend request endpoint. Accepts the supplied friend request id.
    /// Creates entry into Friends Table & modifies friend request status to ACP
    /// </summary>
    /// <param name="requestId"></param>
    /// <returns></returns>
    [HttpGet("AcceptFriendRequest/{requestId}")]
    public async Task<JsonResult> AcceptFriendRequest(int requestId)
    {
        var result = await _friendSystem.AcceptFriendRequestAsync(requestId);
        return result ? new JsonResult(new { success = true }) : new JsonResult(new { success = false });
    }

    /// <summary>
    /// Declines friend request. Accepts the supplied friend request id.
    /// Changes the friend request status to declined for the supplied friend request id. 
    /// </summary>
    /// <param name="requestId">friend request id</param>
    /// <returns></returns>
    [HttpGet("DeclineFriendRequest/{requestId}")]
    public async Task<JsonResult> DeclineFriendRequest(int requestId)
    {
        var result = await _friendSystem.DeclineFriendRequestAsync(requestId);
        return result ? new JsonResult(new { success = true }) : new JsonResult(new { success = false });
    }

    /// <summary>
    /// DELETE: api/Friend/FriendRequests/5
    /// Deletes friend request from DB. 
    /// </summary>
    /// <param name="id">FriendRequestId</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFriendRequest(int id)
    {
        var result = await _friendSystem.DeleteFriendRequestAsync(id);

        if (result)
        {
            return Ok();
        }

        return BadRequest();
    }

    /// <summary>
    /// DELETE: api/friend/RemoveFriendship
    /// Tries to remove pair of friends from DB. 
    /// </summary>
    /// <param name="friendRequestDto">Friend Request DTO containing recipientId & senderId</param>
    /// <returns>Returns IActionResult with JSON success result.</returns>
    [HttpDelete("DeleteFriendship")]
    public async Task<IActionResult> DeleteFriendship(FriendRequestDTO friendRequestDto)
    {
        var result = await _friendSystem.RemoveFriendsAsync(friendRequestDto);

        if (result)
        {
            return Ok(new { success = true });
        }

        return BadRequest(new { success = false });
    }

    /// <summary>
    /// PUT: api/friend/CancelFriendRequest
    /// Tries to cancels pending friend request.
    /// </summary>
    /// <param name="friendRequestDto">Friend Request DTO containing recipientId & senderId</param>
    /// <returns>Returns IActionResult with JSON success result. </returns>
    [HttpPut("CancelFriendRequest")]
    public async Task<IActionResult> CancelFriendRequest(FriendRequestDTO friendRequestDto)
    {
        var result = await _friendSystem.CancelFriendRequestAsync(friendRequestDto);

        if (result)
        {
            return Ok(new { success = true });
        }

        return BadRequest(new { success = false });
    }

    /// <summary>
    /// GET: api/friend/RetrieveMutualFriends
    /// Retrieves full list of mutual friends users from DB. Requires UserId of both ApplicationUsers
    /// </summary>
    /// <param name="friendRequestDto">Friend Request DTO containing recipientId & senderId</param>
    /// <returns>Returns IActionResult with JSON success result. </returns>
    [HttpGet("RetrieveMutualFriends")]
    public async Task<IActionResult> RetrieveMutualFriends(FriendRequestDTO friendRequestDto)
    {
        var mutualFriends = await _friendSystem.GetMutualFriendsAsync(friendRequestDto.RecipientUserId, friendRequestDto.SenderUserId);
        return mutualFriends.Count != 0 ? Ok(new { success = true, data = mutualFriends }) : NoContent();
    }

    /// <summary>
    /// GET: api/friend/RetrieveFriends
    /// Retrieves full list of friends associated with user. 
    /// </summary>
    /// <param name="userId">UserId you want to retrieve friend list for</param>
    /// <returns>Returns IActionResult with JSON success result. </returns>
    [HttpGet("RetrieveFriends")]
    public async Task<IActionResult> RetrieveFriends(string userId)
    {
        var friendList = await _friendSystem.GetUsersFriendListAsync(userId);
        return friendList.Count != 0 ? Ok(new { success = true, data = friendList }) : NoContent();
    }

    private bool FriendRequestExists(int id)
    {
        return _context.FriendRequests!.Any(e => e.Id == id);
    }
}

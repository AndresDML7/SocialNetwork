using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Application.Common;
using SocialNetwork.Application.DTO;
using SocialNetwork.Application.Interfaces;

namespace SocialNetwork.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("post")]
        public IActionResult PostMessage([FromBody] MessageDto request)
        {
            _userService.PostMessage(request.Username, request.Content, request.Timestamp);
            return Ok();
        }

        [HttpPost("{followerUsername}/follow/{followeeUsername}")]
        public IActionResult FollowUser(string followerUsername, string followeeUsername)
        {
            try
            {
                _userService.FollowUser(followerUsername, followeeUsername);
                return Ok();
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UserAlreadyFollowedException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{username}/dashboard")]
        public ActionResult<IEnumerable<MessageDto>> GetDashboard(string username)
        {
            try
            {
                var dashboard = _userService.GetDashboard(username);
                return Ok(dashboard);
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsocketBackend.data;

namespace WebsocketBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public MemberController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Get the current member count
        [HttpGet("count")]
        public async Task<IActionResult> GetMemberCount()
        {
            int count = await _dbContext.Members.CountAsync();
            return Ok(new { count });
        }

        // add a new member 
        [HttpPost("add")]
        public async Task<IActionResult> AddMember()
        {
            _dbContext.Members.Add(new Member { Name = "New Member" });
            await _dbContext.SaveChangesAsync();
            return Ok(new { message = "Member added successfully!" });
        }
    }
}

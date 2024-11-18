using Backend_Multifinance.Data;
using Backend_Multifinance.Models.MasterDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Backend_Multifinance.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MsUserController : ControllerBase
    {
        private readonly MasterDBContext _context;

        public MsUserController(MasterDBContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ms_user>> Login([FromBody] LoginModel loginModel)
        {
            var msUser = await _context.ms_users
                .FirstOrDefaultAsync(u => u.user_name == loginModel.Username);

            if (msUser == null || !VerifyPassword(msUser.password, loginModel.Password))
            {
                return Unauthorized();
            }

            if (msUser.is_active != true)
            {
                return Forbid();
            }

            return Ok(msUser);
        }

        private bool VerifyPassword(string storedPassword, string enteredPassword)
        {
            using (var sha256 = SHA256.Create())
            {
                var enteredPasswordHash = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(enteredPassword)));
                return storedPassword == enteredPassword;//enteredPasswordHash;
            }
        }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}

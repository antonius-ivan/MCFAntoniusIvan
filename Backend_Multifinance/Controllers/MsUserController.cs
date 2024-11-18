using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend_Multifinance.Data;
using Backend_Multifinance.Models.MasterDB;

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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ms_user>>> GetMsUsers()
        {
            return await _context.ms_users.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ms_user>> GetMsUser(long id)
        {
            var msUser = await _context.ms_users.FindAsync(id);

            if (msUser == null)
            {
                return NotFound();
            }

            return msUser;
        }

        [HttpPost]
        public async Task<ActionResult<ms_user>> PostMsUser(ms_user msUser)
        {
            _context.ms_users.Add(msUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMsUser), new { id = msUser.user_id }, msUser);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutMsUser(long id, ms_user msUser)
        {
            if (id != msUser.user_id)
            {
                return BadRequest();
            }

            _context.Entry(msUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MsUserExists(id))
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMsUser(long id)
        {
            var msUser = await _context.ms_users.FindAsync(id);
            if (msUser == null)
            {
                return NotFound();
            }

            _context.ms_users.Remove(msUser);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MsUserExists(long id)
        {
            return _context.ms_users.Any(e => e.user_id == id);
        }
    }
}

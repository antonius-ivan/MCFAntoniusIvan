using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend_Multifinance.Models.MasterDB;
using Backend_Multifinance.Data;

namespace Backend_Multifinance.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StorageLocationController : ControllerBase
    {
        private readonly MasterDBContext _context;

        public StorageLocationController(MasterDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ms_storage_location>>> GetStorageLocations()
        {
            return await _context.ms_storage_locations.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ms_storage_location>> GetStorageLocation(string id)
        {
            var storageLocation = await _context.ms_storage_locations.FindAsync(id);

            if (storageLocation == null)
            {
                return NotFound();
            }

            return storageLocation;
        }

        [HttpPost]
        public async Task<ActionResult<ms_storage_location>> PostStorageLocation(ms_storage_location storageLocation)
        {
            _context.ms_storage_locations.Add(storageLocation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStorageLocation), new { id = storageLocation.location_id }, storageLocation);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutStorageLocation(string id, ms_storage_location storageLocation)
        {
            if (id != storageLocation.location_id)
            {
                return BadRequest();
            }

            _context.Entry(storageLocation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StorageLocationExists(id))
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
        public async Task<IActionResult> DeleteStorageLocation(string id)
        {
            var storageLocation = await _context.ms_storage_locations.FindAsync(id);
            if (storageLocation == null)
            {
                return NotFound();
            }

            _context.ms_storage_locations.Remove(storageLocation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StorageLocationExists(string id)
        {
            return _context.ms_storage_locations.Any(e => e.location_id == id);
        }
    }
}

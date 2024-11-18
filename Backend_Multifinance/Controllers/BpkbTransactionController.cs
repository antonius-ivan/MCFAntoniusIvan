using Backend_Multifinance.Data;
using Backend_Multifinance.Models.TransactionDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_Multifinance.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BpkbTransactionController : ControllerBase
    {
        private readonly TransactionDBContext _context;

        public BpkbTransactionController(TransactionDBContext context)
        {
            _context = context;
        }

        // GET: api/BpkbTransaction
        [HttpGet]
        public async Task<ActionResult<IEnumerable<tr_bpkb>>> GetBpkbTransactions()
        {
            return await _context.tr_bpkbs.ToListAsync();
        }

        // GET: api/BpkbTransaction/{id}
        [HttpGet("{agreementNumber}")]
        public async Task<ActionResult<tr_bpkb>> GetBpkbTransaction(string agreementNumber)
        {
            var transaction = await _context.tr_bpkbs.FindAsync(agreementNumber);

            if (transaction == null)
            {
                return NotFound();
            }

            return transaction;
        }

        // POST: api/BpkbTransaction
        [HttpPost]
        public async Task<ActionResult<tr_bpkb>> CreateBpkbTransaction(tr_bpkb transaction)
        {
            // Set the created time and created by if necessary
            transaction.created_on = DateTime.Now;
            transaction.created_by = "System"; // Or use the current user

            _context.tr_bpkbs.Add(transaction);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBpkbTransaction), new { agreementNumber = transaction.agreement_number }, transaction);
        }

        // PUT: api/BpkbTransaction/{id}
        [HttpPut("{agreementNumber}")]
        public async Task<IActionResult> UpdateBpkbTransaction(string agreementNumber, tr_bpkb transaction)
        {
            if (agreementNumber != transaction.agreement_number)
            {
                return BadRequest();
            }

            // Update the modified time and last updated by if necessary
            transaction.last_updated_on = DateTime.Now;
            transaction.last_updated_by = "System"; // Or use the current user

            _context.Entry(transaction).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BpkbTransactionExists(agreementNumber))
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

        // DELETE: api/BpkbTransaction/{id}
        [HttpDelete("{agreementNumber}")]
        public async Task<IActionResult> DeleteBpkbTransaction(string agreementNumber)
        {
            var transaction = await _context.tr_bpkbs.FindAsync(agreementNumber);
            if (transaction == null)
            {
                return NotFound();
            }

            _context.tr_bpkbs.Remove(transaction);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BpkbTransactionExists(string agreementNumber)
        {
            return _context.tr_bpkbs.Any(e => e.agreement_number == agreementNumber);
        }
    }
}

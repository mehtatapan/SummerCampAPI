using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SummerCampAPI.Data;
using SummerCampAPI.Models;

namespace SummerCampAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompoundsController : ControllerBase
    {
        private readonly CampWebApiContext _context;

        public CompoundsController(CampWebApiContext context)
        {
            _context = context;
        }

        // GET: api/Compounds
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompoundDTO>>> GetCompounds()
        {
            return await _context.Compounds
                .Select(c => new CompoundDTO
                {
                    ID = c.ID,
                    Name = c.Name,
                    RowVersion = c.RowVersion
                })
                .ToListAsync();
        }

        // GET: api/Compounds/inc - Include the Campers Collection
        [HttpGet("inc")]
        public async Task<ActionResult<IEnumerable<CompoundDTO>>> GetCompoundsInc()
        {
            return await _context.Compounds
                .Include(d => d.Campers)
                .Select(d => new CompoundDTO
                {
                    ID = d.ID,
                    Name = d.Name,
                    RowVersion = d.RowVersion,
                    Campers = d.Campers.Select(dCamper => new CamperDTO
                    {
                        ID = dCamper.ID,
                        FirstName = dCamper.FirstName,
                        MiddleName = dCamper.MiddleName,
                        LastName = dCamper.LastName,
                        DOB = dCamper.DOB,
                        Gender = dCamper.Gender,
                        EMail = dCamper.EMail,
                        Phone = dCamper.Phone
                    }).ToList()
                })
                .ToListAsync();
        }

        // GET: api/Compounds/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CompoundDTO>> GetCompound(int id)
        {
            var compound = await _context.Compounds
                .Select(c => new CompoundDTO
                {
                    ID = c.ID,
                    Name = c.Name,
                    RowVersion = c.RowVersion
                })
                .FirstOrDefaultAsync(p => p.ID == id);

            if (compound == null)
            {
                return NotFound();
            }

            return compound;
        }

        // GET: api/Compounds/inc/5
        [HttpGet("inc/{id}")]
        public async Task<ActionResult<CompoundDTO>> GetCompoundInc(int id)
        {
            var doctorDTO = await _context.Compounds
                .Include(d => d.Campers)
                .Select(d => new CompoundDTO
                {
                    ID = d.ID,
                    Name = d.Name,
                    RowVersion = d.RowVersion,
                    Campers = d.Campers.Select(dCamper => new CamperDTO
                    {
                        ID = dCamper.ID,
                        FirstName = dCamper.FirstName,
                        MiddleName = dCamper.MiddleName,
                        LastName = dCamper.LastName,
                        DOB = dCamper.DOB,
                        Gender = dCamper.Gender,
                        EMail = dCamper.EMail,
                        Phone = dCamper.Phone
                    }).ToList()
                })
                .FirstOrDefaultAsync(p => p.ID == id);

            if (doctorDTO == null)
            {
                return NotFound(new { message = "Error: Doctor not found." });
            }

            return doctorDTO;
        }

        // PUT: api/Compounds/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompound(int id, CompoundDTO compoundDTO)
        {
            if (id != compoundDTO.ID)
            {
                return BadRequest(new { message = "Error: Incorrect ID for Compound." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Get the record you want to update
            var compoundToUpdate = await _context.Compounds.FindAsync(id);

            //Check that you got it
            if (compoundToUpdate == null)
            {
                return NotFound(new { message = "Error: Compound record not found." });
            }

            //Wow, we have a chance to check for concurrency even before bothering
            //the database!  Of course, it will get checked again in the database just in case
            //it changes after we pulled the record.  
            //Note using SequenceEqual becuase it is an array after all.
            if (compoundDTO.RowVersion != null)
            {
                if (!compoundToUpdate.RowVersion.SequenceEqual(compoundDTO.RowVersion))
                {
                    return Conflict(new { message = "Concurrency Error: Doctor has been changed by another user.  Back out and try editing the record again." });
                }
            }

            compoundToUpdate.ID = compoundDTO.ID;
            compoundToUpdate.Name = compoundDTO.Name;

            //Put the original RowVersion value in the OriginalValues collection for the entity
            _context.Entry(compoundToUpdate).Property("RowVersion").OriginalValue = compoundDTO.RowVersion;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompoundExists(id))
                {
                    return Conflict(new { message = "Concurrency Error: Compound has been Removed." });
                }
                else
                {
                    return Conflict(new { message = "Concurrency Error: Compound has been updated by another user.  Back out and try editing the record again." });
                }
            }
        }

        // POST: api/Compounds
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Compound>> PostCompound(CompoundDTO compoundDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Compound compound = new Compound
            {
                ID = compoundDTO.ID,
                Name = compoundDTO.Name
            };

            try
            {
                _context.Compounds.Add(compound);
                await _context.SaveChangesAsync();
                //Assign Database Generated values back into the DTO
                compoundDTO.ID = compound.ID;
                compoundDTO.RowVersion = compound.RowVersion;
                return CreatedAtAction(nameof(GetCompound), new { id = compound.ID }, compoundDTO);
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { message = "Unable to save changes to the database. Try again, and if the problem persists see your system administrator." });
            }
        }

        // DELETE: api/Compounds/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Compound>> DeleteCompound(int id)
        {
            var compound = await _context.Compounds.FindAsync(id);
            if (compound == null)
            {
                return NotFound(new { message = "Delete Error: Compound has already been removed." });
            }

            try
            {
                _context.Compounds.Remove(compound);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
                {
                    return BadRequest(new { message = "Delete Error: Remember, you cannot delete a Compound that has campers assigned." });
                }
                else
                {
                    return BadRequest(new { message = "Delete Error: Unable to delete Compound. Try again, and if the problem persists see your system administrator." });
                }
            }
        }

        private bool CompoundExists(int id)
        {
            return _context.Compounds.Any(e => e.ID == id);
        }
    }
}

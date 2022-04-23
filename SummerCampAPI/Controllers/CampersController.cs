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
    public class CampersController : ControllerBase
    {
        private readonly CampWebApiContext _context;

        public CampersController(CampWebApiContext context)
        {
            _context = context;
        }

        // GET: api/Campers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CamperDTO>>> GetCampers()
        {
            return await _context.Campers
                .Include(p=>p.Compound)
                .Select(p=>new CamperDTO
                {
                    ID = p.ID,
                    FirstName = p.FirstName,
                    MiddleName = p.MiddleName,
                    LastName = p.LastName,
                    DOB = p.DOB,
                    Gender = p.Gender,
                    EMail = p.EMail,
                    Phone = p.Phone,
                    RowVersion = p.RowVersion,
                    CompoundID = p.CompoundID,
                    Compound = new CompoundDTO
                    {
                        ID = p.Compound.ID,
                        Name = p.Compound.Name
                    }
                })
                .ToListAsync();
        }

        // GET: api/Campers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CamperDTO>> GetCamper(int id)
        {
            var camper = await _context.Campers
                .Include(p => p.Compound)
                .Select(p => new CamperDTO
                {
                    ID = p.ID,
                    FirstName = p.FirstName,
                    MiddleName = p.MiddleName,
                    LastName = p.LastName,
                    DOB = p.DOB,
                    Gender = p.Gender,
                    EMail = p.EMail,
                    Phone = p.Phone,
                    RowVersion = p.RowVersion,
                    CompoundID = p.CompoundID,
                    Compound = new CompoundDTO
                    {
                        ID = p.Compound.ID,
                        Name = p.Compound.Name
                    }
                })
                .FirstOrDefaultAsync(p => p.ID == id);

            if (camper == null)
            {
                return NotFound();
            }

            return camper;
        }

        // GET: api/CampersByCompound
        [HttpGet("ByCompound/{id}")]
        public async Task<ActionResult<IEnumerable<CamperDTO>>> GetCampersByCompound(int id)
        {
            return await _context.Campers
                .Select(p => new CamperDTO
                {
                    ID = p.ID,
                    FirstName = p.FirstName,
                    MiddleName = p.MiddleName,
                    LastName = p.LastName,
                    DOB = p.DOB,
                    Gender = p.Gender,
                    EMail = p.EMail,
                    Phone = p.Phone,
                    RowVersion = p.RowVersion,
                    CompoundID = p.CompoundID,
                    Compound = new CompoundDTO
                    {
                        ID = p.Compound.ID,
                        Name = p.Compound.Name
                    }
                })
                .Include(e => e.Compound)
                .Where(e => e.CompoundID == id).ToListAsync();
        }

        // PUT: api/Campers/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCamper(int id, CamperDTO camperDTO)
        {
            if (id != camperDTO.ID)
            {
                return BadRequest(new { message = "Error: ID does not match any Camper"});
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Get the record you want to update
            var camperToUpdate = await _context.Campers.FindAsync(id);

            //Check that you got it
            if (camperToUpdate == null)
            {
                return NotFound(new { message = "Error: Camper record not found." });
            }

            //Wow, we have a chance to check for concurrency even before bothering
            //the database!  Of course, it will get checked again in the database just in case
            //it changes after we pulled the record.  
            //Note using SequenceEqual becuase it is an array after all.
            if (camperDTO.RowVersion != null)
            {
                if (!camperToUpdate.RowVersion.SequenceEqual(camperDTO.RowVersion))
                {
                    return Conflict(new { message = "Concurrency Error: Patient has been changed by another user.  Try editing the record again." });
                }
            }
            
            //Update the copy from the database from the client data 
            camperToUpdate.ID = camperDTO.ID;
            camperToUpdate.FirstName = camperDTO.FirstName;
            camperToUpdate.MiddleName = camperDTO.MiddleName;
            camperToUpdate.LastName = camperDTO.LastName;
            camperToUpdate.DOB = camperDTO.DOB;
            camperToUpdate.Gender = camperDTO.Gender;
            camperToUpdate.EMail = camperDTO.EMail;
            camperToUpdate.Phone = camperDTO.Phone;
            camperToUpdate.CompoundID = camperDTO.CompoundID;

            //Put the original RowVersion value in the OriginalValues collection for the entity
            _context.Entry(camperToUpdate).Property("RowVersion").OriginalValue = camperDTO.RowVersion;

            _context.Entry(camperDTO).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CamperExists(id))
                {
                    return Conflict(new { message = "Concurrency Error: Camper has been Removed." });
                }
                else
                {
                    return Conflict(new { message = "Concurrency Error: Camper has been updated by another user. Back out and try editing the record again." });
                }
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("UNIQUE"))
                {
                    return BadRequest(new { message = "Unable to save: Duplicate EMail." });
                }
                else
                {
                    return BadRequest(new { message = "Unable to save changes to the database. Try again, and if the problem persists see your system administrator." });
                }
            }
        }

        // POST: api/Campers
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<CamperDTO>> PostCamper(CamperDTO camperDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Camper camper = new Camper
            {
                ID = camperDTO.ID,
                FirstName = camperDTO.FirstName,
                MiddleName = camperDTO.MiddleName,
                LastName = camperDTO.LastName,
                DOB = camperDTO.DOB,
                Gender = camperDTO.Gender,
                EMail = camperDTO.EMail,
                Phone = camperDTO.Phone,
                CompoundID = camperDTO.CompoundID
            };

            try
            {
                _context.Campers.Add(camper);
                await _context.SaveChangesAsync();

                //Assign Database Generated values back into the DTO
                camperDTO.ID = camper.ID;
                camperDTO.RowVersion = camper.RowVersion;

                return CreatedAtAction("GetCamper", new { id = camper.ID }, camperDTO);
            }

            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("UNIQUE"))
                {
                    return BadRequest(new { message = "Unable to save: Duplicate Email." });
                }
                else
                {
                    return BadRequest(new { message = "Unable to save changes to the database. Try again, and if the problem persists see your system administrator." });
                }
            }
        }

        // DELETE: api/Campers/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Camper>> DeleteCamper(int id)
        {
            var camper = await _context.Campers.FindAsync(id);
            if (camper == null)
            {
                return NotFound(new { message = "Delete Error: Camper has already been removed." });
            }

            try
            {
                _context.Campers.Remove(camper);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { message = "Delete Error: Unable to delete Camper." });
            }
        }

        private bool CamperExists(int id)
        {
            return _context.Campers.Any(e => e.ID == id);
        }
    }
}

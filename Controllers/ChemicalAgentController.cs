using AGROCHEM.Models.EntitiesDto;
using AGROCHEM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGROCHEM.Controllers
{

    [Route("agrochem/chemicalagents")]
    [Authorize]
    
    public class ChemicalAgentController : ControllerBase
    {
        private readonly ChemicalAgentService _chemicalAgentService;
        public ChemicalAgentController(ChemicalAgentService chemicalAgentService)
        {
            _chemicalAgentService = chemicalAgentService;
        }


        [HttpGet]
        public async Task<IActionResult> GetChemicalAgents([FromQuery] bool isArchive)
        {
            try
            {
                var result = await _chemicalAgentService.GetChemicalAgents(isArchive);
                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Wystąpił błąd podczas pobierania środków chemicznych." });
            }

        }

        
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetChemicalAgentById(int id)
        {
            try
            {
                var result = await _chemicalAgentService.GetChemicalAgentById(id);
                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Wystąpił błąd podczas pobierania środków chemicznych." });
            }

        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddChemicalAgent([FromForm] ChemicalAgentPhotoDTO chemicalAgentDTO)
        {
            try
            {
                string result = await _chemicalAgentService.AddChemicalAgent(chemicalAgentDTO);
                if (result == "Utworzono środek chemiczny.")
                {
                    return Ok(new { message = result });
                }
                else
                {
                    return BadRequest(new { message = result });
                }
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Wystąpił błąd podczas tworzenia środka." });
            }

        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        [Route("archive/{id}")]
        public async Task<IActionResult> ArchiveChemicalAgent(int id, [FromQuery] bool archive)
        {
            try
            {

                bool isUpdated = await _chemicalAgentService.UpdateArchiveChemAgent(id, archive);
                if (archive == true)
                {
                    if (!isUpdated)
                    {
                        return BadRequest(new { message = "Nie można zarchiwizować tego środka." });
                    }

                    return Ok(new { message = "Zarchiwizowano pomyślnie" });
                }
                else
                {
                    if (!isUpdated)
                    {
                        return BadRequest(new { message = "Nie można cofnąć archiwizacji tego środka." });
                    }

                    return Ok(new { message = "Cofnięto archiwizację pomyślnie" });
                }
            }
            catch (ApplicationException ex)
            {
                // Złap ApplicationException wyrzucony z serwisu
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateChemAgent(int id, [FromForm] ChemicalAgentPhotoDTO chemicalAgentDTO)
        {
            try
            {

                bool isUpdated = await _chemicalAgentService.UpdateChemicalAgent(id, chemicalAgentDTO);

                if (!isUpdated)
                {
                    return BadRequest(new { message = "Nie można edytować tego środka." });
                }

                return Ok(new { message = "Edytowano pomyślnie" });

            }
            catch (ApplicationException ex)
            {
                // Złap ApplicationException wyrzucony z serwisu
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}

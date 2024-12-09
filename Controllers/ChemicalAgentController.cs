using AGROCHEM.Models.EntitiesDto;
using AGROCHEM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGROCHEM.Controllers
{

    [Route("agrochem/chemicalagents")]
    [Authorize(Roles = "Admin")]
    public class ChemicalAgentController : ControllerBase
    {
        private readonly ChemicalAgentService _chemicalAgentService;
        public ChemicalAgentController(ChemicalAgentService chemicalAgentService)
        {
            _chemicalAgentService = chemicalAgentService;
        }


        [HttpGet]
        public async Task<IActionResult> GetChemicalAgents()
        {
            try
            {
                var result = await _chemicalAgentService.GetChemicalAgents();
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

        [HttpPost]
        public async Task<IActionResult> AddChemicalAgent([FromBody] ChemicalAgentDTO chemicalAgentDTO)
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

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateChemAgent(int id, [FromBody] ChemicalAgentDTO chemicalAgentDTO)
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

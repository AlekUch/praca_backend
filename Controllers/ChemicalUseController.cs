using AGROCHEM.Models.EntitiesDto;
using AGROCHEM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AGROCHEM.Controllers
{
    [Route("agrochem/chemicaluse")]
    public class ChemicalUseController : ControllerBase
    {
        private readonly ChemicalUseService _chemAgentUseService;
        public ChemicalUseController(ChemicalUseService chemAgentUseService)
        {
            _chemAgentUseService = chemAgentUseService;
        }

        [Authorize]
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetChemAgentUse(int id)
        {            
                try
                {
                    var chemAgentUse = await _chemAgentUseService.GetChemAgentUse(id);
                    return Ok(chemAgentUse);
                }
                catch (ApplicationException ex)
                {
                    return StatusCode(500, new { message = ex.Message });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = "Wystąpił błąd podczas pobierania szczegółowych informacji." });
                }            
        }

        [HttpPost]
        public async Task<IActionResult> AddChemicalUse([FromBody] ChemicalUseDTO chemicalUseDTO)
        {
            try
            {
                string result = await _chemAgentUseService.AddChemicalUse(chemicalUseDTO);
                if (result == "Utworzono nową informację.")
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
                return StatusCode(500, new { message = "Wystąpił błąd podczas tworzenia informacji." });
            }

        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateChemUse(int id, [FromBody] ChemicalUseDTO chemicalUseDTO)
        {
            try
            {

                bool isUpdated = await _chemAgentUseService.UpdateChemicalUse(id, chemicalUseDTO);

                if (!isUpdated)
                {
                    return BadRequest(new { message = "Nie można edytować tej informacji." });
                }

                return Ok(new { message = "Edytowano pomyślnie" });

            }
            catch (ApplicationException ex)
            {
                // Złap ApplicationException wyrzucony z serwisu
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut]
        [Route("archive/{id}")]
        public async Task<IActionResult> ArchiveChemicalUse(int id, [FromQuery] bool archive)
        {
            try
            {

                bool isUpdated = await _chemAgentUseService.UpdateArchiveChemUse(id, archive);
                if (archive == true)
                {
                    if (!isUpdated)
                    {
                        return BadRequest(new { message = "Nie można zarchiwizować tej informacji." });
                    }

                    return Ok(new { message = "Zarchiwizowano pomyślnie" });
                }
                else
                {
                    if (!isUpdated)
                    {
                        return BadRequest(new { message = "Nie można cofnąć archiwizacji tej informcji." });
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
    }
}

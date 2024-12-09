using AGROCHEM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    }
}

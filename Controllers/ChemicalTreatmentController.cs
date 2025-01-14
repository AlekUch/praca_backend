using AGROCHEM.Models.Entities;
using AGROCHEM.Models.EntitiesDto;
using AGROCHEM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace AGROCHEM.Controllers
{
    [Route("agrochem/chemicaltreatment")]
    public class ChemicalTreatmentController : ControllerBase
    {
        private readonly ChemicalTreatmentService _chemicalTreatmentService;
        public ChemicalTreatmentController(ChemicalTreatmentService chemicalTreatmentService)
        {
            _chemicalTreatmentService = chemicalTreatmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetChemicalTreatments()
        {
            var userId = HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "Brak uprawnień" });
            }

            try
            {
                var result = await _chemicalTreatmentService.GetChemicalTreatments(Convert.ToInt32(userId));
                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Wystąpił błąd podczas pobierania zabiegów chemicznych." });
            }

        }


        [HttpPost]
        public async Task<IActionResult> AddChemicalTreatment([FromBody] ChemicalTreatmentDTO chemicalTreatmentDTO)
        {
            try
            {
                string result = await _chemicalTreatmentService.AddChemicalTreatment(chemicalTreatmentDTO);
                if (result == "Utworzono nowy zabieg chemiczny.")
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
                return StatusCode(500, new { message = "Wystąpił błąd podczas tworzenia zabiegu." });
            }

        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateChemTreat(int id, [FromBody] ChemicalTreatmentDTO chemtreatDTO)
        {
            try
            {

                bool isUpdated = await _chemicalTreatmentService.UpdateChemTreat(id, chemtreatDTO);

                if (!isUpdated)
                {
                    return BadRequest(new { message = "Nie można edytować tej działki." });
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

using AGROCHEM.Models.Entities;
using AGROCHEM.Models.EntitiesDto;
using AGROCHEM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AGROCHEM.Controllers
{
    [Route("agrochem/cultivations")]
    public class CultivationController : ControllerBase
    {
        private readonly CultivationService _cultivationService;
        public CultivationController(CultivationService cultivationService)
        {
            _cultivationService = cultivationService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCultivationsPlants([FromQuery] bool isArchive)
        {
            var userId = Convert.ToInt32(HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (userId == null)
            {
                return BadRequest(new { message = "Brak uprawnień" });
            }
            else
            {
                try
                {
                    var cultivations = await _cultivationService.GetCultivations(userId, isArchive);
                    
                    return Ok(cultivations);
                }
                catch (ApplicationException ex)
                {
                    return StatusCode(500, new { message = ex.Message });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = "Wystąpił błąd podczas pobierania działek." });
                }
            }
        }

       

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddCultivation([FromBody] Cultivation cultivation)
        {
            var userId = Convert.ToInt32(HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (userId == null)
            {
                return BadRequest(new { message = "Brak uprawnień" });
            }
            else
            {
                try
                {
                    string result = await _cultivationService.AddCultivation(cultivation);
                    if (result == "Utworzono uprawę.")
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
                    return StatusCode(500, new { message = "Wystąpił błąd podczas pobierania działek." });
                }

            }
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateCultivation(int id, [FromBody] CultivationDTO cultivationDTO)
        {
            try
            {

                bool isUpdated = await _cultivationService.UpdateCultivation(id, cultivationDTO);

                if (!isUpdated)
                {
                    return BadRequest(new { message = "Nie można edytować tej uprawy." });
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
        public async Task<IActionResult> ArchiveCultivation(int id, [FromQuery] bool archive)
        {
            try
            {

                bool isUpdated = await _cultivationService.UpdateArchiveCultivation(id, archive);
                if (archive == true)
                {
                    if (!isUpdated)
                    {
                        return BadRequest(new { message = "Nie można zakończyć uprawy." });
                    }

                    return Ok(new { message = "Zakończono pomyślnie" });
                }
                else
                {
                    if (!isUpdated)
                    {
                        return BadRequest(new { message = "Nie można cofnąć archiwizacji tej działki." });
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

        [Authorize]
        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<IActionResult> DeleteCultivation(int id)
        {
            try
            {
                bool isDeleted = await _cultivationService.DeleteCultivation(id);

                if (!isDeleted)
                {
                    return BadRequest(new { message = "Nie można usunąć uprawy." });
                }

                return Ok(new { message = "Usunięto pomyślnie" });

            }
            catch (ApplicationException ex)
            {
                // Złap ApplicationException wyrzucony z serwisu
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}

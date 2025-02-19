using AGROCHEM.Models.Entities;
using AGROCHEM.Models.EntitiesDto;
using AGROCHEM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGROCHEM.Controllers
{
    [Route("agrochem/plants")]
    [Authorize/*(Roles = "Admin")*/]
    public class PlantController : ControllerBase
    {
        private readonly PlantService _plantService;
        public PlantController(PlantService plantService)
        {
            _plantService = plantService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetPlants([FromQuery] bool isArchive)
        {
            try
            {
                var result = await _plantService.GetPlants(isArchive);
                return Ok(result);
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

        [HttpPost]
        public async Task<IActionResult> AddPlant([FromBody] PlantDTO plantDTO)
        {
            try
            {
                string result = await _plantService.AddPlant(plantDTO);
                if (result == "Utworzono roślinę.")
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

        [HttpPut]
        [Route("archive/{id}")]
        public async Task<IActionResult> ArchivePlant(int id, [FromQuery] bool archive)
        {
            try
            {

                bool isUpdated = await _plantService.UpdateArchivePlant(id, archive);
                if (archive == true)
                {
                    if (!isUpdated)
                    {
                        return BadRequest(new { message = "Nie można zarchiwizować tej rośliny." });
                    }

                    return Ok(new { message = "Zarchiwizowano pomyślnie" });
                }
                else
                {
                    if (!isUpdated)
                    {
                        return BadRequest(new { message = "Nie można cofnąć archiwizacji tej rośliny." });
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
        public async Task<IActionResult> UpdatePlant(int id, [FromBody] PlantDTO PlantDTO)
        {
            try
            {

                bool isUpdated = await _plantService.UpdatePlant(id, PlantDTO);

                if (!isUpdated)
                {
                    return BadRequest(new { message = "Nie można edytować tej rośliny." });
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

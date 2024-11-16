using AGROCHEM.Models.Entities;
using AGROCHEM.Models.EntitiesDto;
using AGROCHEM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGROCHEM.Controllers
{
    [Route("agrochem/plants")]
    [Authorize(Roles = "Admin")]
    public class PlantController : ControllerBase
    {
        private readonly PlantService _plantService;
        public PlantController(PlantService plantService)
        {
            _plantService = plantService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPlants()
        {
            try
            {
                var result = await _plantService.GetPlants();
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
    }
}

using AGROCHEM.Models.EntitiesDto;
using AGROCHEM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGROCHEM.Controllers
{
    [Route("agrochem/disease")]
    [Authorize(Roles = "Admin")]
    public class DiseaseController : ControllerBase
    {
        private readonly DiseaseService _diseaseService;
        public DiseaseController(DiseaseService diseaseService)
        {
            _diseaseService = diseaseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDisease()
        {
            try
            {
                var result = await _diseaseService.GetDisease();
                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Wystąpił błąd podczas pobierania chorób." });
            }

        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetDiseaseById(int id)
        {
            try
            {
                var result = await _diseaseService.GetDiseaseById(id);
                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Wystąpił błąd podczas pobierania danych." });
            }

        }

        [HttpPost]
        public async Task<IActionResult> AddDisease([FromForm] DiseasePhotoDTO diseasePhotoDTO)
        {
            try
            {
                string result = await _diseaseService.AddDisease(diseasePhotoDTO);
                if (result == "Utworzono chorobę.")
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
                return StatusCode(500, new { message = "Wystąpił błąd podczas tworzenia choroby." });
            }

        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateDisease(int id, [FromForm] DiseasePhotoDTO diseasePhotoDTO)
        {
            try
            {

                bool isUpdated = await _diseaseService.UpdateDisease(id, diseasePhotoDTO);

                if (!isUpdated)
                {
                    return BadRequest(new { message = "Nie można edytować tej choroby." });
                }

                return Ok(new { message = "Edytowano pomyślnie" });

            }
            catch (ApplicationException ex)
            {
                // Złap ApplicationException wyrzucony z serwisu
                return StatusCode(500, new { message = ex.Message });
            }
        }


        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<IActionResult> DeleteDisease(int id)
        {
            try
            {
                bool isDeleted = await _diseaseService.DeleteDisease(id);

                if (!isDeleted)
                {
                    return BadRequest(new { message = "Nie można usunąć choroby." });
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

using AGROCHEM.Models.Entities;
using AGROCHEM.Models.EntitiesDto;
using AGROCHEM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Newtonsoft.Json.Linq;


namespace AGROCHEM.Controllers
{
    [Route("agrochem/plots")]
    [Authorize]
    public class PlotController : ControllerBase
    {
        private readonly PlotService _plotService;
        public PlotController(PlotService plotService)
        {
            _plotService = plotService;
        }

            
        [HttpPost]
        public async Task<IActionResult> Plots([FromBody] PlotDto plotDto)
        {

            if (ModelState.IsValid)
            {

                var userId = HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return BadRequest(new { message = "Brak uprawnień" });
                }
                else
                {
                    plotDto.OwnerId = Convert.ToInt32(userId);
                    string result = await _plotService.AddPlot(plotDto);
                    if (result == "Utworzono nową działkę.")
                    {
                        return Ok(new { message = result });
                    }
                    else
                    {
                        return BadRequest(new { message = result });
                    }
                }
            }
            return BadRequest(new { message = "Wystąpił błąd w tworzeniu działki." });
        }

        
        [HttpGet]
        public async Task<IActionResult> GetUserPlots([FromQuery] bool isArchive)
        {
            var userId = HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return BadRequest(new { message = "Brak uprawnień" });
            }
            else
            {
                try
                {
                    var plots = await _plotService.GetUserPlots(Convert.ToInt32(userId), isArchive);
                    Console.WriteLine(plots);
                    return Ok(plots);
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
        [Route("archive/{id}")]
        public async Task<IActionResult> ArchivePlot(int id, [FromQuery] bool archive)
        {
            try
            {

                bool isUpdated = await _plotService.UpdateArchivePlot(id, archive);
                if (archive == true)
                {
                    if (!isUpdated)
                    {
                        return BadRequest(new { message = "Nie można zarchiwizować tej działki." });
                    }

                    return Ok(new { message = "Zarchiwizowano pomyślnie" });
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
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdatePlot(int id, [FromBody] PlotDto PlotDto)
        {
            try
            {

                bool isUpdated = await _plotService.UpdatePlot(id, PlotDto);

                if (!isUpdated)
                {
                    return BadRequest(new { message = "Nie można edytować tej działki." });
                }

                return Ok(new { message = "Edytowano pomyślnie" });

            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

       
        [Route("plotsArea")]
        [HttpGet]
        public async Task<IActionResult> GetPlotsArea()
        {
            var userId = HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return BadRequest(new { message = "Brak uprawnień" });
            }
            else
            {
                try
                {
                    var plotsArea = await _plotService.GetPlotsArea(Convert.ToInt32(userId));
                    Console.WriteLine(plotsArea);
                    return Ok(plotsArea);
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
}

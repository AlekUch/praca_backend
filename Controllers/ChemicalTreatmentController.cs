using AGROCHEM.Models.Entities;
using AGROCHEM.Models.EntitiesDto;
using AGROCHEM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace AGROCHEM.Controllers
{
    [Route("agrochem/plants")]
    public class ChemicalTreatmentController : ControllerBase
    {
        private readonly ChemicalTreatmentService _chemicalTreatmentService;
        public ChemicalTreatmentController(ChemicalTreatmentService chemicalTreatmentService)
        {
            _chemicalTreatmentService = chemicalTreatmentService;
        }
    }
}

using AGROCHEM.Data;
using AGROCHEM.Models.Entities;
using AGROCHEM.Models.EntitiesDto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace AGROCHEM.Services
{
    public class ChemicalTreatmentService
    {
        private readonly AgrochemContext _context;
        private readonly IConfiguration _configuration;

        public ChemicalTreatmentService(AgrochemContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

        }
    }
}

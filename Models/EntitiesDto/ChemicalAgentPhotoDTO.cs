namespace AGROCHEM.Models.EntitiesDto
{
    public class ChemicalAgentPhotoDTO
    {
        public int ChemAgentId { get; set; }

        public string? Name { get; set; }
        public string? Type { get; set; }
        public string? Description { get; set; }
        public IFormFile? File { get; set; }
    }
}

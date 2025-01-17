using AGROCHEM.Data;
using AGROCHEM.Models.Entities;
using AGROCHEM.Models.EntitiesDto;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AGROCHEM.Services
{
    public class PlotService
    {
        private readonly AgrochemContext _context;
        private readonly IConfiguration _configuration;

        public PlotService(AgrochemContext context,  IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

        }

        public async Task<string> AddPlot(PlotDto plotDto)
        {
            try
            {
                var plot = _context.Plots
                .FirstOrDefault(p => p.PlotNumber == plotDto.PlotNumber);
                if (plot != null)
                {
                    return "Działka o tym numerze już istnieje";
                }

                var address = _context.PlotAddresses
                .FirstOrDefault(a => a.Location == plotDto.Location && a.District == plotDto.District && a.Voivodeship == plotDto.Voivodeship);

                int newAddressId;
                if (address is null)
                {
                    var newAddress = new PlotAddress
                    {
                        Location = plotDto.Location,
                        District = plotDto.District,
                        Voivodeship = plotDto.Voivodeship
                    };

                    _context.PlotAddresses.Add(newAddress);
                    await _context.SaveChangesAsync();
                    newAddressId = newAddress.PlotAddressId;

                }
                else
                {
                    newAddressId = address.PlotAddressId;
                }

                var newPlot = new Plot
                {
                    PlotNumber = plotDto.PlotNumber,
                    AddressId = newAddressId,
                    OwnerId = plotDto.OwnerId,
                    Area = Convert.ToDecimal(plotDto.Area),
                    Archival = false
                };

                _context.Plots.Add(newPlot);
                await _context.SaveChangesAsync();
                return "Utworzono nową działkę.";
            }
            catch (Exception ex)
            {
                // Logowanie błędu
                Console.WriteLine(ex.Message);
                return ex.Message;
            }
        }

        public async Task<List<Plot>> GetUserPlots(int userId, bool isArchive)
        {
            try
            {
                if (isArchive == false)
                {
                    var plots = await _context.Plots
                   .Where(p => p.OwnerId == userId && p.Archival==isArchive )
                   .Include(p => p.Address)
                   .OrderBy(p => p.Archival)
                   .ToListAsync();
                    return plots;
                }
                else
                {
                    var plots = await _context.Plots
                  .Where(p => p.OwnerId == userId )
                  .Include(p => p.Address)
                  .OrderBy(p => p.Archival)
                  .ToListAsync();
                    return plots;
                }                              
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                throw new ApplicationException("Błąd podczas pobierania działek", ex);
            }
        }

        public async Task<bool> UpdatePlot(int id, PlotDto plotDto)
        {
            var plot = await _context.Plots.FindAsync(id);
            if (plot == null)
            {
                return false; // Użytkownik nie istnieje
            }

            // Aktualizowanie właściwości użytkownika
            plot.PlotNumber = plotDto.PlotNumber;
            plot.Area = plotDto.Area;

            var addressId = plot.AddressId;
            var address = await _context.PlotAddresses.FindAsync(addressId);
            if (address == null)
            {
                return false;
            }
            address.Location = plotDto.Location;
            address.District = plotDto.District;
            address.Voivodeship = plotDto.Voivodeship;
            // inne pola

            _context.PlotAddresses.Update(address);
            _context.Plots.Update(plot);
            await _context.SaveChangesAsync();

            return true; // Operacja zakończona sukcesem
        }
        public async Task<bool> UpdateArchivePlot(int id, bool archive)
        {
            try
            {
                var plot = await _context.Plots.FindAsync(id);
                if (plot == null)
                {
                    return false;
                }

                plot.Archival = archive;

                _context.Plots.Update(plot);
                await _context.SaveChangesAsync();

                return true; // Operacja zakończona sukcesem
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                throw new ApplicationException("Błąd podczas archiwizacj działek", ex);
            }
        }
        public async Task<List<PlotsAreaDto>> GetPlotsArea(int userId)
        {
            try
            {
                var plotsArea = await _context.Plots
                 .FromSqlRaw("SELECT PlotId,PlotNumber,   p.Area -  (SELECT COALESCE(SUM(c.Area), 0) FROM [agro_chem].Cultivation c WHERE c.PlotId=p.PlotId AND (c.Archival IS NULL OR c.Archival=0)) AS Area " +
                    "FROM [agro_chem].Plot p WHERE p.OwnerId = @userId",
                 new SqlParameter("@userId", userId))
                 .Select(p => new PlotsAreaDto
                 {
                     PlotId = p.PlotId,
                     PlotNumber = p.PlotNumber,
                     Area = p.Area
                 })
                .ToListAsync();



                return plotsArea;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                throw new ApplicationException("Błąd podczas pobierania działek", ex);
            }
        }
    }
}

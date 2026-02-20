using EscalaGcm.Application.DTOs.Guardas;

namespace EscalaGcm.Application.Services.Interfaces;

public interface IGuardaAvailabilityService
{
    Task<List<DayAvailabilityDto>> GetAvailabilityAsync(int guardaId, int ano, int mes);
}

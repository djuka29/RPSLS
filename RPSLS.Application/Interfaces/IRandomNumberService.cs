namespace RPSLS.Application.Interfaces;

public interface IRandomNumberService
{
    Task<int> GetRandomNumberAsync();
}

using RPSLS.Domain.Entities;

namespace RPSLS.Application.Interfaces;

public interface IRPSLSGameService
{
    Task<RoundResult> PlayRoundAsync(Guid userId, int playerChoice);
    List<RoundResult> GetScoreboard(Guid userId);
    void ResetScoreboard(Guid userId);
}

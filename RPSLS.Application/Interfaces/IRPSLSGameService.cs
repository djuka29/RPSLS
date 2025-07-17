using RPSLS.Domain.Entities;

namespace RPSLS.Application.Interfaces;

public interface IRPSLSGameService
{
    RoundResult PlayRound(Guid userId, int playerChoice);
    List<RoundResult> GetScoreboard(Guid userId);
    void ResetScoreboard(Guid userId);
}

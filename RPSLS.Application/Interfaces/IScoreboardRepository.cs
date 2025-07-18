using RPSLS.Domain.Entities;

namespace RPSLS.Application.Interfaces
{
    public interface IScoreboardRepository
    {
        void AddRoundResult(Guid userId, RoundResult roundResult);
        List<RoundResult> GetScoreboard(Guid userId);
        void ResetScoreboard(Guid userId);
    }
}

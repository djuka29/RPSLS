using RPSLS.Application.Interfaces;
using RPSLS.Domain.Entities;
using RPSLS.Domain.Enums;
using RPSLS.Domain.Extensions;
using System.Collections.Concurrent;

namespace RPSLS.Application.Services;

public class RPSLSGameService : IRPSLSGameService
{
    private readonly IRandomNumberService _randomNumberService; 
    private readonly IScoreboardRepository _scoreboardRepository;

    private readonly ConcurrentDictionary<Guid, Queue<RoundResult>> _scoreboards = new();

    public RPSLSGameService(
        IRandomNumberService randomNumberService, 
        IScoreboardRepository scoreboardRepository)
    {
        _randomNumberService = randomNumberService;
        _scoreboardRepository = scoreboardRepository;

    }

    public async Task<RoundResult> PlayRoundAsync(Guid userId, int playerChoiceId)
    {
        var player = (Choice)playerChoiceId;
        var rnd = await _randomNumberService.GetRandomNumberAsync();
        var computer = (Choice)((rnd % 5) + 1);

        RoundResultType result;
        if (player == computer)
            result = RoundResultType.Tie;
        else if (player.IsWinnerAgainst(computer))
            result = RoundResultType.Win;
        else
            result = RoundResultType.Lose;

        var roundResult = new RoundResult
        {
            Player = player,
            Computer = computer,
            Result = result
        };

        _scoreboardRepository.AddRoundResult(userId, roundResult);

        return roundResult;
    }

    public List<RoundResult> GetScoreboard(Guid userId)
    {
        return _scoreboardRepository.GetScoreboard(userId);
    }

    public void ResetScoreboard(Guid userId)
    {
        _scoreboardRepository.ResetScoreboard(userId);
    }
}

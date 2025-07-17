using RPSLS.Application.Interfaces;
using RPSLS.Domain.Entities;
using RPSLS.Domain.Enums;
using RPSLS.Domain.Extensions;
using System.Collections.Concurrent;

namespace RPSLS.Application.Services;

public class RPSLSGameService : IRPSLSGameService
{
    private readonly IRandomNumberService _randomNumberService;
    private readonly ConcurrentDictionary<Guid, Queue<RoundResult>> _scoreboards = new();

    public RPSLSGameService(IRandomNumberService randomNumberService)
    {
        _randomNumberService = randomNumberService;
    }

    public RoundResult PlayRound(Guid userId, int playerChoiceId)
    {
        var player = (Choice)playerChoiceId;
        var rnd = _randomNumberService.GetRandomNumberAsync().Result;
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

        var queue = _scoreboards.GetOrAdd(userId, _ => new Queue<RoundResult>());
        lock (queue)
        {
            queue.Enqueue(roundResult);
            if (queue.Count > 10) queue.Dequeue();
        }

        return roundResult;
    }

    public List<RoundResult> GetScoreboard(Guid userId)
    {
        if (_scoreboards.TryGetValue(userId, out var queue))
            return queue.ToList();

        return [];
    }

    public void ResetScoreboard(Guid userId)
    {
        _scoreboards[userId] = new Queue<RoundResult>();
    }
}

using Moq;
using RPSLS.Application.Interfaces;
using RPSLS.Application.Services;
using RPSLS.Domain.Entities;
using RPSLS.Domain.Enums;

namespace RPSLS.Tests
{
    public class RPSLSTests
    {
        private readonly Mock<IRandomNumberService> _randomMock;
        private readonly IRPSLSGameService _gameService;
        private readonly Guid _defaultUserId = Guid.NewGuid();

        public RPSLSTests()
        {
            _randomMock = new Mock<IRandomNumberService>();
            _gameService = new RPSLSGameService(_randomMock.Object);
        }

        [Theory]
        [InlineData(Choice.Rock, Choice.Scissors, RoundResultType.Win)]
        [InlineData(Choice.Scissors, Choice.Rock, RoundResultType.Lose)]
        [InlineData(Choice.Lizard, Choice.Spock, RoundResultType.Win)]
        [InlineData(Choice.Spock, Choice.Lizard, RoundResultType.Lose)]
        [InlineData(Choice.Paper, Choice.Paper, RoundResultType.Tie)]
        public void PlayRound_ReturnsExpectedOutcome(Choice player, Choice computer, RoundResultType expected)
        {
            _randomMock.Setup(r => r.GetRandomNumberAsync())
                       .ReturnsAsync((int)computer - 1);

            var roundResult = _gameService.PlayRound(_defaultUserId, (int)player);

            Assert.Equal(player, roundResult.Player);
            Assert.Equal(computer, roundResult.Computer);
            Assert.Equal(expected, roundResult.Result);
        }

        [Fact]
        public void Scoreboard_ReturnsLast10Results()
        {
            _randomMock.Setup(r => r.GetRandomNumberAsync()).ReturnsAsync((int)Choice.Scissors - 1);
            for (int i = 0; i < 15; i++)
            {
                _gameService.PlayRound(_defaultUserId, (int)Choice.Rock);
            }

            var board = _gameService.GetScoreboard(_defaultUserId);
            Assert.Equal(10, board.Count);
        }

        [Fact]
        public void ResetScoreboard_ClearsUserScores()
        {
            _randomMock.Setup(r => r.GetRandomNumberAsync()).ReturnsAsync((int)Choice.Scissors - 1);
            _gameService.PlayRound(_defaultUserId, (int)Choice.Rock);

            _gameService.ResetScoreboard(_defaultUserId);
            var board = _gameService.GetScoreboard(_defaultUserId);
            Assert.Empty(board);
        }

        [Fact]
        public void GetScoreboard_ForUnknownUser_ReturnsEmpty()
        {
            var result = _gameService.GetScoreboard(Guid.NewGuid());
            Assert.Empty(result);
        }

        [Fact]
        public void PlayRound_HandlesAllChoicesAgainstEachOther()
        {
            foreach (Choice player in Enum.GetValues(typeof(Choice)))
            {
                foreach (Choice computer in Enum.GetValues(typeof(Choice)))
                {
                    _randomMock.Setup(r => r.GetRandomNumberAsync()).ReturnsAsync((int)computer - 1);
                    var roundResult = _gameService.PlayRound(_defaultUserId, (int)player);
                    Assert.Equal(player, roundResult.Player);
                    Assert.Equal(computer, roundResult.Computer);
                    Assert.Contains(roundResult.Result, new[] { RoundResultType.Win, RoundResultType.Lose, RoundResultType.Tie });
                }
            }
        }

        [Fact]
        public void InvalidChoiceId_ThrowsException()
        {
            _randomMock.Setup(r => r.GetRandomNumberAsync()).ReturnsAsync(0);
            Assert.Throws<ArgumentException>(() => _gameService.PlayRound(_defaultUserId, 999));
        }
    }
}
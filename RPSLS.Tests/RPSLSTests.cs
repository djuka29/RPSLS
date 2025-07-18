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
        private readonly Mock<IScoreboardRepository> _scoreboardRepositoryMock;
        private readonly IRPSLSGameService _gameService;
        private readonly Guid _defaultUserId = Guid.NewGuid();

        public RPSLSTests()
        {
            _randomMock = new Mock<IRandomNumberService>();
            _scoreboardRepositoryMock = new Mock<IScoreboardRepository>();
            _gameService = new RPSLSGameService(_randomMock.Object, _scoreboardRepositoryMock.Object);
        }

        [Theory]
        [InlineData(Choice.Rock, Choice.Scissors, RoundResultType.Win)]
        [InlineData(Choice.Scissors, Choice.Rock, RoundResultType.Lose)]
        [InlineData(Choice.Lizard, Choice.Spock, RoundResultType.Win)]
        [InlineData(Choice.Spock, Choice.Lizard, RoundResultType.Lose)]
        [InlineData(Choice.Paper, Choice.Paper, RoundResultType.Tie)]
        public async Task PlayRound_ReturnsExpectedOutcomeAsync(Choice player, Choice computer, RoundResultType expected)
        {
            _randomMock.Setup(r => r.GetRandomNumberAsync())
                       .ReturnsAsync((int)computer - 1);

            RoundResult? capturedResult = null;
            _scoreboardRepositoryMock
                .Setup(r => r.AddRoundResult(_defaultUserId, It.IsAny<RoundResult>()))
                .Callback<Guid, RoundResult>((_, rr) => capturedResult = rr);

            var roundResult = await _gameService.PlayRoundAsync(_defaultUserId, (int)player);

            Assert.Equal(player, roundResult.Player);
            Assert.Equal(computer, roundResult.Computer);
            Assert.Equal(expected, roundResult.Result);

            // Ensure AddRoundResult was called with correct data
            _scoreboardRepositoryMock.Verify(r => r.AddRoundResult(_defaultUserId, It.IsAny<RoundResult>()), Times.Once);
            Assert.NotNull(capturedResult);
            Assert.Equal(player, capturedResult.Player);
            Assert.Equal(computer, capturedResult.Computer);
            Assert.Equal(expected, capturedResult.Result);
        }

        [Fact]
        public void Scoreboard_ReturnsLast10Results()
        {
            var expectedResults = Enumerable.Range(0, 10)
                .Select(i => new RoundResult
                {
                    Player = Choice.Rock,
                    Computer = Choice.Scissors,
                    Result = RoundResultType.Win
                }).ToList();

            _scoreboardRepositoryMock.Setup(r => r.GetScoreboard(_defaultUserId))
                .Returns(expectedResults);

            var board = _gameService.GetScoreboard(_defaultUserId);
            Assert.Equal(10, board.Count);
            Assert.All(board, r => Assert.Equal(Choice.Rock, r.Player));
        }

        [Fact]
        public void ResetScoreboard_ClearsUserScores()
        {
            _scoreboardRepositoryMock.Setup(r => r.GetScoreboard(_defaultUserId)).Returns(new List<RoundResult>());
            _gameService.ResetScoreboard(_defaultUserId);

            _scoreboardRepositoryMock.Verify(r => r.ResetScoreboard(_defaultUserId), Times.Once);

            var board = _gameService.GetScoreboard(_defaultUserId);
            Assert.Empty(board);
        }

        [Fact]
        public void GetScoreboard_ForUnknownUser_ReturnsEmpty()
        {
            _scoreboardRepositoryMock.Setup(r => r.GetScoreboard(It.IsAny<Guid>())).Returns(new List<RoundResult>());
            var result = _gameService.GetScoreboard(Guid.NewGuid());
            Assert.Empty(result);
        }

        [Fact]
        public async Task PlayRound_HandlesAllChoicesAgainstEachOtherAsync()
        {
            foreach (Choice player in Enum.GetValues(typeof(Choice)))
            {
                foreach (Choice computer in Enum.GetValues(typeof(Choice)))
                {
                    _randomMock.Setup(r => r.GetRandomNumberAsync()).ReturnsAsync((int)computer - 1);

                    RoundResult? capturedResult = null;
                    _scoreboardRepositoryMock
                        .Setup(r => r.AddRoundResult(_defaultUserId, It.IsAny<RoundResult>()))
                        .Callback<Guid, RoundResult>((_, rr) => capturedResult = rr);

                    var roundResult = await _gameService.PlayRoundAsync(_defaultUserId, (int)player);

                    Assert.Equal(player, roundResult.Player);
                    Assert.Equal(computer, roundResult.Computer);
                    Assert.Contains(roundResult.Result, new[] { RoundResultType.Win, RoundResultType.Lose, RoundResultType.Tie });

                    _scoreboardRepositoryMock.Verify(r => r.AddRoundResult(_defaultUserId, It.IsAny<RoundResult>()), Times.AtLeastOnce);
                    Assert.NotNull(capturedResult);
                }
            }
        }
    }
}
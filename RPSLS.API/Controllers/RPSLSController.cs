using Microsoft.AspNetCore.Mvc;
using RPSLS.Application.Interfaces;
using RPSLS.Application.DTOs;
using RPSLS.Domain.Entities;
using RPSLS.Domain.Extensions;

namespace RPSLS.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class RPSLSController : ControllerBase
    {
        private readonly IRPSLSGameService _gameService;
        private readonly IRandomNumberService _randomNumberService;
        
        public RPSLSController(IRPSLSGameService gameService, IRandomNumberService randomNumberService)
        {
            _gameService = gameService;
            _randomNumberService = randomNumberService;
        }

        [HttpGet("choices")]
        public IActionResult GetChoices()
        {
            var choices = Enum.GetValues(typeof(Choice))
                              .Cast<Choice>()
                              .Select(c => new { id = (int)c, name = c.ToDisplayName() });
            return Ok(choices);
        }

        [HttpGet("randomchoice")]
        public async Task<IActionResult> GetRandomChoice()
        {
            var rnd = await _randomNumberService.GetRandomNumberAsync();
            var choice = (Choice)((rnd % 5) + 1);
            return Ok(new { id = (int)choice, name = choice.ToDisplayName() });
        }

        [HttpPost("play")]
        public IActionResult Play([FromBody] PlayRequestDTO request)
        {
            var roundResult = _gameService.PlayRound(request.UserId, request.PlayerChoice);
            return Ok(roundResult);
        }

        [HttpGet("scoreboard")]
        public IActionResult GetScoreboard([FromQuery] Guid userId)
        {
            var board = _gameService.GetScoreboard(userId)
                         .Select(r => new { player = (int)r.Player, computer = (int)r.Computer, result = r.Result });
            return Ok(board);
        }

        [HttpPost("scoreboard/reset")]
        public IActionResult ResetScoreboard([FromQuery] Guid userId)
        {
            _gameService.ResetScoreboard(userId);
            return NoContent();
        }
    }
}

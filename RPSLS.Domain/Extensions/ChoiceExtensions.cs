using RPSLS.Domain.Entities;

namespace RPSLS.Domain.Extensions;

public static class ChoiceExtensions
{
    public static string ToDisplayName(this Choice choice) => choice.ToString().ToLower();

    /// <summary>
    /// Determines if the player's choice beats the opponent's choice according to RPSLS rules.
    /// </summary>
    public static bool IsWinnerAgainst(this Choice player, Choice opponent)
    {
        // Each choice beats two other choices
        var winnerMap = new Dictionary<Choice, Choice[]>
        {
            { Choice.Rock,     new[] { Choice.Scissors, Choice.Lizard } },   // Rock wins against Scissors and Lizard
            { Choice.Paper,    new[] { Choice.Rock, Choice.Spock } },        // Paper wins against Rock and Spock
            { Choice.Scissors, new[] { Choice.Paper, Choice.Lizard } },      // Scissors wins against Paper and Lizard
            { Choice.Lizard,   new[] { Choice.Spock, Choice.Paper } },       // Lizard wins against Spock and Paper
            { Choice.Spock,    new[] { Choice.Scissors, Choice.Rock } }      // Spock wins against Scissors and Rock
        };

        // Return true if the opponent is in the list of choices beaten by the player
        return winnerMap.TryGetValue(player, out var choicesBeaten) && choicesBeaten.Contains(opponent);
    }
}
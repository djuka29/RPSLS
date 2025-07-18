using Microsoft.Data.Sqlite;
using RPSLS.Application.Interfaces;
using RPSLS.Domain.Entities;
using RPSLS.Domain.Enums;

namespace RPSLS.Infrastructure.Repositories
{
    public class ScoreboardRepository : IScoreboardRepository
    {
        private readonly string _connectionString;

        public ScoreboardRepository(string connectionString)
        {
            _connectionString = connectionString;
            EnsureTable();
        }

        private void EnsureTable()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = """
            CREATE TABLE IF NOT EXISTS Scoreboard (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId TEXT NOT NULL,
                Player INTEGER NOT NULL,
                Computer INTEGER NOT NULL,
                Result INTEGER NOT NULL
        );
        """;
            cmd.ExecuteNonQuery();
        }

        public void AddRoundResult(Guid userId, RoundResult roundResult)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Scoreboard (UserId, Player, Computer, Result) VALUES (@UserId, @Player, @Computer, @Result)";
            cmd.Parameters.AddWithValue("@UserId", userId.ToString());
            cmd.Parameters.AddWithValue("@Player", (int)roundResult.Player);
            cmd.Parameters.AddWithValue("@Computer", (int)roundResult.Computer);
            cmd.Parameters.AddWithValue("@Result", (int)roundResult.Result);
            cmd.ExecuteNonQuery();
        }

        public List<RoundResult> GetScoreboard(Guid userId)
        {
            var results = new List<RoundResult>();
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT Player, Computer, Result FROM Scoreboard WHERE UserId = @UserId ORDER BY rowid DESC LIMIT 10";
            cmd.Parameters.AddWithValue("@UserId", userId.ToString());
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                results.Add(new RoundResult
                {
                    Player = (Choice)reader.GetInt32(0),
                    Computer = (Choice)reader.GetInt32(1),
                    Result = (RoundResultType)reader.GetInt32(2)
                });
            }
            results.Reverse(); // To keep the order as oldest to newest
            return results;
        }

        public void ResetScoreboard(Guid userId)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"DELETE FROM Scoreboard WHERE UserId = @UserId";
            cmd.Parameters.AddWithValue("@UserId", userId.ToString());
            cmd.ExecuteNonQuery();
        }
    }
}

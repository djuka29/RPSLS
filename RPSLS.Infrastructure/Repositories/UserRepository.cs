using Microsoft.Data.Sqlite;
using RPSLS.Application.Interfaces;
using RPSLS.Domain.Entities;

namespace RPSLS.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly string _connectionString;

    public UserRepository(string connectionString)
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
            CREATE TABLE IF NOT EXISTS Users (
                Id TEXT PRIMARY KEY,
                Username TEXT UNIQUE,
                HashedPassword TEXT
            );
        """;
        cmd.ExecuteNonQuery();
    }

    public void Add(User user)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "INSERT INTO Users (Id, Username, HashedPassword) VALUES ($id, $username, $password)";
        cmd.Parameters.AddWithValue("$id", user.Id.ToString());
        cmd.Parameters.AddWithValue("$username", user.Username);
        cmd.Parameters.AddWithValue("$password", user.HashedPassword);
        cmd.ExecuteNonQuery();
    }

    public User? GetById(Guid id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT Id, Username, HashedPassword FROM Users WHERE Id = $id";
        cmd.Parameters.AddWithValue("$id", id.ToString());
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new User
            {
                Id = Guid.Parse(reader.GetString(0)),
                Username = reader.GetString(1),
                HashedPassword = reader.GetString(2)
            };
        }
        return null;
    }

    public User? GetByUsername(string username)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT Id, Username, HashedPassword FROM Users WHERE Username = $username";
        cmd.Parameters.AddWithValue("$username", username);
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new User
            {
                Id = Guid.Parse(reader.GetString(0)),
                Username = reader.GetString(1),
                HashedPassword = reader.GetString(2)
            };
        }
        return null;
    }

    public IEnumerable<User> GetAll()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT Id, Username, HashedPassword FROM Users";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            yield return new User
            {
                Id = Guid.Parse(reader.GetString(0)),
                Username = reader.GetString(1),
                HashedPassword = reader.GetString(2)
            };
        }
    }
}
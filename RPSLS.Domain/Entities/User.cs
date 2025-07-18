namespace RPSLS.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string HashedPassword { get; set; } = string.Empty;
}
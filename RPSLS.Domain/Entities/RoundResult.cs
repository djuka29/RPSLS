using RPSLS.Domain.Enums;

namespace RPSLS.Domain.Entities;

public class RoundResult
{
    public Choice Player { get; set; }
    public Choice Computer { get; set; }
    public RoundResultType Result { get; set; }
}
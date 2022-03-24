namespace GreatLeaderXiBot.Common.Outlook.Dtos;

public class OutlookAppointmentDto
{
    public string Subject { get; init; }
    
    public string Location { get; init; }

    public DateTime Start { get; init; }

    public DateTime End { get; init; }
}

namespace GreatLeaderXiBot.Common.Outlook.Dtos;

// Disable "Non-nullable property must contain a non-null value when exiting constructor. Consider declaring the property as nullable."
#pragma warning disable 8618

public class OutlookAppointmentDto
{
    public string Subject { get; init; }
    
    public string Location { get; init; }

    public DateTime Start { get; init; }

    public DateTime End { get; init; }
}

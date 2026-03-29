namespace NoviVovi.Infrastructure.DatabaseObjects.Labels;

public class ReplicaDbO
{
    public Guid Id { get; set; }
    public Guid? SpeakerId { get; set; }
    public string? Text { get; set; }
}

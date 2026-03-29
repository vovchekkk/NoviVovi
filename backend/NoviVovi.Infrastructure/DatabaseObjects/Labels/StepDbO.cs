namespace NoviVovi.Infrastructure.DatabaseObjects.Labels;

public class StepDbO
{
    public Guid Id { get; set; }
    public Guid LabelId { get; set; }
    public Guid? ReplicaId { get; set; }
    public Guid? MenuId { get; set; }
    public Guid? BgId { get; set; }
    public Guid? NextLabelId { get; set; }
    public int StepOrder { get; set; }
    public string? StepType { get; set; }
}
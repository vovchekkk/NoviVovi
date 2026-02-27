namespace NoviVovi.Api.Contracts.Novels.Requests;

public class AddSlideRequest
{
    public int Number { get; set; }
    public string Text { get; set; } = null!;
}
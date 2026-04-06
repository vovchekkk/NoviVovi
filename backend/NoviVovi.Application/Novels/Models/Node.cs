using NoviVovi.Domain.Images;

namespace NoviVovi.Application.Novels.Models;

public class Node
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
}
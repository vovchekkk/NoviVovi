using MediatR;
using NoviVovi.Application.Characters.Dtos;
using NoviVovi.Application.Characters.Mappers;
using NoviVovi.Application.Common;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Novels;
using NoviVovi.Application.Novels.Abstractions;
using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Common;

namespace NoviVovi.Application.Characters.Features.Add;

public record AddCharacterCommand : IRequest<CharacterDto>
{
    public required Guid NovelId { get; init; }
    public required string Name { get; init; }
    public required string NameColor { get; init; }
    public string? Description { get; init; }
}

public class AddCharacterHandler(
    INovelRepository novelRepository,
    IUnitOfWork unitOfWork,
    CharacterDtoMapper mapper
) : IRequestHandler<AddCharacterCommand, CharacterDto>
{
    public async Task<CharacterDto> Handle(AddCharacterCommand request, CancellationToken ct)
    {
        unitOfWork.BeginTransaction();
        
        try
        {
            var novel = await novelRepository.GetByIdAsync(request.NovelId, ct)
                        ?? throw new NotFoundException($"Новелла '{request.NovelId}' не найдена");

            var colorName = Color.FromHex(request.NameColor);

            var character = Character.Create(request.Name, request.NovelId, colorName, request.Description);

            novel.AddCharacter(character);
            
            await novelRepository.AddOrUpdateAsync(novel, ct);
            
            await unitOfWork.CommitAsync(ct);

            return mapper.ToDto(character);
        }
        catch
        {
            await unitOfWork.RollbackAsync(ct);
            throw;
        }
    }
}
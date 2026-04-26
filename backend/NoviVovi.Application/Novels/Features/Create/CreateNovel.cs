using MediatR;
using NoviVovi.Application.Common;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Labels.Abstractions;
using NoviVovi.Application.Novels.Abstractions;
using NoviVovi.Application.Novels.Dtos;
using NoviVovi.Application.Novels.Mappers;
using NoviVovi.Domain.Novels;

namespace NoviVovi.Application.Novels.Features.Create;

public record CreateNovelCommand : IRequest<NovelDto>
{
    public required string Title { get; init; }
}

public class CreateNovelHandler(
    INovelRepository novelRepository,
    ILabelRepository labelRepository,
    IUnitOfWork unitOfWork,
    NovelDtoMapper mapper
) : IRequestHandler<CreateNovelCommand, NovelDto>
{
    public async Task<NovelDto> Handle(CreateNovelCommand request, CancellationToken ct)
    {
        unitOfWork.BeginTransaction();
        
        try
        {
            const string startLabelName = "start";
            
            // 1. Создаем Novel БЕЗ StartLabel
            var novel = Novel.Create(request.Title);
            await novelRepository.AddOrUpdateAsync(novel, ct);
            
            // 2. Инициализируем StartLabel (теперь Novel уже есть в БД)
            var label = novel.InitializeStartLabel(startLabelName);
            await labelRepository.AddOrUpdateAsync(label, ct);
            
            // 3. Обновляем Novel с StartLabelId
            await novelRepository.AddOrUpdateAsync(novel, ct);
            
            await unitOfWork.CommitAsync(ct);
            
            return mapper.ToDto(novel);
        }
        catch
        {
            await unitOfWork.RollbackAsync(ct);
            throw;
        }
    }
}
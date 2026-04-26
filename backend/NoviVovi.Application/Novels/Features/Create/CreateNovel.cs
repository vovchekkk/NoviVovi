using MediatR;
using NoviVovi.Application.Common;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Labels.Abstractions;
using NoviVovi.Application.Novels.Abstractions;
using NoviVovi.Application.Novels.Dtos;
using NoviVovi.Application.Novels.Mappers;
using NoviVovi.Domain.Labels;
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
            const string startLabelName = "StartLabel";
            
            var novel = Novel.Create(request.Title);
            
            await novelRepository.AddOrUpdateAsync(novel, ct);
            
            var label = novel.SetStartLabel(startLabelName);
            
            await labelRepository.AddOrUpdateAsync(label, ct);
            
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
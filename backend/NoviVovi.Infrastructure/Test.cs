using Npgsql;

namespace NoviVovi.Infrastructure;

using System;
using System.Threading.Tasks;

public class Test
{
    private readonly NovelDatabaseService _dbService;

    public Test(NovelDatabaseService dbService)
    {
        _dbService = dbService;
    }

    public async Task TestDb()
    {
        await GetAllPublicNovelsExample();
        await GetNovelByIdExample(Guid.Parse("2a66792c-aa2a-45cd-86fb-dae3c621a6a5"));
        await GetFullNovelExample(Guid.Parse("2a66792c-aa2a-45cd-86fb-dae3c621a6a5"));
        await GetLabelStepsExample(Guid.Parse("2a66792c-aa2a-45cd-86fb-dae3c621a6a5"));
    }

    private async Task GetAllPublicNovelsExample()
    {
        try
        {
            var novels = await _dbService.GetAllNovelsAsync(onlyPublic: true);
            
            Console.WriteLine($"\n📚 Публичные новеллы ({novels.Count()}):");
            foreach (var novel in novels)
            {
                Console.WriteLine($"  - {novel.Title} (ID: {novel.Id})");
                Console.WriteLine($"    Создана: {novel.CreatedAt:dd.MM.yyyy}");
                var description = novel.Description ?? "Нет описания";
                var shortDesc = description.Length > 50 
                    ? description[..50] + "..." 
                    : description;
                Console.WriteLine($"    Описание: {shortDesc}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Ошибка при получении новелл: {ex.Message}");
        }
    }

    private async Task GetNovelByIdExample(Guid novelId)
    {
        try
        {
            var novel = await _dbService.GetNovelByIdAsync(novelId);
            
            if (novel != null)
            {
                Console.WriteLine($"\n📖 Новелла: {novel.Title}");
                Console.WriteLine($"  ID: {novel.Id}");
                Console.WriteLine($"  Автор: {novel.UserId}");
                Console.WriteLine($"  Публичная: {novel.IsPublic}");
                Console.WriteLine($"  Описание: {novel.Description ?? "Нет описания"}");
                Console.WriteLine($"  Создана: {novel.CreatedAt:dd.MM.yyyy HH:mm}");
                Console.WriteLine($"  Изменена: {novel.EditedAt:dd.MM.yyyy HH:mm}");
            }
            else
            {
                Console.WriteLine($"\n❌ Новелла с ID {novelId} не найдена");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Ошибка при получении новеллы: {ex.Message}");
        }
    }

    private async Task GetFullNovelExample(Guid novelId)
    {
        try
        {
            var fullNovel = await _dbService.GetFullNovelByIdAsync(novelId);
            
            if (fullNovel != null)
            {
                Console.WriteLine($"\n📖 Полная информация о новелле: {fullNovel.Novel.Title}");
                Console.WriteLine($"  Персонажей: {fullNovel.Characters.Count}");
                Console.WriteLine($"  Сцен: {fullNovel.Labels.Count}");
                Console.WriteLine($"  Изображений: {fullNovel.Images.Count}");
                
                if (fullNovel.Characters.Any())
                {
                    Console.WriteLine("\n  Персонажи:");
                    foreach (var character in fullNovel.Characters)
                    {
                        Console.WriteLine($"    - {character.Name} (#{character.NameColor})");
                        if (!string.IsNullOrEmpty(character.Description))
                        {
                            var desc = character.Description.Length > 60 
                                ? character.Description[..60] + "..." 
                                : character.Description;
                            Console.WriteLine($"      {desc}");
                        }
                    }
                }
                
                if (fullNovel.Labels.Any())
                {
                    Console.WriteLine("\n  Сцены:");
                    foreach (var label in fullNovel.Labels)
                    {
                        Console.WriteLine($"    - {label.LabelName}");
                    }
                }
            }
            else
            {
                Console.WriteLine($"\n❌ Полная информация о новелле {novelId} не найдена");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Ошибка при получении полной информации: {ex.Message}");
        }
    }

    private async Task GetLabelStepsExample(Guid labelId)
    {
        try
        {
            var steps = await _dbService.GetStepsByLabelIdAsync(labelId);
            
            Console.WriteLine($"\n🎬 Шаги сцены (всего: {steps.Count()}):");
            foreach (var step in steps)
            {
                Console.WriteLine($"  [{step.StepOrder}] Тип: {step.StepType ?? "unknown"}");
                
                if (step.ReplicaId.HasValue)
                {
                    var replica = await _dbService.GetReplicaByIdAsync(step.ReplicaId.Value);
                    if (replica != null)
                    {
                        var text = string.IsNullOrEmpty(replica.Text) 
                            ? "Нет текста" 
                            : (replica.Text.Length > 50 ? replica.Text[..50] + "..." : replica.Text);
                        Console.WriteLine($"      Реплика: {text}");
                        
                        if (replica.SpeakerId.HasValue)
                        {
                            Console.WriteLine($"      Говорит: {replica.SpeakerId}");
                        }
                    }
                }
                
                if (step.MenuId.HasValue)
                {
                    var menu = await _dbService.GetFullMenuByIdAsync(step.MenuId.Value);
                    if (menu != null)
                    {
                        Console.WriteLine($"      Меню: {menu.Menu.Name} (выборов: {menu.Choices.Count})");
                        foreach (var choice in menu.Choices)
                        {
                            Console.WriteLine($"        - {choice.Name}: {choice.Text}");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Ошибка при получении шагов: {ex.Message}");
        }
    }

    public void Dispose()
    {
        _dbService?.Dispose();
    }
}

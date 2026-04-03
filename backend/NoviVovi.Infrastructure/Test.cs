using NoviVovi.Infrastructure.DatabaseService;
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
        var novelId = Guid.Parse("2a66792c-aa2a-45cd-86fb-dae3c621a6a5");
        
        await GetAllPublicNovelsExample();
        await GetNovelByIdExample(novelId);
        await GetFullNovelExample(novelId);
        
        // Получаем первую метку новеллы, чтобы показать её шаги
        var labels = await _dbService.GetLabelsByNovelIdAsync(novelId);
        var firstLabelId = labels.FirstOrDefault()?.Id;
        if (firstLabelId.HasValue)
        {
            await GetLabelStepsExample(firstLabelId.Value);
        }
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
            // Используем GetNovelByIdAsync для плоской новеллы
            var novel = await _dbService.GetNovelByIdAsync(novelId);
            
            if (novel != null)
            {
                Console.WriteLine($"\n📖 Новелла (плоская): {novel.Title}");
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
                Console.WriteLine($"\n📖 Полная информация о новелле: {fullNovel.Title}");
                Console.WriteLine($"  Сцен: {fullNovel.Labels.Count}");
                
                if (fullNovel.Labels.Any())
                {
                    Console.WriteLine("\n  Сцены:");
                    foreach (var label in fullNovel.Labels)
                    {
                        Console.WriteLine($"    - {label.LabelName} (ID: {label.Id})");
                        
                        // Показываем количество шагов в сцене
                        if (label.Steps != null && label.Steps.Any())
                        {
                            Console.WriteLine($"      Шагов: {label.Steps.Count}");
                        }
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
            // Используем GetFullStepByIdAsync для получения полных шагов
            var steps = await _dbService.GetStepsByLabelIdAsync(labelId);
            
            Console.WriteLine($"\n🎬 Шаги сцены (ID: {labelId}, всего: {steps.Count()}):");
            
            foreach (var step in steps)
            {
                // Загружаем полный шаг со всеми зависимостями
                var fullStep = await _dbService.GetFullStepByIdAsync(step.Id);
                
                if (fullStep == null) continue;
                
                Console.WriteLine($"\n  [{fullStep.StepOrder}] Тип: {fullStep.StepType ?? "unknown"} (ID: {fullStep.Id})");
                
                // Реплика
                if (fullStep.Replica != null)
                {
                    var text = string.IsNullOrEmpty(fullStep.Replica.Text) 
                        ? "Нет текста" 
                        : (fullStep.Replica.Text.Length > 100 
                            ? fullStep.Replica.Text[..100] + "..." 
                            : fullStep.Replica.Text);
                    Console.WriteLine($"      Реплика: {text}");
                    
                    if (fullStep.Replica.Speaker != null)
                    {
                        Console.WriteLine($"      Говорит: {fullStep.Replica.Speaker.Name} (ID: {fullStep.Replica.Speaker.Id})");
                    }
                    else if (fullStep.Replica.SpeakerId.HasValue)
                    {
                        Console.WriteLine($"      Говорит: персонаж {fullStep.Replica.SpeakerId}");
                    }
                }
                
                // Меню
                if (fullStep.Menu != null)
                {
                    Console.WriteLine($"      Меню: {fullStep.Menu.Name} (выборов: {fullStep.Menu.Choices.Count})");
                    foreach (var choice in fullStep.Menu.Choices)
                    {
                        var nextLabelName = choice.NextLabel?.LabelName ?? "неизвестно";
                        Console.WriteLine($"        - {choice.Name}: {choice.Text} -> {nextLabelName}");
                    }
                }
                
                // Фон
                if (fullStep.Background != null && fullStep.Background.Image != null)
                {
                    Console.WriteLine($"      Фон: {fullStep.Background.Image.Name} ({fullStep.Background.Image.Url})");
                }
                
                // Персонажи на сцене
                if (fullStep.StepCharacters != null && fullStep.StepCharacters.Any())
                {
                    Console.WriteLine($"      Персонажи на сцене: {fullStep.StepCharacters.Count}");
                    foreach (var sc in fullStep.StepCharacters)
                    {
                        var characterName = sc.CharacterState != null 
                            ? $"{sc.CharacterState.StateName}" 
                            : sc.CharacterStateId.ToString();
                        Console.WriteLine($"        - Состояние: {characterName}");
                        
                        if (sc.Transform != null)
                        {
                            Console.WriteLine($"          Позиция: X={sc.Transform.XPos}, Y={sc.Transform.YPos}, Scale={sc.Transform.Scale}");
                        }
                    }
                }
                
                // Следующая метка
                if (fullStep.NextLabel != null)
                {
                    Console.WriteLine($"      Следующая сцена: {fullStep.NextLabel.LabelName}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Ошибка при получении шагов: {ex.Message}");
            Console.WriteLine($"   StackTrace: {ex.StackTrace}");
        }
    }

    public void Dispose()
    {
        _dbService?.Dispose();
    }
}
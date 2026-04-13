using NoviVovi.Domain.Common;
using NoviVovi.Domain.Images;
using NoviVovi.Domain.Scene;
using NoviVovi.Domain.Transitions;

namespace NoviVovi.Domain.Steps;

public class ShowBackgroundStep : Step
{
    public BackgroundObject BackgroundObject { get; private set; }

    private ShowBackgroundStep(
        Guid id,
        BackgroundObject backgroundObject,
        NextStepTransition transition
    ) : base(id, transition)
    {
        BackgroundObject = backgroundObject;
    }

    public static ShowBackgroundStep Create(
        BackgroundObject? backgroundObject
    )
    {
        if (backgroundObject is null)
            throw new DomainException($"BackgroundObject cannot be null");
        
        return new ShowBackgroundStep(Guid.NewGuid(), backgroundObject, NextStepTransition.Create());
    }
    
    public void Update(Image? image, TransformPatch? transformPatch)
    {
        if (image is not null) 
            BackgroundObject.UpdateImage(image);

        if (transformPatch is not null)
            BackgroundObject.PatchTransform(transformPatch);
    }
    
    public new NextStepTransition Transition => (NextStepTransition)base.Transition;
}
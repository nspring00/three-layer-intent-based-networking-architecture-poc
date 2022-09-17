using FastEndpoints;
using FluentValidation;
using Knowledge.API.Endpoints.Intents;
using Knowledge.API.Models;

namespace Knowledge.API.Validators;

public class UpdateIntentRequestValidator : Validator<UpdateIntentRequest>
{
    public UpdateIntentRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .GreaterThan(0);
        
        RuleFor(x => x.Region)
            .NotEmpty();

        RuleFor(x => x.Kpi)
            .NotEmpty()
            .IsEnumName(typeof(KeyPerformanceIndicator), false);
        
        RuleFor(x => x.TargetMode)
            .NotEmpty()
            .IsEnumName(typeof(TargetMode), false);

        RuleFor(x => x.Value)
            .NotEmpty()
            .InclusiveBetween(0f, 1f);
    }
}

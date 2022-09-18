using FastEndpoints;
using FluentValidation;
using Knowledge.API.Contracts.Requests;
using Knowledge.API.Models;

namespace Knowledge.API.Validators;

public class CreateIntentRequestValidator : Validator<CreateIntentRequest>
{
    public CreateIntentRequestValidator()
    {
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

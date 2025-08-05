using FluentValidation;
using BaseAuth.Domain.Common;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BaseAuth.Application.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : Result
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                var validationResults = await Task.WhenAll(
                    _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

                var failures = validationResults
                    .Where(r => r.Errors.Any())
                    .SelectMany(r => r.Errors)
                    .ToList();

                if (failures.Any())
                {
                    var errors = failures.Select(x => x.ErrorMessage).ToList();
                    
                    // Create Result.Failure with errors
                    var resultType = typeof(TResponse);
                    
                    if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Result<>))
                    {
                        var dataType = resultType.GetGenericArguments()[0];
                        var method = typeof(Result)
                            .GetMethods()
                            .First(m => m.Name == "Failure" && m.IsGenericMethodDefinition && m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType == typeof(List<string>))
                            .MakeGenericMethod(dataType);
                        
                        return (TResponse)method.Invoke(null, new object[] { errors });
                    }
                    else
                    {
                        var method = typeof(Result)
                            .GetMethods()
                            .First(m => m.Name == "Failure" && !m.IsGenericMethodDefinition && m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType == typeof(List<string>));
                        
                        return (TResponse)method.Invoke(null, new object[] { errors });
                    }
                }
            }

            return await next();
        }
    }
} 
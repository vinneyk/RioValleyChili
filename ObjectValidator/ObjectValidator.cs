using System;
using System.Collections.Generic;
using Solutionhead.Services;

namespace ObjectValidator
{
    public class ObjectValidator<TObject> : IValidator<TObject> where TObject : class
    {
        private readonly IEnumerable<Func<TObject, IResult>> _rules;

        public ObjectValidator(IEnumerable<Func<TObject, IResult>> rules)
        {
            _rules = rules;
        }

        public IResult Validate(TObject objectToValidate)
        {
            foreach (var func in _rules)
            {
                var result = func(objectToValidate);
                if (result.State != ResultState.Success)
                {
                    return result;
                }
            }

            return new SuccessResult();
        }
    }
}
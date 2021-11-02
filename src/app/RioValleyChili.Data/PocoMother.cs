using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RioValleyChili.Data
{
    //todo: extract into Solutionhead.Data project

    public class PocoMother<TChild> : IMother<TChild> where TChild : class, new()
    {
        public IEnumerable<TChild> BirthAll(Action consoleCallback = null)
        {
            var properties = GetType().GetProperties().Where(p => p.PropertyType == typeof(TChild));
            return properties.Select(p => p.GetValue(this, BindingFlags.GetProperty, null, null, null) as TChild);
        }
    }
}
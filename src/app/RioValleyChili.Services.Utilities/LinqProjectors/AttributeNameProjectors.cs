using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class AttributeNameProjectors
    {
        internal static Expression<Func<AttributeName, AttributeNameKeyReturn>> SelectKey()
        {
            return a => new AttributeNameKeyReturn
                {
                    AttributeNameKey_ShortName = a.ShortName
                };
        }

        internal static Expression<Func<AttributeName, AttributeNameAndType>> Select()
        {
            return a => new AttributeNameAndType
                {
                    Key = a.ShortName,
                    Name = a.Name,
                    ValidForChile = a.ValidForChileInventory,
                    ValidForAdditive = a.ValidForAdditiveInventory,
                    ValidForPackaging = a.ValidForPackagingInventory
                };
        }

        internal static Expression<Func<IEnumerable<AttributeNameAndType>>> SelectActiveAttributeNames(ILotUnitOfWork lotUnitOfWork)
        {
            var attributes = lotUnitOfWork.AttributeNameRepository.Filter(a => a.Active);
            var selector = Select();

            return () => attributes.Select(a => selector.Invoke(a));
        }
    }
}
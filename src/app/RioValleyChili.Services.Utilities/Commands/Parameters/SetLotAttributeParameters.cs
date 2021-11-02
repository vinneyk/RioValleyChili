using System.Collections.Generic;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.LotService;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class SetLotAttributeParameters : ISetLotStatusParameters
    {
        public ISetLotAttributeParameters Parameters { get; set; }

        public LotKey LotKey { get; set; }
        public Dictionary<AttributeNameKey, IAttributeValueParameters> Attributes { get; set; }

        #region ISetLotStatusParameters
#warning Remove if/when 'OverrideOldContextLotAsCompleted' is removed. -RI 6/16/2014

        string ISetLotStatusParameters.LotKey { get { return Parameters.LotKey; } }
        LotQualityStatus ISetLotStatusParameters.QualityStatus { get { return LotQualityStatus.Released; } }
        string IUserIdentifiable.UserToken { get { return Parameters.UserToken; } set { } }

        #endregion
    }
}
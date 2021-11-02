using System;
using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.MillAndWetdownService;

namespace RioValleyChili.Services.Models.Parameters
{
    public class CreateMillAndWetdownParameters : ICreateMillAndWetdownParameters
    {
        public DateTime ProductionDate { get; set; }
        public int LotSequence { get; set; }
        public string ShiftKey { get; set; }
        public string ChileProductKey { get; set; }
        public string ProductionLineKey { get; set; }
        public DateTime ProductionBegin { get; set; }
        public DateTime ProductionEnd { get; set; }
        public IEnumerable<MillAndWetdownResultItemParameters> ResultItems { get; set; }
        public IEnumerable<MillAndWetdownPickedItemParameters> PickedItems { get; set; }

        IEnumerable<IMillAndWetdownResultItemParameters> ISetMillAndWetdownParameters.ResultItems { get { return ResultItems; } }
        IEnumerable<IMillAndWetdownPickedItemParameters> ISetMillAndWetdownParameters.PickedItems { get { return PickedItems; } }

        string IUserIdentifiable.UserToken { get; set; }
    }

    public class UpdateMillAndWetdownParameters : IUpdateMillAndWetdownParameters
    {
        public string LotKey { get; set; }
        public string ChileProductKey { get; set; }
        public string ShiftKey { get; set; }
        public string ProductionLineKey { get; set; }
        public DateTime ProductionBegin { get; set; }
        public DateTime ProductionEnd { get; set; }
        public IEnumerable<MillAndWetdownResultItemParameters> ResultItems { get; set; }
        public IEnumerable<MillAndWetdownPickedItemParameters> PickedItems { get; set; }

        IEnumerable<IMillAndWetdownResultItemParameters> ISetMillAndWetdownParameters.ResultItems { get { return ResultItems; } }
        IEnumerable<IMillAndWetdownPickedItemParameters> ISetMillAndWetdownParameters.PickedItems { get { return PickedItems; } }

        string IUserIdentifiable.UserToken { get; set; }
    }

    public class MillAndWetdownResultItemParameters : IMillAndWetdownResultItemParameters
    {
        public string LocationKey { get; set; }
        public int Quantity { get; set; }
        public string PackagingProductKey { get; set; }
    }

    public class MillAndWetdownPickedItemParameters : IMillAndWetdownPickedItemParameters
    {
        public string InventoryKey { get; set; }
        public int Quantity { get; set; }
    }
}
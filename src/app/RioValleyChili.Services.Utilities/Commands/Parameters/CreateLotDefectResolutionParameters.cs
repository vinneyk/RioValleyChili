using System;
using RioValleyChili.Core;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class CreateLotDefectResolutionParameters
    {
        internal Employee Employee { get; set; }

        internal DateTime TimeStamp { get; set; }

        internal LotDefect LotDefect { get; set; }

        internal ResolutionTypeEnum ResolutionType { get; set; }

        internal string Description { get; set; }
    }
}
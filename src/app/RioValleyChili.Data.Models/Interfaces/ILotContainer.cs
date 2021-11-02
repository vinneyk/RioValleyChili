using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.Models.Interfaces
{
    public interface ILotContainer : ILotKey
    {
        [ForeignKey("LotDateCreated, LotDateSequence, LotTypeId")]
        Lot Lot { get; set; }
    }
}

using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Utilities.Models
{
    public class PickedAdditiveInventoryItem
    {
        public PickedInventoryItem PickedInventoryItem { get; set; }
        public AdditiveLot AdditiveLot { get; set; }
        public AdditiveProduct AdditiveProduct { get; set; }
        public AdditiveType AdditiveType { get; set; }
        public PackagingProduct Packaging { get; set; }
    }

    public class PickedChileInventoryItem
    {
        public PickedInventoryItem PickedInventoryItem { get; set; }
        public ChileLot ChileLot { get; set; }
        public PackagingProduct Packaging { get; set; }
    }
    
    public class PickedPackagingInventoryItem 
    {
        public PickedInventoryItem PickedInventoryItem { get; set; }
        public PackagingLot PackagingLot { get; set; }
        public PackagingProduct PackagingProduct { get; set; }
        public Product Product { get; set; }
    }
}
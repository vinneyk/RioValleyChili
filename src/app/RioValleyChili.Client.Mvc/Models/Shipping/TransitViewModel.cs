using System.ComponentModel.DataAnnotations;
using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;

namespace RioValleyChili.Client.Mvc.Models.Shipping
{
    public class TransitViewModel : ITransitInformation, IOptionalViewModelComponent
    {
        #region fields and constructors

        private readonly bool _isEmpty;

        public TransitViewModel() : this(false) { }

        private TransitViewModel(bool isEmpty)
        {
            _isEmpty = isEmpty;
        }

        #endregion

        [Display(Name = "Shipment Method")]
        public string ShipmentMethod { get; set; }
        
        [Display(Name = "Freight Type")]
        public string FreightType { get; set; }

        [Display(Name = "Driver Name"), Required]
        public string DriverName { get; set; }

        [Display(Name = "Carrier Name"), Required]
        public string CarrierName { get; set; }

        [Display(Name = "Trailer License #"), Required]
        public string TrailerLicenseNumber { get; set; }

        [Display(Name = "Container Seal"), Required]
        public string ContainerSeal { get; set; }
        
        [Display(AutoGenerateField = false)]
        public bool InitializedAsEmpty { get { return _isEmpty; } }

        public static TransitViewModel AsEmpty
        {
            get { return new TransitViewModel(true) { DriverName = "Headless Horseman"}; }
        }

        public static TransitViewModel Create(ITransitInformation source)
        {
            return new TransitViewModel(source == null || (
                source.CarrierName == null 
                && source.ContainerSeal == null 
                && source.DriverName == null
                && source.FreightType == null
                && source.TrailerLicenseNumber == null
                && source.ShipmentMethod == null));
        }
    }
}
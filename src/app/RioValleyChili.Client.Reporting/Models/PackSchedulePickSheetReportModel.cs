using System;
using RioValleyChili.Core.Helpers;

namespace RioValleyChili.Client.Reporting.Models
{
    public class PackSchedulePickSheetReportModel
    {
        public string ProductName { get; set; }
        public string PackScheduleKey { get; set; }
        public string PSNum { get; set; }
        public DateTime PackScheduleDate { get; set; }
        public string BatchType { get; set; }
        public string PackScheduleDescription { get; set; }

        public string BatchLotNumber { get; set; }
        public string PickedLotNumber { get; set; }
        public string PickedProductName { get; set; }
        public string PickedProductKey { get; set; }
        public string WarehouseLocation { get; set; }
        public string WarehouseLocationStreet { get { return _street; } }
        public int WarehouseLocationRow { get { return _row; } }
        public string PackagingName { get; set; }
        public string Treatment { get; set; }
        public bool LoBac { get; set; }
        public int QuantityPicked { get; set; }
        public double PoundsPicked { get; set; }

        public PackSchedulePickSheetReportModel Initialize()
        {
            if(LocationDescriptionHelper.GetStreetRow(WarehouseLocation, out _street, out _row))
            {
                WarehouseLocation = LocationDescriptionHelper.ToDisplayString(_street, _row);
            }
            else
            {
                _street = null;
                _row = 0;
            }

            return this;
        }

        private string _street;
        private int _row;
    }
}
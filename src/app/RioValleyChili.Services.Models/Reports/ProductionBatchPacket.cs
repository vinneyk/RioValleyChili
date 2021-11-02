using System;
using System.Collections.Generic;

namespace RioValleyChili.Services.Models.Reports
{
    public class ProductionBatchPacket
    {
        public ProductionBatchPacketHeader PacketHeader { get; set; }

        public string BatchNotes { get; set; }

        public ProductionBatchParameters TargetParameters { get; set; }

        public ProductionBatchParameters CalculatedParameters { get; set; }

        public IEnumerable<ProductionBatchMaterial> ProductionBatchMaterials { get; set; }

        public IEnumerable<ProductionBatchInstruction> BatchInstructions { get; set; }
    }

    public class ProductionBatchParameters
    {
        public int Weight { get; set; }

        public int Asta { get; set; }

        public int Scoville { get; set; }

        public int Scan { get; set; }
    }

    public class ProductionBatchPacketHeader
    {
        [Obsolete("Use ProductionBatchKey or ProductionBatchOutputLotNumber instead.")]
        public string LotNumber { get; set; }

        public string ProductionBatchKey { get; set; }

        public string ProductionBatchOutputLotNumber { get; set; }

        public string PackScheduleNumber { get; set; }

        public DateTime ScheduledProductionDate { get; set; }

        public string WorkType { get; set; }

        public string ProductName { get; set; }

        public string SummaryOfWork { get; set; }
    }

    public class ProductionBatchMaterial
    {
        public string MaterialType { get; set; }

        public string ProductType { get; set; }

        public string ProductName { get; set; }

        public string LotNumber { get; set; }

        public string PackagingName { get; set; }

        public string TreatmentType { get; set; }

        public int Quantity { get; set; }

        public int TotalWeight { get; set; }

        public string LotStatus { get; set; }

        public string InitialLocation { get; set; }
    }

    public class ProductionBatchInstruction
    {
        public int Step { get; set; }

        public string Instruction { get; set; }
    }
}
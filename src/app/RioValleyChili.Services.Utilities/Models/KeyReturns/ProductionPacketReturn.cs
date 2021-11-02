using System;
using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;
using RioValleyChili.Services.Interfaces.Returns.PackScheduleService;
using RioValleyChili.Services.Interfaces.Returns.ProductService;
using RioValleyChili.Services.Interfaces.Returns.ProductionService;

namespace RioValleyChili.Services.Utilities.Models.KeyReturns
{
    internal class ProductionPacketReturn : IProductionPacketReturn
    {
        public string PackScheduleKey { get { return PackScheduleKeyReturn.PackScheduleKey; } }
        public int? PSNum { get; internal set; }
        public DateTime DateCreated { get; internal set; }
        public string ProductionLineDescription { get; internal set; }
        public string SummaryOfWork { get; internal set; }
        public IProductReturn ChileProduct { get; internal set; }
        public IProductKeyNameReturn PackagingProduct { get; internal set; }
        public IWorkTypeReturn WorkType { get; internal set; }
        public ICompanyHeaderReturn Customer { get; internal set; }
        public IEnumerable<IProductionPacketBatchReturn> Batches { get; internal set; }

        internal PackScheduleKeyReturn PackScheduleKeyReturn { get; set; }
    }
}
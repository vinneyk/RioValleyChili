using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Core;
using RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers;

namespace RioValleyChili.Data.DataSeeders.Utilities
{
    public class PackSchedulePackagingHelper
    {
        public enum PackagingResult
        {
            CouldNotDetermine,
            FromSinglePickedPackaging,
            ResolvedFromMultiplePickedPackaging,
            DeterminedFromResultingLotIncoming,
            DeterminedFromRelabelInputs,
            DeterminedFromRelabelInputsFromDescription,
            ToteInDescription,
            DrumInDescriptionMesh20,
            DrumInDescriptionNotMesh20,
            BagInDescription,
            BoxInDescription,
            DeterminedFromResultingLotInventory,
            DeterminedFromReworkInputs
        }

        /// <summary>
        /// Constructs a PackSchedulePackagingHelper without a list of packaging. DeterminePackaging method may output null packageId.
        /// </summary>
        public PackSchedulePackagingHelper()
        {
            _packaging = null;
        }

        /// <summary>
        /// Constructs a PackSchedulePackagingHelper with a list of packaging. DeterminePackaging method will use supplied packaging to attempt to always output a packageId.
        /// </summary>
        /// <param name="packaging">List of packaging. Expected to include required packaging.</param>
        public PackSchedulePackagingHelper(List<tblPackaging> packaging)
        {
            if(packaging == null) { throw new ArgumentNullException("packaging"); }
            _packaging = new Dictionary<PackagingResult, tblPackaging>
                {
                    { PackagingResult.CouldNotDetermine, packaging.Single(p => p.Packaging.ToUpper() == "NO PACKAGING") },
                    { PackagingResult.ToteInDescription, packaging.Single(p => p.NetWgt == 1000 && p.Packaging.ToUpper().Contains("TOTE")) },
                    { PackagingResult.DrumInDescriptionMesh20, packaging.Single(p => p.NetWgt == 240 && p.Packaging.ToUpper().Contains("DRUM")) },
                    { PackagingResult.DrumInDescriptionNotMesh20, packaging.Single(p => p.NetWgt == 220 && p.Packaging.ToUpper().Contains("DRUM")) },
                    { PackagingResult.BoxInDescription, packaging.Single(p => p.NetWgt == 50 && p.Packaging.ToUpper().Contains("BOX")) },
                    { PackagingResult.BagInDescription, packaging.Single(p => p.NetWgt == 50 && p.Packaging.ToUpper().Contains("BAG")) }
                };
        }

        public PackagingResult DeterminePackaging(PackScheduleEntityObjectMother.PackScheduleDTO packSchedule, out int? packageId, out tblPackaging defaultUsed)
        {
            defaultUsed = null;
            var result = _DeterminePackaging(packSchedule, out packageId);
            if(packageId == null && _packaging != null)
            {
                if(_packaging.TryGetValue(result, out defaultUsed))
                {
                    packageId = defaultUsed.PkgID;
                }
            }
            return result;
        }

        #region Private Parts

        private readonly Dictionary<PackagingResult, tblPackaging> _packaging;

        private static PackagingResult _DeterminePackaging(PackScheduleEntityObjectMother.PackScheduleDTO packSchedule, out int? packageId)
        {
            packageId = null;

            var pickedPackagingItems = packSchedule.BatchItems.Where(b => b.SourceLot.PTypeID == (int?) LotTypeEnum.Packaging).ToList();
            if(pickedPackagingItems.Count == 1)
            {
                packageId = pickedPackagingItems.Single().SourceLot.Product.PkgID;
                return PackagingResult.FromSinglePickedPackaging;
            }

            if(pickedPackagingItems.Count > 1)
            {
                packageId = pickedPackagingItems.GroupBy(b => b.SourceLot.Product.Packaging)
                                                .OrderByDescending(g => (g.Key.NetWgt ?? 0) * (g.Sum(b => b.Quantity ?? 0)))
                                                .First().Key.PkgID;
                return PackagingResult.ResolvedFromMultiplePickedPackaging;
            }

            var resultingLotIncoming = packSchedule.BatchLots.SelectMany(b => b.Incoming).ToList();
            if(resultingLotIncoming.Any())
            {
                packageId = resultingLotIncoming.GroupBy(i => i.PkgID).OrderByDescending(g => g.Sum(i => i.TtlWgt ?? 0)).First().Key;
                return PackagingResult.DeterminedFromResultingLotIncoming;
            }

            var resultingLotInventory = packSchedule.BatchLots.SelectMany(b => b.Inventory).ToList();
            if(resultingLotInventory.Any())
            {
                packageId = resultingLotInventory.GroupBy(i => i.PkgID).OrderByDescending(g => g.Sum(i => i.NetWgt)).First().Key;
                return PackagingResult.DeterminedFromResultingLotInventory;
            }

            if(!string.IsNullOrWhiteSpace(packSchedule.PackSchDesc))
            {
                var desc = packSchedule.PackSchDesc.ToUpper();
                if(desc.Contains("TOTE"))
                {
                    return PackagingResult.ToteInDescription;
                }
                if(desc.Contains("DRUM"))
                {
                    return packSchedule.Product.Mesh == 20 ? PackagingResult.DrumInDescriptionMesh20 : PackagingResult.DrumInDescriptionNotMesh20;
                }
                if(desc.Contains("BAG"))
                {
                    return PackagingResult.BagInDescription;
                }
                if(desc.Contains("BOX"))
                {
                    return PackagingResult.BoxInDescription;
                }
                if(desc.Contains("RELABEL"))
                {
                    packageId = PackageIdFromMostInventoryPicked(packSchedule);
                    return PackagingResult.DeterminedFromRelabelInputsFromDescription;
                }
            }

            if(BatchTypeIDHelper.GetBatchTypeID(packSchedule.BatchTypeID.Value) == BatchTypeID.ReLabel)
            {
                packageId = PackageIdFromMostInventoryPicked(packSchedule);
                return PackagingResult.DeterminedFromRelabelInputs;
            }

            if(BatchTypeIDHelper.GetBatchTypeID(packSchedule.BatchTypeID.Value) == BatchTypeID.Rework)
            {
                packageId = PackageIdFromMostInventoryPicked(packSchedule);
                return PackagingResult.DeterminedFromReworkInputs;
            }

            return PackagingResult.CouldNotDetermine;
        }

        private static int PackageIdFromMostInventoryPicked(PackScheduleEntityObjectMother.PackScheduleDTO packSchedule)
        {
            return packSchedule.BatchItems.GroupBy(b => b.Packaging)
                               .OrderByDescending(g => (g.Key.NetWgt ?? 0) * g.Sum(b => b.Quantity ?? 0))
                               .First().Key.PkgID;
        }

        #endregion

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Ninject;
using RioValleyChili.Core;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.StaticRecords;
using RioValleyChili.Services.Tests.Helpers.DataModelExtensions;
using RioValleyChili.Services.Tests.IntegrationTests.Services.TestBases;
using TechTalk.SpecFlow;

namespace RioValleyChili.Services.Tests
{
    [Binding]
    public sealed class SharedContext : ServiceIntegrationTestBase
    {
        [Given(@"inventory from a lot in any of the following states")]
        public void GivenInventoryInAnyOfTheFollowingStates(List<Action<Inventory>[]> createInventory)
        {
            createInventory.ForEach(i => CreateInventory(i));
        }

        [Given(@"inventory with the following attributes")]
        public void GivenInventoryWithTheFollowingAttributes(CreateParameters createParameters)
        {
            var attributes = TestHelper.Context.AttributeNames.ToDictionary(n => n.ShortName, n => n);
            foreach(var attribute in createParameters.Attributes)
            {
                if(!attributes.ContainsKey(attribute))
                {
                    attributes.Add(attribute, TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>(n => n.SetValues(attribute, attribute, true, true, true, true)));
                }
            }

            foreach(var createInventory in createParameters.Inventory)
            {
                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(
                    n => n.SetValidToPick(RinconFacility),
                    n => n.PackagingProduct.Weight = createInventory.PackagingWeight, n => n.Quantity = createInventory.Quantity);

                inventory.Lot.Attributes = createInventory.AttributeValues
                    .Where(v => v.Value.HasValue)
                    .Select(v => TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttribute>(
                    a => a.SetValues(inventory, attributes[v.Key], v.Value.Value))).ToList();

                if(CreatedInventory != null)
                {
                    CreatedInventory.Add(inventory);
                }
            }
        }

        public TService GetService<TService>()
            where TService : class
        {
            TestHelper.SaveChangesToContext();
            return Kernel.Get<TService>();
        }

        [StepArgumentTransformation]
        public List<Action<Inventory>[]> QualityStatusesTransform(Table table)
        {
            return table.Rows.Select(r =>
                {
                    var row = r.Values.ToArray();
                    return new List<Action<Inventory>>
                        {
                            i => i.Lot.QualityStatus = (LotQualityStatus) Enum.Parse(typeof(LotQualityStatus), Regex.Replace(row[0], @"\s+", "").ToUpper(), true),
                            i => i.Lot.Hold = row[1].ToUpper() == "NONE" ? (LotHoldType?) null : LotHoldType.HoldForAdditionalTesting,
                            i => i.Location.Locked = row[2].ToUpper() == "LOCKED"
                        }.ToArray();
                }).ToList();
        }

        [StepArgumentTransformation]
        public CreateParameters CreateParametersTransform(Table table)
        {
            var header = table.Header.ToArray();
            var attributes = new List<string>
                {
                    header[2],
                    header[3],
                    header[4]
                };

            var inventory = table.Rows.Select(r =>
            {
                var row = r.Values.ToArray();
                return new CreateInventoryItem
                {
                    Quantity = int.Parse(row[0]),
                    PackagingWeight = double.Parse(row[1]),
                    AttributeValues = new Dictionary<string, double?>
                                {
                                    { header[2], string.IsNullOrWhiteSpace(row[2]) ? (double?) null : double.Parse(row[2]) },
                                    { header[3], string.IsNullOrWhiteSpace(row[3]) ? (double?) null : double.Parse(row[3]) },
                                    { header[4], string.IsNullOrWhiteSpace(row[4]) ? (double?) null : double.Parse(row[4]) }
                                }
                };
            }).ToList();

            return new CreateParameters
            {
                Attributes = attributes,
                Inventory = inventory
            };
        }

        [StepArgumentTransformation]
        public Dictionary<string, double> CreateExpectedResults(Table table)
        {
            var header = table.Header.ToArray();
            var index = 0;
            return header.ToDictionary(h => h, h => double.Parse(table.Rows[0][index++]));
        }

        public class CreateParameters
        {
            public List<string> Attributes;
            public List<CreateInventoryItem> Inventory;
        }

        public class CreateInventoryItem
        {
            public int Quantity;
            public double PackagingWeight;
            public Dictionary<string, double?> AttributeValues;
        }

        public List<Inventory> CreatedInventory = new List<Inventory>();
        public Inventory CreateInventory(params Action<Inventory>[] inventoryInitializers)
        {
            var initializers = new List<Action<Inventory>> { i => i.SetValidToPick(RinconFacility) };
            if(inventoryInitializers != null)
            {
                initializers.AddRange(inventoryInitializers);
            }

            var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase(initializers.ToArray());
            if(CreatedInventory != null)
            {
                CreatedInventory.Add(inventory);
            }

            return inventory;
        }
    }
}
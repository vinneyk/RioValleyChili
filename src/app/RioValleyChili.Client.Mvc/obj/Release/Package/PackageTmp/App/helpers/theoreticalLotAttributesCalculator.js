define(['ko'], function(ko) {
    return {
        calculateTheoreticalLotAttributes: calculateTheoreticalLotAttributes
    };

    function calculateTheoreticalLotAttributes(inputs) {
        var working = { };

        sumAttributesForAllInputs();
        return computeWeightedAverages();
        
        function sumAttributesForAllInputs() {
            ko.utils.arrayMap(inputs, function (i) {
                ko.utils.arrayForEach(i.attributes, attributeAggregateor(i));
            });

            function attributeAggregateor(item) {
                return function (attr) {
                    addToWorkingTotal(attr.key, attr.value, item.weightInPounds);
                }
            }
            function addToWorkingTotal(key, value, weight) {
                //if (!(value > 0 && weight > 0)) return;

                var workingHold = working[key];
                if (!workingHold) workingHold = working[key] = { sumWeightedValue: 0, sumWeight: 0 };
                if (!value) return;

                weight = weight || 0;
                workingHold.sumWeightedValue += value * weight;
                workingHold.sumWeight += weight;
            }
        }
        function computeWeightedAverages() {
            var result = [];
            for (var p in working) {
                result.push(getResultForAttribute(p));
            }
            return result;

            function getResultForAttribute(attrKey) {
                var current = working[attrKey];
                return {
                    key: p,
                    value: current.sumWeight == 0 ? 0 : current.sumWeightedValue / current.sumWeight,
                };
            }
        }
    }
});
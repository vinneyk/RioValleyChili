define(['ko', 'services/qualityControl/labResultsService', 'rvc'], function (ko, labResultsService, rvc) {

    function LabResultInfo(values) {
        if (!(this instanceof LabResultInfo)) return new LabResultInfo(values);

        var model = {
            Description: values.qualityStatus === rvc.enums.LotQualityStatus.RequiresAttention
                ? 'Quality Hold' : 'Pending Lab Results',
            Count: values.count,
            NotificationLevel: values.qualityStatus === rvc.enums.LotQualityStatus.RequiresAttention
                ? rvc.enums.NotificationLevel.Warning
                : rvc.enums.NotificationLevel.Information,
            NotificationLevels: rvc.enums.NotificationLevel,
        }
        
        return model;
    }

    var metrics = ko.observableArray();

    var vm = {
        activate: activate,
        metrics: metrics,
    }

    return vm;

    function activate() {
        return labResultsService.getCountOfLotsByQualityStatus()
            .done(function (data) {
                metrics([]);
                for (var p in data) {
                    var qualityStatus = rvc.enums.LotQualityStatus[p];
                    switch(qualityStatus) {
                        case rvc.enums.LotQualityStatus.RequiresAttention:
                        case rvc.enums.LotQualityStatus.Undetermined:
                            metrics.push(new LabResultInfo({
                                qualityStatus: qualityStatus,
                                count: data[p]
                            }));
                        default: continue;
                    }
                }
            })
            .fail(function() {
                alert('Lab Results Service failed');
            });
    }
});
define(function () {
    function BatchingViewModel() {
        if (!(this instanceof BatchingViewModel)) return new BatchingViewModel();

        return {
            packSchedules: []
        }
    }

    return BatchingViewModel;
})
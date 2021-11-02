define(['jquery'], function($) {
    return {
        getCountOfLotsByQualityStatus: getCountOfLotsByQualityStatus
    }

    function getCountOfLotsByQualityStatus() {
        return $.ajax('api/lots/countByQualityStatus');
    }
})
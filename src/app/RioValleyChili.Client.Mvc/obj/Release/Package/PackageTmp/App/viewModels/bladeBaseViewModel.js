define(['ko'], function (ko) {
    return {
        init: init
    }
    
    function init(props) {
        var vm = {
            children: ko.observableArray([]),
            //bladeSize: ko.observable(),
        };

        return vm;
    }
})
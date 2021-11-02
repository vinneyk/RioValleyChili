require('App/ko.bindingHandlers.sortableTable');

define(['App/ko.bindingHandlers.clickableTableHeaders', 'helpers/propertyGetter', 'ko'], function (tableHeaderClickHelper, propertyGetter, ko) {
    var sortOption = {};

    ko.bindingHandlers.sortableTable = {
        init: function (element, valueAccessor, allBindingsAccessor) {
            var allBindings = allBindingsAccessor();
            allBindings.enableClick = canBeSorted;

            ko.bindingHandlers.clickableTableHeaders.init(
                element,
                ko.utils.wrapAccessor(sort),
                ko.utils.wrapAccessor(allBindings));

            function sort(th) {
                $(element).find('thead .' + ko.bindingHandlers.sortableTable.options.sortedCssClass)
                    .removeClass(ko.bindingHandlers.sortableTable.options.sortedCssClass + ' ' +
                        ko.bindingHandlers.sortableTable.options.sortedAscCssClass + ' ' +
                        ko.bindingHandlers.sortableTable.options.sortedDescCssClass);

                sortData(th, valueAccessor());
            }
        }
    }

    ko.bindingHandlers.sortableTable.options = {
        sortedCssClass: 'sorted',
        sortedAscCssClass: 'asc',
        sortedDescCssClass: 'desc'
    }

    function sortData(thElement, data) {
        var $th = $(thElement);
        var sort = $th.attr('data-sort');
        if (!sort) return;

        var previousSort = sortOption;
        var sortDirection = previousSort && previousSort.propertyName === sort
            ? previousSort.direction * -1
            : 1;

        sortOption = {
            propertyName: sort,
            direction: sortDirection
        };

        // todo: get context for table's tbody in order to prevent duplicate declaration source property as valueAccessor for the body and clickable header

        var sortFn = dynamicSortFn(sort, sortDirection);
        if (ko.isObservable(data)) {
          var dataCache = data();

            data(dataCache.sort(sortFn));
        }

        $th.addClass(ko.bindingHandlers.sortableTable.options.sortedCssClass + ' ' +
            (sortDirection > 0
                ? ko.bindingHandlers.sortableTable.options.sortedAscCssClass
                : ko.bindingHandlers.sortableTable.options.sortedDescCssClass));
    };

    function dynamicSortFn( sort, direction ) {
        if ( !sort ) { return null; }

        // 1 = Ascending, -1 = Descending
        if (direction != -1) { direction = 1; }

        var lt = -1 * direction,
            gt = 1 * direction;

        function sortFn( a, b ) {
            var _a = ko.utils.unwrapObservable( propertyGetter.getValue( a, sort ) );
            var _b = ko.utils.unwrapObservable( propertyGetter.getValue( b, sort ) );

            if ( _a < _b || _a == null ) {
              return lt;
            } else if ( _a > _b ) {
              return gt;
            }

            return 0;
        }

        return sortFn;
    }

    function canBeSorted(element) {
        var $th = $(element);
        var sort = $th.attr('data-sort');
        return sort && true;
    }
})

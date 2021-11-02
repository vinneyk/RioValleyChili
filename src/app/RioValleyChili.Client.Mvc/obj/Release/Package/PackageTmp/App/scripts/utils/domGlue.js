/*! Copyright (c) 2014 by Solutionhead Technologies, LLC
MIT license info: ... */

(function () {
    //if (require)
        define(['jquery'], factory);
    //else factory($);

    var wrapperClassName = 'domGlueWrapper',
        boundsClassName = 'domGlueBounds';

    var defaultOptions = {
        fixedOffset: 0,
        offsetTop: 0,
        offsetBottom: 0,
        parent: window,
        floatingElementId: "domGlueFloatingElement",
        position: {},
        zIndex: 25,
        initiallyHidden: false,
    };

    function factory($) {
        return {
            init: function(el, options) {
                var self = {};

                var $el = $(el);
                
                options = options || {};
                self.options = $.extend({}, defaultOptions, options);

                self.floatingElementId = options.floatingElementId;
                self.$parent = $(options.parent); //warning: options.parent is a DOM reference
                self.getParentElement = function () { return $(options.parent); } //warning: options.parent is a DOM reference

                self.handlers = [];

                self.cleanupClone = function () {
                    $('.' + options.floatingElementId, $el).remove();
                }
                self.cleanup = function () {
                    $el = null;
                    self.cleanupClone();
                    destroy.call(self);
                    self = null;
                }
                self.updateBoundaryPosition = function () {
                    var boundary = self.getBoundaryElement();
                    boundary.updateBoundaryPosition && boundary.updateBoundaryPosition();
                }

                init.call(self, $el, options);

                return self;
            },
            defaultOptions: defaultOptions,
        }


        function init($el, options) {
            var self = this;
            var $parent = self.getParentElement();
            var $window = $(window);

            if (!$el.parent("." + wrapperClassName).length) {
                $el.wrap('<div class="' + wrapperClassName + '"></div>');
            }

            self.getTargetElement = function() {
                return $(options.target, $el);
            };
            self.getCloneElement = function() {
                return $('.' + options.floatingElementId, $el);
            };
            self.getBoundaryElement = function() {
                return self.getTargetElement().siblings('.' + boundsClassName);
            };


            self.rebuildClone = function () {
                self.cleanupClone();
                $clone = cloneTarget();
                $clone.appendTo(self.getBoundaryElement());
            }

            self.cleanup = function () {
                $el = null;
                $clone = null;
                $parent = null; //remove resize handler, remove parents().scroll handlers
                target = null;
                boundary = null;
                $window.off('resize', updateBoundariesFn);
                $window = null; //remove resize handler
            }
            
            var target = self.getTargetElement(); 
            target.addClass('domGlueFloatingElementOriginal');
            var boundary = initBoundaryElement();

            var $clone = cloneTarget(); 
            $clone.appendTo(boundary);

            target.after(boundary);

            if (typeof options.onParentScroll === "function") {
                var parentScrolledEventFromOptions = function (event) {
                    options.onParentScroll($parent, target, $clone);
                    event.stopPropagation();
                }
                self.handlers.push({ obj: $parent, event: 'scroll', handler: parentScrolledEventFromOptions });
                $parent.scroll(parentScrolledEventFromOptions);
            }

            var updateBoundariesFn = currySetBoundaryPositioningEventHandler(boundary);

            self.handlers.push({ obj: $parent.parents(), event: 'scroll', handler: updateBoundariesFn });
            $parent.parents().scroll(updateBoundariesFn);

            self.handlers.push({ obj: $parent, event: 'resize', handler: updateBoundariesFn });
            $parent.resize(updateBoundariesFn);

            if ($parent[0] !== $window[0]) {
                self.handlers.push({ obj: $window, event: 'resize', handler: updateBoundariesFn });
                $window.resize(updateBoundariesFn);
            }

            setTimeout(function () {
                setBoundaryPositioning();
            }, 0); // ensure that the content has been drawn.
            
            function currySetBoundaryPositioningEventHandler(boundaryElement) {
                return function (event) {
                    setBoundaryPositioning(boundaryElement);
                    event && event.stopPropagation && event.stopPropagation();
                }
            }

            function initBoundaryElement() {
                var boundaryElement;
                boundaryElement = self.getBoundaryElement();
                if (boundaryElement.length) {
                    return boundaryElement[0];
                }

                boundaryElement = $("<div></div>");
                boundaryElement.addClass(boundsClassName);
                boundaryElement.addClass('pass-through');
                self.updateBoundaryPosition = function() {
                    setBoundaryPositioning(boundaryElement);
                }
                setBoundaryPositioning(boundaryElement);
                return boundaryElement;
            }
            function setBoundaryPositioning() {
                var parentPosition = $parent.offset() || { top: 0, left: 0 };

                self.getBoundaryElement().css({
                    'position': 'fixed',
                    'z-index': options.zIndex,
                    'top': parentPosition.top - $(window).scrollTop(),
                    'left': parentPosition.left,
                    'display': options.initiallyHidden ? 'none' : 'block',
                    'height': getHeight(),
                    'width': getWidth(),
                    overflow: 'hidden',
                });

                function getHeight() {
                    return $parent[0].clientHeight || $parent.height();
                }

                function getWidth() {
                    return $parent[0].clientWidth || $parent.width();
                }
            }
            function cloneTarget() {
                var originalEl = self.getTargetElement();
                var cloneEl = originalEl.clone();
                cloneEl.addClass(options.floatingElementId);

                cloneEl.css({
                    'position': 'absolute',
                    'top': options.position.top,
                    'left': options.position.left,
                    'bottom': options.position.bottom,
                    'right': options.position.right,
                });

                options.formatClone && options.formatClone(originalEl, cloneEl);

                return cloneEl;
            }
        }
        
        function destroy() {
            var self = this;

            self.$parent = null;

            for (var i = 0; i < self.handlers.length; i++) {
                var handle = self.handlers[i];
                handle.obj.off(handle.event, handle.handler);
            }

            self.handlers = [];
            self.cleanup();
        }
    }   
}());
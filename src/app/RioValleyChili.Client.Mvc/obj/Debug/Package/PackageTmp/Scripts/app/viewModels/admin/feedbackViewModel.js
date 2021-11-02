
// Admin model for viewing feedback notes.

// Singleton
var FeedbackViewModel = (function () {
    var self = {};
    var key = '17530101-0';

    self.notebook = new NotebookViewModel();
    self.filter = {
        showNeg: ko.observable(true),
        showPos: ko.observable(true),
        dateMin: ko.observable().extend({isoDate: 'm/d/yyyy HH:MM'}),
        dateMax: ko.observable().extend({isoDate: 'm/d/yyyy HH:MM'}),
        href: ko.observable(),
        user: ko.observable(),
        text: ko.observable()
    };

    self.remove = function (note, ev) {
        ev.stopPropagation();
        var n = self.notebook.getNote(note.NoteKey);
        self.notebook.delete(note);
    }
    self.notes = ko.computed(function () {
        return ko.utils.arrayMap(self.notebook.notes(), function (n) {
            n = dummify(n); // TODO: remove dummy data
            n.removable = ko.observable(false);

            var feedbackInfo = ko.utils.parseJson(ko.unwrap(n.Text));
            n.Text = feedbackInfo.text;
            n.Type = feedbackInfo.type;
            n.Href = feedbackInfo.href;
            return n;
        });
    });
    self.filteredNotes = ko.computed(function () {
        return ko.utils.arrayFilter(self.notes(), function (n) {
            var validType = validDate = validHref = validUser = validText = true;
            var date = new Date(n.NoteDate);

            if(self.filter.showNeg() || self.filter.showPos()){
                validType = (n.Type === 'pos' && self.filter.showPos()) || (n.Type === 'neg' && self.filter.showNeg());
            }
            if(self.filter.dateMin()){
                validDate = date >= new Date(self.filter.dateMin.formattedDate());
            }
            if(self.filter.dateMax()){
                validDate = validDate && (date <= new Date(self.filter.dateMax.formattedDate()));
            }
            if(self.filter.href()){
                validHref = n.Href.toLowerCase().indexOf(self.filter.href().toLowerCase()) !== -1;
            }
            if(self.filter.user()){
                validUser = n.CreatedByUser.toLowerCase().indexOf(self.filter.user().toLowerCase()) !== -1;
            }
            if(self.filter.text()){
                validText = n.Text.toLowerCase().indexOf(self.filter.text().toLowerCase()) !== -1;
            }
            return validType && validDate && validHref && validUser && validText;
        });
    });

    self.init = function () {
        self.notebook.show(key);
        return self;
    }

    return self;

    // TODO: remove
    function dummify(n) {
        var txt = ko.unwrap(n.Text);
        n.Text = ko.observable(JSON.stringify({
            text: txt,
            type: Math.random().toString(2)[2] % 2 == 0 ? 'neg' : 'pos',
            href: '/' + Math.random().toString(36).substring(7)
        }));
        return n;
    }
    // TODO end
})();


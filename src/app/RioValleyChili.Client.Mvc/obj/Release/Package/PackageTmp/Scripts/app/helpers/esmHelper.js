// Attaches esm properties the the supplied objectToTrack
var EsmHelper = function (objectToTrack, options) {
    if (!objectToTrack) throw new Error("Must provide an objectToTrack.");
    return setup(options);

    function setup() {
        var esm = ko.EditStateManager(objectToTrack, options);
        var propertiesToCopy = ['toggleEditingCommand', 'beginEditingCommand', 'endEditingCommand', 'revertEditsCommand', 'cancelEditsCommand', 'saveEditsCommand', 'isEditing', 'hasChanges'];
        for (var prop in propertiesToCopy) {
            var propName = propertiesToCopy[prop];
            objectToTrack[propName] = esm[propName];
        }
        return esm;
    }
}
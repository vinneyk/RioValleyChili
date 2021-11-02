define(['services/serviceCore', 'ko'], function (core, ko) {
    return {
        getNotebookByKey: getNotebookByKey,
        insertNote: insertNote,
        updateNote: putNote,
        updateNoteText: updateNoteText,
        deleteNote: deleteNote,

        // phase these methods out
        putNote: putNote,
        postNote: postNote,
    };

    //#region exports
    function getNotebookByKey(key, options) {
        return core.ajax("/api/notebooks/" + key + "/notes", options);
    }

    function putNote(note, options) {
        options.data = ko.toJSON(note.toDto());
        return core.ajaxPut("/api/notebooks/" + note.NotebookKey + "/notes/" + note.NoteKey, options);
    }

    function updateNoteText(note) {
        var NoteKey = note.NoteKey;
        var NotebookKey = note.NotebookKey;
        var NoteText = note.toDto();

        return core.ajaxPut("/api/notebooks/" + NotebookKey + "/notes/" + NoteKey, NoteText);
    }

    function insertNote(notebookKey, values) {
        return core.ajaxPost("/api/notebooks/" + notebookKey + "/notes", values);
    }
    function postNote(note, options) {
        console.warn("postNote is obsolete. Use insertNote instead.");
        options.data = ko.toJSON(note.toDto ? note.toDto() : note);
        return core.ajaxPost("/api/notebooks/" + note.NotebookKey + "/notes", options);
    }

    function deleteNote(note, options) {
        return core.ajaxDelete("/api/notebooks/" + note.NotebookKey + "/notes/" + note.NoteKey, options);        
    }
    //#endregion
});

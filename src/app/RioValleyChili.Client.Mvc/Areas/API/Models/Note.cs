using System;
using System.ComponentModel.DataAnnotations;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.NotebookService;
using RioValleyChili.Services.Interfaces.Returns.NotebookService;

namespace RioValleyChili.Client.Mvc.Areas.API.Models
{
    public class Note : INoteReturn
    {
        public string NoteKey { get; set; }
        public int Sequence { get; set; }
        public string Text { get; set; }
        public DateTime NoteDate { get; set; }
        public string CreatedByUser { get; set; }
    }

    public class NoteDto : ICreateNoteParameters, IUpdateNoteParameters
    {
        [Required]
        public string Text { get; set; }

        string IUserIdentifiable.UserToken { get; set; }
    }
}
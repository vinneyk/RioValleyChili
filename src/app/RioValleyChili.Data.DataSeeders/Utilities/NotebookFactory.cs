using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Utilities
{
    public class NotebookFactory
    {
        public static NotebookFactory Create(RioValleyChiliDataContext context)
        {
            return _instance ?? (_instance = NewInstance(context));
        }

        private static NotebookFactory _instance;

        private readonly RioValleyChiliDataContext _context;
        private readonly Dictionary<DateTime, int> _dateSequences = new Dictionary<DateTime, int>();

        private NotebookFactory(RioValleyChiliDataContext context)
        {
            if (context == null) { throw new ArgumentNullException("context"); }
            _context = context;
        }

        public Notebook BirthNext(DateTime timestamp)
        {
            int sequence;
            var date = timestamp.Date;
            if(!_dateSequences.TryGetValue(date, out sequence))
            {
                var existingNotebooks = _context.Set<Notebook>().Where(n => n.Date == date).ToList();
                sequence = existingNotebooks.Any() ? existingNotebooks.Max(n => n.Sequence) : 0;
                _dateSequences.Add(date, ++sequence);
            }
            else
            {
                _dateSequences[date] = ++sequence;
            }

            return new Notebook
                {
                    Date = date,
                    Sequence = sequence,
                    Notes = new List<Note>()
                };
        }

        /// <summary>
        /// Creates a new Notebook with Notes initialized with same TimeStamp and EmployeeId.
        /// </summary>
        public Notebook BirthNext(DateTime timestamp, int employeeId, params string[] notes)
        {
            var notebook = BirthNext(timestamp);

            var noteSequence = 1;
            notebook.Notes = notes == null ? new List<Note>() : notes.Where(n => !string.IsNullOrWhiteSpace(n)).Select(n => new Note
                {
                    NotebookDate = notebook.Date,
                    NotebookSequence = notebook.Sequence,
                    Sequence = noteSequence++,
                    TimeStamp = timestamp,
                    EmployeeId = employeeId,
                    Text = n
                }).ToList();

            return notebook;
        }

        /// <summary>
        /// Creates a new Notebook with Notes with unique TimeStamp and EmployeeId.
        /// </summary>
        public Notebook BirthNext(DateTime timestamp, int employeeId, IEnumerable<CreateNoteParameters> notes)
        {
            var notebook = BirthNext(timestamp);

            var noteSequence = 1;
            notebook.Notes = notes == null ? new List<Note>() : notes.Where(n => !string.IsNullOrWhiteSpace(n.Note)).Select(n => new Note
                {
                    NotebookDate = notebook.Date,
                    NotebookSequence = notebook.Sequence,
                    Sequence = noteSequence++,
                    TimeStamp = n.TimeStamp,
                    EmployeeId = n.EmployeeId,
                    Text = n.Note
                }).ToList();

            return notebook;
        }

        public class CreateNoteParameters
        {
            public string Note;
            public DateTime TimeStamp;
            public int EmployeeId;
        }

        private static NotebookFactory NewInstance(RioValleyChiliDataContext context)
        {
            return new NotebookFactory(context);
        }
    }
}
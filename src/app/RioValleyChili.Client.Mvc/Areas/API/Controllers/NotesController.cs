using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Mvc;
using RioValleyChili.Client.Mvc.Areas.API.Models;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Client.Mvc.Extensions;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Returns.NotebookService;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    public class NotesController : ApiController
    {
        #region fields and constructors

        private readonly INotebookService _notebookService;
        private readonly IUserIdentityProvider _identityProvider;

        public NotesController(INotebookService notebookService, IUserIdentityProvider identityProvider)
        {
            if (notebookService == null) { throw new ArgumentNullException("notebookService");}
            _notebookService = notebookService;

            if (identityProvider == null) { throw new ArgumentNullException("identityProvider"); }
            _identityProvider = identityProvider;
        }

        #endregion

        #region API methods
        
        // GET api/notebooks/5/notes
        public IEnumerable<INoteReturn> Get(string notebookKey)
        {
            var results = _notebookService.GetNotebook(notebookKey);
            results.EnsureSuccessWithHttpResponseException();
            return results.ResultingObject.Notes;
        }

        // GET api/notebooks/5/notes/1
        public INoteReturn Get(string notebookKey, string id)
        {
            var note = Get(notebookKey).FirstOrDefault(n => n.NoteKey == id);
            if (note == null) { throw new HttpResponseException(HttpStatusCode.NotFound); }
            return note;
        }

        // POST api/notebooks/5/notes
        public HttpResponseMessage Post(string notebookKey, [FromBody] NoteDto value)
        {
            if (!ModelState.IsValid) { return new HttpResponseMessage(HttpStatusCode.BadRequest); }
            _identityProvider.SetUserIdentity(value);

            var result = _notebookService.AddNote(notebookKey, value);
            var message = result.ToHttpResponseMessage(HttpVerbs.Post);

            message.Content = new ObjectContent<INoteReturn>(result.ResultingObject, new JsonMediaTypeFormatter());
            return message;
        }

        // PUT api/notebooks/5/notes/1
        public HttpResponseMessage Put(string id, [FromBody] NoteDto value)
        {
            if(!ModelState.IsValid) { throw new HttpResponseException(HttpStatusCode.BadRequest);}
            _identityProvider.SetUserIdentity(value);
            var result = _notebookService.UpdateNote(id, value);
            return result.ToHttpResponseMessage(HttpVerbs.Put);
        }

        // DELETE api/notebooks/5/notes/1
        public HttpResponseMessage Delete(string id)
        {
            return _notebookService.DeleteNote(id).ToHttpResponseMessage(HttpVerbs.Delete);
        }

        #endregion
    }
}

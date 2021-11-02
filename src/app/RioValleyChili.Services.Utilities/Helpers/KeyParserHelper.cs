using Ninject;
using RioValleyChili.Services.Utilities.Configuration.NinjectModules;
using Solutionhead.EntityKey;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Helpers
{
    internal class KeyParserHelper
    {
        private static KeyParserHelper Instance
        {
            get { return _instance ?? (_instance = new KeyParserHelper()); }
        }
        private static KeyParserHelper _instance;

        private readonly IKernel _kernel;

        private KeyParserHelper()
        {
            _kernel = new StandardKernel(new KeysModule());
        }

        internal static IResult<TKeyInterface> ParseResult<TKeyInterface>(string key) where TKeyInterface : class
        {
            var parser = Instance._kernel.Get<IKeyParser<TKeyInterface>>();
            if(parser == null)
            {
                return new FailureResult<TKeyInterface>(null, string.Format("Could not get key parser of '{0}'.", typeof(TKeyInterface).Name));
            }
            TKeyInterface parsedKey;
            if (!parser.TryParse(key, out parsedKey))
            {
                return new InvalidResult<TKeyInterface>(null, parser.GetParseFailMessage(key));
            }

            return new SuccessResult<TKeyInterface>(parsedKey);
        }
        
    }
}
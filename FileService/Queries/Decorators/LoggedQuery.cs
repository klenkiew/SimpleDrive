using Microsoft.Extensions.Logging;
using Serialization;

namespace FileService.Queries.Decorators
{
    public class LoggedQuery <TQuery, TResult>  : IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        private readonly IQueryHandler<TQuery, TResult> decorated;
        private readonly ILogger<IQueryHandler<TQuery, TResult> > logger;
        private readonly IObjectConverter converter;
        
        public LoggedQuery(
            IQueryHandler<TQuery, TResult> decorated, 
            ILogger<IQueryHandler<TQuery, TResult>> logger, 
            IObjectConverter converter)
        {
            this.decorated = decorated;
            this.logger = logger;
            this.converter = converter;
        }

        public TResult Handle(TQuery query)
        {
            logger.LogDebug("Executing query: " + converter.ToString(query));
            return decorated.Handle(query);
        }
    }
}
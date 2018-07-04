using System;
using FileService.Serialization;
using Microsoft.Extensions.Logging;

namespace FileService.Queries
{
    public class LoggedQuery <TQuery, TResult>  : IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        private readonly IQueryHandler<TQuery, TResult> decorated;
        private readonly ILogger<IQueryHandler<TQuery, TResult> > logger;
        private readonly ISerializer serializer;

        public LoggedQuery(
            IQueryHandler<TQuery, TResult> decorated, 
            ILogger<IQueryHandler<TQuery, TResult>> logger, 
            ISerializer serializer)
        {
            this.decorated = decorated;
            this.logger = logger;
            this.serializer = serializer;
        }

        public TResult Handle(TQuery query)
        {
            logger.LogDebug("Executing query: " + serializer.Serialize(query));
            return decorated.Handle(query);
        }
    }
}
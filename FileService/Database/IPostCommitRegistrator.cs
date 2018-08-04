using System;

namespace FileService.Database
{
    public interface IPostCommitRegistrator
    {
        event Action Committed;
    }

    public sealed class PostCommitRegistratorImpl : IPostCommitRegistrator
    {
        public event Action Committed = () => { };
 
        public void ExecuteActions()
        {
            Committed(); 
        }

        public void Reset()
        {
            // Clears the list of actions.
            Committed = () => { };    
        }
    }
}
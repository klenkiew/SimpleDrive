using System;
using FileService.Database;

namespace FileService.Tests.Fakes
{
    public class FakePostCommitRegistrar : IPostCommitRegistrator
    {
        private readonly PostCommitRegistratorImpl decorated = new PostCommitRegistratorImpl();
        
        public event Action Committed
        {
            add => decorated.Committed += value;
            remove => decorated.Committed -= value;
        }
        
        public void ExecuteActions()
        {
            decorated.ExecuteActions(); 
        }

        public void Reset()
        {
            decorated.ExecuteActions();
        }
    }
}
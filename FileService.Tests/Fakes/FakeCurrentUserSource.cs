using System;
using FileService.Infrastructure;
using FileService.Model;

namespace FileService.Tests.Fakes
{
    public class FakeCurrentUserSource : ICurrentUserSource
    {
        public User currentUser { get; set; }
        
        public User GetCurrentUser()
        {
            return currentUser ?? throw new InvalidOperationException("Current user not set");
        }
    }
}
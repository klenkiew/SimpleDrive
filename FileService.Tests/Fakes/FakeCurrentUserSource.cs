using System;
using FileService.Infrastructure;
using FileService.Model;

namespace FileService.Tests.Fakes
{
    public class FakeCurrentUserSource : ICurrentUserSource
    {
        // to prevent bugs when the test author forget to set the current user
        // - make this mistake clear by throwing an exception
        private bool userSet = false;

        private User currentUser;

        public User CurrentUser
        {
            get => currentUser;
            set
            {
                userSet = true;
                currentUser = value;
            }
        }

        public User GetCurrentUser()
        {
            if (!userSet)
                throw new InvalidOperationException("Current user not set");

            return CurrentUser;

        }
    }
}
﻿namespace AuthenticationService.Dto
{
    public class UserDto
    {
        public string Id { get; }
        public string Username { get; }
        public string Email { get; }

        public UserDto(string id, string username, string email)
        {
            Id = id;
            Username = username;
            Email = email;
        }
    }
}
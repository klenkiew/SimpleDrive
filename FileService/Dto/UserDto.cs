using System.Collections.Generic;

namespace FileService.Dto
{
    public class UserDto
    {
        public string Id { get; }
        public string Username { get; }
        
        public UserDto(string id, string username)
        {
            Id = id;
            Username = username;
        }
    }

    public class UserDtoEqualityComparer : IEqualityComparer<UserDto>
    {
        public bool Equals(UserDto x, UserDto y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return string.Equals(x.Id, y.Id);
        }

        public int GetHashCode(UserDto obj)
        {
            return (obj.Id != null ? obj.Id.GetHashCode() : 0);
        }
    }
}
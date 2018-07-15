namespace FileService.Dto
{
    public class FileLockDto
    {
        public bool IsLockPresent { get; }
        public UserDto LockOwner { get; }

        public FileLockDto(bool isLockPresent, UserDto lockOwner)
        {
            IsLockPresent = isLockPresent;
            LockOwner = lockOwner;
        }

        public static FileLockDto NoLock()
        {
            return new FileLockDto(false, null);
        }

        public static FileLockDto ForUser(UserDto user)
        {
            return new FileLockDto(user != null, user);
        }
    }
}
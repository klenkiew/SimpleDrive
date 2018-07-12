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
    }
}
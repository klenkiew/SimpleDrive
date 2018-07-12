using System;

namespace FileService.Configuration
{
    public class StorageConfiguration
    {
        public string Path { get; set; }
        public TimeSpan FileLockDuration { get; set; }
    }
}
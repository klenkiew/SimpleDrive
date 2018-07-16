using System;
using System.Linq;
using System.Timers;
using EventBus;
using FileService.Dto;
using FileService.Events;
using Microsoft.Extensions.Logging;
using Priority_Queue;

namespace FileService.Services
{
    internal class FileLockExpiryNotificator : IDisposable, IFileLockExpiryNotificator
    {
        private readonly IEventBusWrapper eventBus;
        private readonly ILogger<FileLockExpiryNotificator> logger;

        private readonly IPriorityQueue<FileLockExpiryInfo, DateTime> expiryDatesByFileId;
        private readonly Timer timer;

        private readonly object @lock = new object();

        public FileLockExpiryNotificator(IEventBusWrapper eventBus, ILoggerFactory loggerFactory)
        {
            this.eventBus = eventBus;
            this.logger = loggerFactory.CreateLogger<FileLockExpiryNotificator>();
            this.expiryDatesByFileId = new SimplePriorityQueue<FileLockExpiryInfo, DateTime>();
            this.timer = new Timer
            {
                AutoReset = true,
                Interval = int.MaxValue,
            };
            this.timer.Elapsed += ProcessLockExpiry;
            this.timer.Start();
        }

        private void ProcessLockExpiry(object sender, ElapsedEventArgs e)
        {
            logger.LogDebug($"ProcessLockExpiry");
            lock (@lock)
            {
                if (!expiryDatesByFileId.Any() || expiryDatesByFileId.First.ExpiryDate > DateTime.UtcNow)
                {
                    ResetInterval();
                    return;
                }

                var expiredLockInfo = expiryDatesByFileId.Dequeue();
                eventBus.Publish<IEvent<FileLockChangedMessage>, FileLockChangedMessage>(new FileLockChangedEvent(
                    new FileLockChangedMessage(expiredLockInfo.FileId, FileLockDto.NoLock())));
                ResetInterval();
            }
        }

        public void ResetFileLock(string fileId, TimeSpan fileLockDuration)
        {
            var expiryDate = DateTime.UtcNow + fileLockDuration;
            var lockexpiryInfo = new FileLockExpiryInfo(fileId, expiryDate);

            logger.LogDebug($"ResetFileLock: (fileId: {fileId}, fileLockDuration: {fileLockDuration})");
            lock (@lock)
            {
                if (expiryDatesByFileId.Any() && expiryDatesByFileId.Contains(lockexpiryInfo))
                    expiryDatesByFileId.Remove(lockexpiryInfo);
                expiryDatesByFileId.Enqueue(lockexpiryInfo, expiryDate);
                if (Equals(expiryDatesByFileId.First, lockexpiryInfo))
                    ResetInterval();
            }
        }

        public void RemoveFileLock(string fileId)
        {
            var expiryInfo = new FileLockExpiryInfo(fileId, DateTime.UtcNow);

            logger.LogDebug($"RemoveFileLock: (fileId: {fileId})");
            lock (@lock)
            {
                bool resetInterval = Equals(expiryDatesByFileId.First, expiryInfo);
                expiryDatesByFileId.Remove(expiryInfo);
                if (resetInterval)
                    ResetInterval();
            }
        }

        public void Dispose()
        {
            timer?.Dispose();
        }

        private void ResetInterval()
        {
            if (!expiryDatesByFileId.Any())
                timer.Interval = int.MaxValue;
            else
                timer.Interval = (expiryDatesByFileId.First.ExpiryDate - DateTime.UtcNow).TotalMilliseconds;
        }
    }
}
﻿namespace EventBus
{
    public interface IMessageHandler<T>
    {
        void Handle(T message);
    }
}
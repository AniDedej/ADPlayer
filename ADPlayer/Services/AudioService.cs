using System;
using ManagedBass;

namespace ADPlayer.Services
{
    public interface IAudioService : IDisposable
    {
        bool Initialize();
        void Load(string filePath);
        void Play();
        void Pause();
        void Stop();
        double GetPosition();
        double GetDuration();
        void Seek(double seconds);
        void SetVolume(float volume);
    }

    public class AudioService : IAudioService
    {
        private int _streamHandle;
        private float _volume = 1.0f;

        public bool Initialize()
        {
            return Bass.Init();
        }

        public void Load(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) return;

            if (_streamHandle != 0)
            {
                Bass.StreamFree(_streamHandle);
            }

            _streamHandle = Bass.CreateStream(filePath, 0, 0, BassFlags.Default);
            if (_streamHandle == 0)
            {
                throw new BassException(Bass.LastError);
            }

            Bass.ChannelSetAttribute(_streamHandle, ChannelAttribute.Volume, _volume);
        }

        public void Play() => Bass.ChannelPlay(_streamHandle, false);
        public void Pause() => Bass.ChannelPause(_streamHandle);
        public void Stop() => Bass.ChannelStop(_streamHandle);

        public double GetPosition()
        {
            var pos = Bass.ChannelGetPosition(_streamHandle);
            return Bass.ChannelBytes2Seconds(_streamHandle, pos);
        }

        public double GetDuration()
        {
            var len = Bass.ChannelGetLength(_streamHandle);
            return Bass.ChannelBytes2Seconds(_streamHandle, len);
        }

        public void Seek(double seconds)
        {
            var pos = Bass.ChannelSeconds2Bytes(_streamHandle, seconds);
            Bass.ChannelSetPosition(_streamHandle, pos);
        }

        public void SetVolume(float volume)
        {
            _volume = volume;
            if (_streamHandle != 0)
                Bass.ChannelSetAttribute(_streamHandle, ChannelAttribute.Volume, _volume);
        }

        public void Dispose()
        {
            Stop();
            Bass.StreamFree(_streamHandle);
            _streamHandle = 0;
            Bass.Free();
        }
    }
}

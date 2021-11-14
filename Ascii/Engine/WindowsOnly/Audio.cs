using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Ascii
{
   
    public class AudioPlaybackEngine : IDisposable
    {
        private readonly IWavePlayer outputDevice;
        private readonly MixingSampleProvider mixer;
        public readonly WaveFormat Format;

        public AudioPlaybackEngine(int sampleRate = 44100, int channelCount = 2)
        {
            WaveOutEvent waveOutEvent = new WaveOutEvent();
            outputDevice = waveOutEvent;
            Format = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channelCount);
            mixer = new MixingSampleProvider(Format);
            mixer.ReadFully = true;
            waveOutEvent.DeviceNumber = -1;
            outputDevice.Init(mixer);
            outputDevice.Play();
        }

        public float MasterVolume
        {
            get { return outputDevice.Volume; }
            set { outputDevice.Volume = value; }
        }

        /// <summary> Only use this if you want to play a sound a single time (not cached) </summary>
        public void PlaySound(string fileName)
        {
            var input = new AudioFileReader(fileName);
            AddMixerInput(new AutoDisposeFileReader(input));
        }

        private ISampleProvider ConvertToRightChannelCount(ISampleProvider input)
        {
            if (input.WaveFormat.Channels == mixer.WaveFormat.Channels)
            {
                return input;
            }
            if (input.WaveFormat.Channels == 1 && mixer.WaveFormat.Channels == 2)
            {
                return new MonoToStereoSampleProvider(input);
            }
            throw new NotImplementedException("Not yet implemented this channel count conversion");
        }

        /// <summary> Play new sounds here using new SoundInstance(cachedSound) </summary>
        public void PlaySoundInstance(ISampleProvider soundInstance)
        {
            if (soundInstance != null)
            {
                AddMixerInput(soundInstance);
            }
        }

        public void StopSoundInstance(ISampleProvider soundInstance)
        {
            mixer.RemoveMixerInput(soundInstance);
        }

        private void AddMixerInput(ISampleProvider input)
        {
            mixer.AddMixerInput(ConvertToRightChannelCount(input));
        }

        private void RemoveMixerInput(ISampleProvider input)
        {
            mixer.RemoveMixerInput(input);
        }

        public void Dispose()
        {
            outputDevice.Dispose();
        }

        public static readonly AudioPlaybackEngine Instance = new AudioPlaybackEngine(44100, 2);
    }

    public class CachedSound
    {
        public float[] AudioData { get; private set; }
        public WaveFormat WaveFormat { get; private set; }

        public CachedSound(string audioFileName)
        {
            AudioFileReader afr = new AudioFileReader(audioFileName);
            WaveToSampleProvider wts = new WaveToSampleProvider(new MediaFoundationResampler(new SampleToWaveProvider(afr), AudioPlaybackEngine.Instance.Format));
            int samplesRead;
            var wholeFile = new List<float>((int)(afr.Length / 4));
            var readBuffer = new float[wts.WaveFormat.SampleRate * wts.WaveFormat.Channels];
            while ((samplesRead = wts.Read(readBuffer, 0, readBuffer.Length)) > 0)
            {
                wholeFile.AddRange(readBuffer.Take(samplesRead));
            }
            WaveFormat = wts.WaveFormat;
            AudioData = wholeFile.ToArray();
        }
    }

    /// <summary> Class using a cached buffer as data and providing access to volume </summary>
    public class SoundInstance : ISampleProvider
    {
        public readonly CachedSound cachedSound;
        public long Position = 0;
        public float Volume = 1f;
        public bool LoopingEnabled = true;
        public bool StopASAP = false;

        public SoundInstance(CachedSound cachedSound)
        {
            this.cachedSound = cachedSound;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            int totalBytesRead = 0;

            while (totalBytesRead < count)
            {
                if (StopASAP)
                    return 0;
                // we have reached the end of the file
                if (cachedSound.AudioData.Length == Position)
                {
                    if (LoopingEnabled)
                        Position = 0; // reset position if looping enabled
                    else
                        return totalBytesRead; // otherwise, stop reading.
                }

                // copy data
                buffer[totalBytesRead + offset] = cachedSound.AudioData[Position] * Volume;

                ++Position;
                ++totalBytesRead;
            }
            return totalBytesRead;
        }

        public WaveFormat WaveFormat { get { return cachedSound.WaveFormat; } }
    }

    // This class automatically disposes the file reader that it contains.
    class AutoDisposeFileReader : ISampleProvider
    {
        private readonly AudioFileReader reader;
        private bool isDisposed;
        public AutoDisposeFileReader(AudioFileReader reader)
        {
            this.reader = reader;
            this.WaveFormat = reader.WaveFormat;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            if (isDisposed)
                return 0;
            int read = reader.Read(buffer, offset, count);
            if (read == 0)
            {
                reader.Dispose();
                isDisposed = true;
            }
            return read;
        }

        public WaveFormat WaveFormat { get; private set; }
    }

    public class SoundManager
    {
        private readonly Dictionary<string, CachedSound> _soundsInMemory = new();

        public SoundManager(string rootFolder)
        {
            LoadAndCacheAllGameSounds(rootFolder);
        }

        private void LoadAndCacheAllGameSounds(string rootFolder)
        {
            if (Directory.Exists($"{rootFolder}/wav"))
            {
                foreach (var globalSoundFile in Directory.GetFiles($"{rootFolder}/wav"))
                {
                    var filename = Path.GetFileName(globalSoundFile).Split('.')[0];
                    LoadSound(filename, globalSoundFile);
                }
            }

            if (Directory.Exists($"{rootFolder}/entities"))
            {
                foreach (var entityDir in Directory.GetDirectories($"{rootFolder}/entities"))
                {
                    string entityName = new DirectoryInfo(Path.GetFileName(entityDir)).Name;

                    if (Directory.Exists($"{entityDir}/wav"))
                    {
                        foreach (var soundFile in Directory.GetFiles($"{entityDir}/wav"))
                        {
                            var filename = $"{entityName}_{Path.GetFileName(soundFile).Split('.')[0]}";
                            LoadSound(filename, soundFile);
                        }
                    }
                }
            }
        }

        public void UnloadSound(string soundName)
        {
            if (_soundsInMemory.ContainsKey(soundName))
                _soundsInMemory.Remove(soundName);
        }

        public void LoadSound(string soundName, string fileName)
        {
            _soundsInMemory.Add(soundName, new CachedSound(fileName));
        }

        public CachedSound GetLoadedSound(string soundName)
        {
            if (_soundsInMemory.ContainsKey(soundName))
                return _soundsInMemory[soundName];
            return null;
        }

        public SoundInstance CreateSoundInstance(string soundName, float volume = 1f, bool enableLooping = false)
        {
            var cachedSound = GetLoadedSound(soundName);
            if (cachedSound != null)
            {
                var sound = new SoundInstance(cachedSound)
                {
                    Volume = volume,
                    LoopingEnabled = enableLooping
                };
                return sound;
            }
            return null;
        }
    }
}
using NAudio.Wave;

namespace Ascii
{
    public class Audio
    {
        public void PlayAudio()
        {
            using (var waveOut = new WaveOutEvent())
            using (var wavReader = new WaveFileReader(@".\wav\\wind.wav"))
            {
                waveOut.Init(wavReader);
                waveOut.Volume = 0.8f;
                waveOut.Play();
            }
        }
    }
}
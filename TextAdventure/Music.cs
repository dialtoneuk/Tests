using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;

namespace TextAdventure
{
    public class Music
    {

        public enum Tracks : int
        {

            TRACK_ACIDHILLS,
            TRACK_UNKNOWN,
            TRACK_HIGHWAYS,
            TRACK_1913,
            TRACK_1851,
            TRACK_1326,
            TRACK_PETSCOP,
            TRACK_MUGGY_SIGNALS,
            TRACK_SEEDED_NUMBERS,
            TRACK_2310
        }

        private Dictionary<Tracks, string> trackFilenames = new Dictionary<Tracks, string>
        {
            {Tracks.TRACK_UNKNOWN,"unknown.mp3" },
            {Tracks.TRACK_ACIDHILLS,"acidhills.mp3" },
            {Tracks.TRACK_HIGHWAYS,"highways.mp3" },
            {Tracks.TRACK_1913,"1913.mp3" },
            {Tracks.TRACK_1851,"1851_compressed.mp3" },
            {Tracks.TRACK_1326,"1326_compressed.mp3" },
            {Tracks.TRACK_PETSCOP,"petscop.mp3" },
            {Tracks.TRACK_MUGGY_SIGNALS,"muggy_signals.mp3" },
            {Tracks.TRACK_SEEDED_NUMBERS, "seeded_numbers.mp3" },
            {Tracks.TRACK_2310, "2310.mp3" },
        };

        public const int JUKEBOX_INTERVAL = 600;
        public const float DEFAULT_VOLUME = 0.5f;

        protected Tracks currentTrack;
        protected AudioFileReader stream;
        protected WaveOutEvent output;
        protected DateTimeOffset later;
        protected static FadeInOutSampleProvider fade;

        public Music(Tracks startingTrack = Tracks.TRACK_UNKNOWN)
        {
            this.currentTrack = startingTrack;
            stream = new AudioFileReader("tracks/" + trackFilenames[this.currentTrack]);
            stream.Volume = DEFAULT_VOLUME;
        }

        public void jukebox()
        {

            if (isPlaying())
                return;

            if (DateTime.UtcNow > later)
            {

                Random r = new Random((int)DateTime.UtcNow.ToBinary());

                if (r.Next(1, 100) > 75)
                {

                    var length = Enum.GetValues(typeof(Tracks)).Length;
                    Tracks track = (Tracks)Enum.GetValues(typeof(Tracks)).GetValue(r.Next(0, length - 1));
                    changeTrack(track);
                    playTrack();
                    later = DateTimeOffset.UtcNow.Add(TimeSpan.FromSeconds(JUKEBOX_INTERVAL));
                }
            }
        }

        public void playTrack()
        {

            if (output.PlaybackState != PlaybackState.Playing)
            {
                if (fade != null)
                {

                    output.Init(fade);
                    output.Play();
                }
            }
        }

        public void setOutputDevice()
        {

            if (output == null)
            {
                output = new WaveOutEvent();
                output.Volume = DEFAULT_VOLUME;
            }
        }

        public void stop()
        {

            output.Stop();
        }

        public void changeTrack(Tracks track)
        {


            if (isPlaying())
            {
                fadeOut();
                System.Threading.Thread.Sleep(1000);
                output.Stop();
            }

            if (stream != null)
                stream.Dispose();

            stream = new AudioFileReader("tracks/" + trackFilenames[track]);
            stream.Volume = DEFAULT_VOLUME;
            fade = new FadeInOutSampleProvider(stream, true);
            fade.BeginFadeIn(2000);
            this.currentTrack = track;
        }

        public void fadeOut()
        {

            fade.BeginFadeOut(1000);
        }

        public void dispose()
        {

            output.Dispose();
            fade = null;
        }

        public void setVolume(float volume)
        {

            if (output != null)
                this.output.Volume = volume;
        }

        public Tracks getCurrentTrack()
        {

            return this.currentTrack;
        }

        public bool isPlaying()
        {

            if (output == null)
                return false;

            return (output.PlaybackState == PlaybackState.Playing);
        }

        public float getVolume()
        {

            return this.output.Volume;
        }

        public bool hasOutputDevice()
        {

            return (output != null);
        }
    }
}

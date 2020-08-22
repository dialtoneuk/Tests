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

            ACIDHILLS,
            UNKNOWN,
            HIGHWAYS,
            _1913,
            _1851,
            _1326,
            PETSCOP,
            MUGGY_SIGNALS,
            SEEDED_NUMBERS,
            _2310,
            TANJENT,
            JELLICA,
            LIQUID_RHYTHM,
            BUXTON,
            BAD
        }

        private Dictionary<Tracks, string> trackFilenames = new Dictionary<Tracks, string>
        {
            {Tracks.UNKNOWN,"unknown.mp3" },
            {Tracks.ACIDHILLS,"acidhills.mp3" },
            {Tracks.HIGHWAYS,"highways.mp3" },
            {Tracks._1913,"1913.mp3" },
            {Tracks._1851,"1851_compressed.mp3" },
            {Tracks._1326,"1326_compressed.mp3" },
            {Tracks.PETSCOP,"petscop.mp3" },
            {Tracks.MUGGY_SIGNALS,"muggy_signals.mp3" },
            {Tracks.SEEDED_NUMBERS, "seeded_numbers.mp3" },
            {Tracks._2310, "2310.mp3" },
            {Tracks.TANJENT, "tanjent.mp3" },
            {Tracks.JELLICA, "jellica.mp3" },
            {Tracks.LIQUID_RHYTHM, "liquid_rhythm.mp3" },
            {Tracks.BUXTON, "buxton.mp3" },
            {Tracks.BAD, "bad.mp3" },
        };

        public const int JUKEBOX_INTERVAL = 1000;
        public const float DEFAULT_VOLUME = 0.25f;

        protected Tracks currentTrack;
        protected AudioFileReader stream;
        protected WaveOutEvent output;
        protected DateTimeOffset later;
        protected static FadeInOutSampleProvider fade;

        public Music(Tracks startingTrack = Tracks.UNKNOWN)
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
                r.Next();

                if (r.Next(1, 500) < 10)
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

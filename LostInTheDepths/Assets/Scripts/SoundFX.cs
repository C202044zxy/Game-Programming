using UnityEngine;

/// <summary>
/// Procedural sound: the audio counterpart to <see cref="RuntimeSprites"/>.
/// Every clip — the ambient drone, the pearl chime, the death sting, the portal
/// cue and the win jingle — is synthesised in C# at first use, so the game ships
/// no imported audio files. A single hidden host object, kept alive across scene
/// loads with <c>DontDestroyOnLoad</c>, owns one looping source for the ambience
/// and one source for one-shot cues. Persisting the host means a sting such as the
/// death sound can finish playing even while the level reloads underneath it, and
/// the ambient loop carries seamlessly across a respawn.
/// </summary>
public static class SoundFX
{
    const int SampleRate = 44100;

    static AudioSource ambienceSource;
    static AudioSource oneShotSource;

    static AudioClip chimeClip;
    static AudioClip portalReadyClip;
    static AudioClip deathClip;
    static AudioClip winClip;
    static AudioClip ambienceClip;

    /// <summary>Lazily creates the persistent host with its two audio sources.</summary>
    static void EnsureHost()
    {
        if (oneShotSource != null) return;

        var host = new GameObject("SoundFX");
        host.hideFlags = HideFlags.HideInHierarchy;
        Object.DontDestroyOnLoad(host);

        ambienceSource = host.AddComponent<AudioSource>();
        ambienceSource.loop = true;
        ambienceSource.playOnAwake = false;
        ambienceSource.volume = 0.35f;

        oneShotSource = host.AddComponent<AudioSource>();
        oneShotSource.playOnAwake = false;
        oneShotSource.volume = 0.7f;
    }

    // ---- one-shot cues -----------------------------------------------------

    /// <summary>Bright bell played when a pearl is collected.</summary>
    public static void PlayChime() => Play(ref chimeClip, BuildChime, 0.7f);

    /// <summary>Distinct rising bloom when the final pearl unlocks the portal.</summary>
    public static void PlayPortalReady() => Play(ref portalReadyClip, BuildPortalReady, 0.85f);

    /// <summary>Descending sting played on predator contact.</summary>
    public static void PlayDeath() => Play(ref deathClip, BuildDeath, 0.9f);

    /// <summary>Ascending arpeggio played on the win screen.</summary>
    public static void PlayWin() => Play(ref winClip, BuildWin, 0.85f);

    static void Play(ref AudioClip cache, System.Func<AudioClip> build, float volume)
    {
        EnsureHost();
        if (cache == null) cache = build();
        oneShotSource.PlayOneShot(cache, volume);
    }

    // ---- looping ambience --------------------------------------------------

    /// <summary>Start the looping ambient drone if it is not already playing.</summary>
    public static void StartAmbience()
    {
        EnsureHost();
        if (ambienceClip == null) ambienceClip = BuildAmbience();
        if (ambienceSource.isPlaying && ambienceSource.clip == ambienceClip) return;
        ambienceSource.clip = ambienceClip;
        ambienceSource.Play();
    }

    /// <summary>Stop the ambient drone (used when leaving for the win screen).</summary>
    public static void StopAmbience()
    {
        if (ambienceSource != null) ambienceSource.Stop();
    }

    // ---- synthesis ---------------------------------------------------------

    // A struck bell: a fundamental plus octave and twelfth partials under a sharp
    // exponential decay.
    static AudioClip BuildChime()
    {
        const float dur = 0.55f, f = 1318.5f; // E6
        var d = new float[(int)(SampleRate * dur)];
        for (int i = 0; i < d.Length; i++)
        {
            float t = (float)i / SampleRate;
            float env = Mathf.Exp(-t * 7f);
            d[i] = env * (Mathf.Sin(Tau * f * t)
                        + 0.5f * Mathf.Sin(Tau * f * 2f * t)
                        + 0.3f * Mathf.Sin(Tau * f * 3f * t));
        }
        return FromSamples("Chime", d);
    }

    // A rising open fifth that blooms — deliberately unlike the single-note chime
    // so "the exit is ready" reads as its own event.
    static AudioClip BuildPortalReady()
    {
        var d = new float[(int)(SampleRate * 1.0f)];
        AddNote(d, 0.00f, 0.70f, 659.25f, 0.7f); // E5
        AddNote(d, 0.15f, 0.80f, 987.77f, 0.7f); // B5
        return FromSamples("PortalReady", d);
    }

    // A short triumphant C-major arpeggio climbing to the octave.
    static AudioClip BuildWin()
    {
        var d = new float[(int)(SampleRate * 1.2f)];
        AddNote(d, 0.00f, 0.50f, 523.25f, 0.8f); // C5
        AddNote(d, 0.12f, 0.50f, 659.25f, 0.8f); // E5
        AddNote(d, 0.24f, 0.60f, 783.99f, 0.8f); // G5
        AddNote(d, 0.40f, 0.80f, 1046.50f, 0.9f); // C6
        return FromSamples("Win", d);
    }

    // A pitch glide from mid down to a low thud, with a quick noise transient.
    static AudioClip BuildDeath()
    {
        const float dur = 0.6f;
        var d = new float[(int)(SampleRate * dur)];
        var rng = new System.Random(7);
        for (int i = 0; i < d.Length; i++)
        {
            float t = (float)i / SampleRate;
            float freq = Mathf.Lerp(420f, 70f, t / dur);
            float env = Mathf.Exp(-t * 4f);
            float tone = Mathf.Sin(Tau * freq * t);
            float noise = (float)(rng.NextDouble() * 2.0 - 1.0) * 0.3f * Mathf.Exp(-t * 12f);
            d[i] = (tone * 0.8f + noise) * env;
        }
        return FromSamples("Death", d);
    }

    // A quiet seamless drone. Every partial and the slow swell complete a whole
    // number of cycles over the 4 s loop, so the join is click-free.
    static AudioClip BuildAmbience()
    {
        const float dur = 4f;
        var d = new float[(int)(SampleRate * dur)];
        for (int i = 0; i < d.Length; i++)
        {
            float t = (float)i / SampleRate;
            float swell = 0.5f + 0.5f * Mathf.Sin(Tau * 0.25f * t); // 1 cycle / 4 s
            d[i] = swell * (0.6f * Mathf.Sin(Tau * 55f * t)   // A1
                          + 0.4f * Mathf.Sin(Tau * 110f * t)  // A2
                          + 0.2f * Mathf.Sin(Tau * 165f * t)); // E3
        }
        return FromSamples("Ambience", d);
    }

    // ---- helpers -----------------------------------------------------------

    const float Tau = 2f * Mathf.PI;

    /// <summary>Adds one decaying, slightly bright note into the buffer at a time offset.</summary>
    static void AddNote(float[] d, float startSec, float durSec, float freq, float amp)
    {
        int start = (int)(startSec * SampleRate);
        int len = (int)(durSec * SampleRate);
        for (int i = 0; i < len && start + i < d.Length; i++)
        {
            float t = (float)i / SampleRate;
            float attack = Mathf.Clamp01(t / 0.01f);
            float env = attack * Mathf.Exp(-t * 5f);
            float s = Mathf.Sin(Tau * freq * t) + 0.4f * Mathf.Sin(Tau * freq * 2f * t);
            d[start + i] += s * env * amp;
        }
    }

    static AudioClip FromSamples(string name, float[] data)
    {
        Normalize(data, 0.9f);
        var clip = AudioClip.Create(name, data.Length, 1, SampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    /// <summary>Scales the buffer so its loudest sample sits at <paramref name="peak"/>.</summary>
    static void Normalize(float[] d, float peak)
    {
        float max = 0f;
        for (int i = 0; i < d.Length; i++) max = Mathf.Max(max, Mathf.Abs(d[i]));
        if (max < 1e-5f) return;
        float g = peak / max;
        for (int i = 0; i < d.Length; i++) d[i] *= g;
    }
}

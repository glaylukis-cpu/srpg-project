using System;
using UnityEngine;

#pragma warning disable 0649

namespace SRPG.Audio
{
    public enum BgmTrack
    {
        None,
        Title,
        StageSelect,
        Battle
    }

    public class AudioManager : MonoBehaviour
    {
        private const int SampleRate = 44100;
        private const float TwoPi = 6.28318530718f;

        [Header("Volume")]
        [SerializeField] private float masterVolume = 0.7f;
        [SerializeField] private float bgmVolume = 0.25f;
        [SerializeField] private float seVolume = 0.45f;
        [SerializeField] private bool muteAll;

        [Header("Optional BGM Clips")]
        [SerializeField] private AudioClip titleBgmClip;
        [SerializeField] private AudioClip stageSelectBgmClip;
        [SerializeField] private AudioClip battleBgmClip;

        [Header("Optional SE Clips")]
        [SerializeField] private AudioClip cursorSeClip;
        [SerializeField] private AudioClip confirmSeClip;
        [SerializeField] private AudioClip cancelSeClip;
        [SerializeField] private AudioClip attackSeClip;
        [SerializeField] private AudioClip hitSeClip;
        [SerializeField] private AudioClip koSeClip;
        [SerializeField] private AudioClip victorySeClip;
        [SerializeField] private AudioClip defeatSeClip;
        [SerializeField] private AudioClip restartSeClip;
        [SerializeField] private AudioClip undoSeClip;

        private AudioSource bgmSource;
        private AudioSource seSource;
        private BgmTrack currentBgmTrack = BgmTrack.None;

        private AudioClip generatedTitleBgm;
        private AudioClip generatedStageSelectBgm;
        private AudioClip generatedBattleBgm;
        private AudioClip generatedCursorSe;
        private AudioClip generatedConfirmSe;
        private AudioClip generatedCancelSe;
        private AudioClip generatedAttackSe;
        private AudioClip generatedHitSe;
        private AudioClip generatedKoSe;
        private AudioClip generatedVictorySe;
        private AudioClip generatedDefeatSe;
        private AudioClip generatedRestartSe;
        private AudioClip generatedUndoSe;

        public static AudioManager Instance { get; private set; }

        public float MasterVolume
        {
            get => masterVolume;
            set
            {
                masterVolume = Clamp01(value);
                RefreshVolumes();
            }
        }

        public float BgmVolume
        {
            get => bgmVolume;
            set
            {
                bgmVolume = Clamp01(value);
                RefreshVolumes();
            }
        }

        public float SeVolume
        {
            get => seVolume;
            set => seVolume = Clamp01(value);
        }

        public bool MuteAll
        {
            get => muteAll;
            set
            {
                if (muteAll == value)
                {
                    return;
                }

                muteAll = value;
                RefreshVolumes();
                if (muteAll)
                {
                    bgmSource?.Stop();
                }
                else
                {
                    ResumeCurrentBgm();
                }
            }
        }

        public void PlayTitleBgm()
        {
            PlayBgm(BgmTrack.Title, titleBgmClip != null ? titleBgmClip : GetGeneratedTitleBgm());
        }

        public void PlayStageSelectBgm()
        {
            PlayBgm(BgmTrack.StageSelect, stageSelectBgmClip != null ? stageSelectBgmClip : GetGeneratedStageSelectBgm());
        }

        public void PlayBattleBgm()
        {
            PlayBgm(BgmTrack.Battle, battleBgmClip != null ? battleBgmClip : GetGeneratedBattleBgm());
        }

        public void StopBgm()
        {
            EnsureSources();
            currentBgmTrack = BgmTrack.None;
            bgmSource.Stop();
            bgmSource.clip = null;
        }

        public void PlayCursorSe() => PlaySe(cursorSeClip != null ? cursorSeClip : GetGeneratedCursorSe());
        public void PlayConfirmSe() => PlaySe(confirmSeClip != null ? confirmSeClip : GetGeneratedConfirmSe());
        public void PlayCancelSe() => PlaySe(cancelSeClip != null ? cancelSeClip : GetGeneratedCancelSe());
        public void PlayAttackSe() => PlaySe(attackSeClip != null ? attackSeClip : GetGeneratedAttackSe());
        public void PlayHitSe() => PlaySe(hitSeClip != null ? hitSeClip : GetGeneratedHitSe());
        public void PlayKoSe() => PlaySe(koSeClip != null ? koSeClip : GetGeneratedKoSe());
        public void PlayVictorySe() => PlaySe(victorySeClip != null ? victorySeClip : GetGeneratedVictorySe());
        public void PlayDefeatSe() => PlaySe(defeatSeClip != null ? defeatSeClip : GetGeneratedDefeatSe());
        public void PlayRestartSe() => PlaySe(restartSeClip != null ? restartSeClip : GetGeneratedRestartSe());
        public void PlayUndoSe() => PlaySe(undoSeClip != null ? undoSeClip : GetGeneratedUndoSe());

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            EnsureSources();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                MuteAll = !MuteAll;
                Debug.Log(MuteAll ? "Audio muted." : "Audio unmuted.");
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void EnsureSources()
        {
            if (bgmSource == null)
            {
                bgmSource = gameObject.AddComponent<AudioSource>();
                bgmSource.loop = true;
                bgmSource.playOnAwake = false;
            }

            if (seSource == null)
            {
                seSource = gameObject.AddComponent<AudioSource>();
                seSource.loop = false;
                seSource.playOnAwake = false;
            }

            RefreshVolumes();
        }

        private void PlayBgm(BgmTrack track, AudioClip clip)
        {
            EnsureSources();
            currentBgmTrack = track;

            if (clip == null || muteAll)
            {
                bgmSource.Stop();
                bgmSource.clip = clip;
                return;
            }

            if (bgmSource.clip == clip && bgmSource.isPlaying)
            {
                return;
            }

            bgmSource.Stop();
            bgmSource.clip = clip;
            bgmSource.loop = true;
            RefreshVolumes();
            bgmSource.Play();
        }

        private void ResumeCurrentBgm()
        {
            switch (currentBgmTrack)
            {
                case BgmTrack.Title:
                    PlayTitleBgm();
                    break;
                case BgmTrack.StageSelect:
                    PlayStageSelectBgm();
                    break;
                case BgmTrack.Battle:
                    PlayBattleBgm();
                    break;
            }
        }

        private void PlaySe(AudioClip clip)
        {
            EnsureSources();
            if (clip == null || muteAll)
            {
                return;
            }

            seSource.PlayOneShot(clip, masterVolume * seVolume);
        }

        private void RefreshVolumes()
        {
            if (bgmSource != null)
            {
                bgmSource.volume = muteAll ? 0f : masterVolume * bgmVolume;
            }
        }

        private AudioClip GetGeneratedTitleBgm()
        {
            return generatedTitleBgm ?? (generatedTitleBgm = CreateLoopClip("Generated_Title_BGM", 6f, new[] { 220f, 277.18f, 329.63f, 277.18f }, 0.055f));
        }

        private AudioClip GetGeneratedStageSelectBgm()
        {
            return generatedStageSelectBgm ?? (generatedStageSelectBgm = CreateLoopClip("Generated_StageSelect_BGM", 5f, new[] { 246.94f, 293.66f, 329.63f, 392f }, 0.05f));
        }

        private AudioClip GetGeneratedBattleBgm()
        {
            return generatedBattleBgm ?? (generatedBattleBgm = CreateLoopClip("Generated_Battle_BGM", 4f, new[] { 164.81f, 196f, 220f, 196f }, 0.06f));
        }

        private AudioClip GetGeneratedCursorSe()
        {
            return generatedCursorSe ?? (generatedCursorSe = CreateToneClip("Generated_Cursor_SE", 880f, 0.055f, 0.34f, 0.003f, 0.02f));
        }

        private AudioClip GetGeneratedConfirmSe()
        {
            return generatedConfirmSe ?? (generatedConfirmSe = CreateTwoToneClip("Generated_Confirm_SE", 660f, 990f, 0.12f, 0.36f));
        }

        private AudioClip GetGeneratedCancelSe()
        {
            return generatedCancelSe ?? (generatedCancelSe = CreateTwoToneClip("Generated_Cancel_SE", 420f, 260f, 0.12f, 0.3f));
        }

        private AudioClip GetGeneratedAttackSe()
        {
            return generatedAttackSe ?? (generatedAttackSe = CreateNoiseClip("Generated_Attack_SE", 0.09f, 0.25f, 420f));
        }

        private AudioClip GetGeneratedHitSe()
        {
            return generatedHitSe ?? (generatedHitSe = CreateToneClip("Generated_Hit_SE", 150f, 0.08f, 0.4f, 0.001f, 0.04f));
        }

        private AudioClip GetGeneratedKoSe()
        {
            return generatedKoSe ?? (generatedKoSe = CreateTwoToneClip("Generated_KO_SE", 180f, 90f, 0.28f, 0.42f));
        }

        private AudioClip GetGeneratedVictorySe()
        {
            return generatedVictorySe ?? (generatedVictorySe = CreateFanfareClip("Generated_Victory_SE", new[] { 523.25f, 659.25f, 783.99f, 1046.5f }, 0.5f));
        }

        private AudioClip GetGeneratedDefeatSe()
        {
            return generatedDefeatSe ?? (generatedDefeatSe = CreateFanfareClip("Generated_Defeat_SE", new[] { 392f, 293.66f, 246.94f, 196f }, 0.45f));
        }

        private AudioClip GetGeneratedRestartSe()
        {
            return generatedRestartSe ?? (generatedRestartSe = CreateTwoToneClip("Generated_Restart_SE", 520f, 390f, 0.14f, 0.32f));
        }

        private AudioClip GetGeneratedUndoSe()
        {
            return generatedUndoSe ?? (generatedUndoSe = CreateTwoToneClip("Generated_Undo_SE", 760f, 520f, 0.11f, 0.28f));
        }

        private static AudioClip CreateLoopClip(string clipName, float duration, float[] melody, float amplitude)
        {
            int sampleCount = Mathf.Max(1, (int)(SampleRate * duration));
            var samples = new float[sampleCount];
            for (int i = 0; i < sampleCount; i++)
            {
                float time = i / (float)SampleRate;
                int noteIndex = (int)(time * 2f) % melody.Length;
                float frequency = melody[noteIndex];
                float low = Mathf.Sin(TwoPi * frequency * time);
                float high = Mathf.Sin(TwoPi * frequency * 2f * time) * 0.25f;
                float pulse = Mathf.Sin(TwoPi * 2f * time) * 0.15f + 0.85f;
                samples[i] = (low + high) * amplitude * pulse;
            }

            return CreateClipFromSamples(clipName, samples);
        }

        private static AudioClip CreateToneClip(string clipName, float frequency, float duration, float amplitude, float attack, float release)
        {
            int sampleCount = Mathf.Max(1, (int)(SampleRate * duration));
            var samples = new float[sampleCount];
            for (int i = 0; i < sampleCount; i++)
            {
                float time = i / (float)SampleRate;
                float envelope = GetEnvelope(time, duration, attack, release);
                samples[i] = Mathf.Sin(TwoPi * frequency * time) * amplitude * envelope;
            }

            return CreateClipFromSamples(clipName, samples);
        }

        private static AudioClip CreateTwoToneClip(string clipName, float firstFrequency, float secondFrequency, float duration, float amplitude)
        {
            int sampleCount = Mathf.Max(1, (int)(SampleRate * duration));
            var samples = new float[sampleCount];
            float split = duration * 0.48f;
            for (int i = 0; i < sampleCount; i++)
            {
                float time = i / (float)SampleRate;
                float frequency = time < split ? firstFrequency : secondFrequency;
                float envelope = GetEnvelope(time, duration, 0.004f, 0.05f);
                samples[i] = Mathf.Sin(TwoPi * frequency * time) * amplitude * envelope;
            }

            return CreateClipFromSamples(clipName, samples);
        }

        private static AudioClip CreateFanfareClip(string clipName, float[] notes, float duration)
        {
            int sampleCount = Mathf.Max(1, (int)(SampleRate * duration));
            var samples = new float[sampleCount];
            float noteDuration = duration / notes.Length;
            for (int i = 0; i < sampleCount; i++)
            {
                float time = i / (float)SampleRate;
                int noteIndex = Mathf.Min(notes.Length - 1, (int)(time / noteDuration));
                float localTime = time - noteIndex * noteDuration;
                float envelope = GetEnvelope(localTime, noteDuration, 0.004f, 0.045f);
                samples[i] = Mathf.Sin(TwoPi * notes[noteIndex] * time) * 0.34f * envelope;
            }

            return CreateClipFromSamples(clipName, samples);
        }

        private static AudioClip CreateNoiseClip(string clipName, float duration, float amplitude, float toneFrequency)
        {
            int sampleCount = Mathf.Max(1, (int)(SampleRate * duration));
            var samples = new float[sampleCount];
            uint state = 22222u;
            for (int i = 0; i < sampleCount; i++)
            {
                float time = i / (float)SampleRate;
                state = state * 1664525u + 1013904223u;
                float noise = ((state >> 16) / 32768f) * 2f - 1f;
                float tone = Mathf.Sin(TwoPi * toneFrequency * time) * 0.35f;
                float envelope = GetEnvelope(time, duration, 0.001f, 0.04f);
                samples[i] = (noise * 0.65f + tone) * amplitude * envelope;
            }

            return CreateClipFromSamples(clipName, samples);
        }

        private static AudioClip CreateClipFromSamples(string clipName, float[] samples)
        {
            var clip = AudioClip.Create(clipName, samples.Length, 1, SampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private static float GetEnvelope(float time, float duration, float attack, float release)
        {
            if (attack > 0f && time < attack)
            {
                return Clamp01(time / attack);
            }

            float releaseStart = duration - release;
            if (release > 0f && time > releaseStart)
            {
                return Clamp01((duration - time) / release);
            }

            return 1f;
        }

        private static float Clamp01(float value)
        {
            if (value < 0f)
            {
                return 0f;
            }

            if (value > 1f)
            {
                return 1f;
            }

            return value;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void BootstrapAudioManager()
        {
            if (FindAnyObjectByType<AudioManager>() != null)
            {
                return;
            }

            var audioObject = new GameObject("AudioManager");
            audioObject.AddComponent<AudioManager>();
        }
    }
}

#pragma warning restore 0649

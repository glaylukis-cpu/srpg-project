using UnityEngine;

namespace SRPG.Persistence
{
    public struct StageBestRecord
    {
        public string Rating;
        public int TurnNumber;
        public int Survivors;
        public int HpTotal;
    }

    public static class GameSaveData
    {
        private const string Prefix = "FinalEscapeTactics.Save.";
        private const int CurrentSaveVersion = 1;

        private const string SaveVersionKey = Prefix + "Version";
        private const string HighestUnlockedStageKey = Prefix + "Progress.HighestUnlockedStage";
        private const string MasterVolumeKey = Prefix + "Settings.MasterVolume";
        private const string BgmVolumeKey = Prefix + "Settings.BgmVolume";
        private const string SeVolumeKey = Prefix + "Settings.SeVolume";
        private const string MuteKey = Prefix + "Settings.Mute";
        private const string DisplayConfiguredKey = Prefix + "Settings.DisplayConfigured";
        private const string DisplayWidthKey = Prefix + "Settings.DisplayWidth";
        private const string DisplayHeightKey = Prefix + "Settings.DisplayHeight";
        private const string FullscreenKey = Prefix + "Settings.Fullscreen";

        public static void EnsureInitialized()
        {
            if (PlayerPrefs.GetInt(SaveVersionKey, 0) >= CurrentSaveVersion)
            {
                return;
            }

            PlayerPrefs.SetInt(SaveVersionKey, CurrentSaveVersion);
            PlayerPrefs.SetInt(HighestUnlockedStageKey, Mathf.Max(1, PlayerPrefs.GetInt(HighestUnlockedStageKey, 1)));
            PlayerPrefs.Save();
        }

        public static int GetHighestUnlockedStageIndex(int totalStages)
        {
            if (totalStages <= 0)
            {
                return 0;
            }

            EnsureInitialized();
            var unlockedStageNumber = PlayerPrefs.GetInt(HighestUnlockedStageKey, 1);
            return Mathf.Max(0, Mathf.Min(unlockedStageNumber - 1, totalStages - 1));
        }

        public static bool IsStageUnlocked(int stageIndex, int totalStages)
        {
            return stageIndex >= 0 && stageIndex <= GetHighestUnlockedStageIndex(totalStages);
        }

        public static bool IsStageCleared(int stageNumber)
        {
            return stageNumber > 0 && PlayerPrefs.GetInt(GetStageKey(stageNumber, "Cleared"), 0) == 1;
        }

        public static void RecordStageClear(int stageNumber, int totalStages, string rating, int turnNumber, int survivors, int hpTotal)
        {
            if (stageNumber <= 0 || totalStages <= 0 || stageNumber > totalStages)
            {
                return;
            }

            EnsureInitialized();
            var candidate = new StageBestRecord
            {
                Rating = string.IsNullOrEmpty(rating) ? "CLEAR" : rating,
                TurnNumber = Mathf.Max(0, turnNumber),
                Survivors = Mathf.Max(0, survivors),
                HpTotal = Mathf.Max(0, hpTotal)
            };

            if (!TryGetStageRecord(stageNumber, out var currentBest) || IsBetterRecord(candidate, currentBest))
            {
                PlayerPrefs.SetString(GetStageKey(stageNumber, "Rating"), candidate.Rating);
                PlayerPrefs.SetInt(GetStageKey(stageNumber, "Turn"), candidate.TurnNumber);
                PlayerPrefs.SetInt(GetStageKey(stageNumber, "Survivors"), candidate.Survivors);
                PlayerPrefs.SetInt(GetStageKey(stageNumber, "HpTotal"), candidate.HpTotal);
            }

            PlayerPrefs.SetInt(GetStageKey(stageNumber, "Cleared"), 1);
            var nextUnlockedStage = Mathf.Min(totalStages, stageNumber + 1);
            var currentUnlockedStage = PlayerPrefs.GetInt(HighestUnlockedStageKey, 1);
            PlayerPrefs.SetInt(HighestUnlockedStageKey, Mathf.Max(currentUnlockedStage, nextUnlockedStage));
            PlayerPrefs.Save();
        }

        public static bool TryGetStageRecord(int stageNumber, out StageBestRecord record)
        {
            record = default;
            if (!IsStageCleared(stageNumber))
            {
                return false;
            }

            record = new StageBestRecord
            {
                Rating = PlayerPrefs.GetString(GetStageKey(stageNumber, "Rating"), "CLEAR"),
                TurnNumber = Mathf.Max(0, PlayerPrefs.GetInt(GetStageKey(stageNumber, "Turn"), 0)),
                Survivors = Mathf.Max(0, PlayerPrefs.GetInt(GetStageKey(stageNumber, "Survivors"), 0)),
                HpTotal = Mathf.Max(0, PlayerPrefs.GetInt(GetStageKey(stageNumber, "HpTotal"), 0))
            };
            return true;
        }

        public static float GetMasterVolume(float fallback) => GetVolume(MasterVolumeKey, fallback);
        public static float GetBgmVolume(float fallback) => GetVolume(BgmVolumeKey, fallback);
        public static float GetSeVolume(float fallback) => GetVolume(SeVolumeKey, fallback);
        public static bool GetMute(bool fallback) => PlayerPrefs.GetInt(MuteKey, fallback ? 1 : 0) == 1;

        public static void SaveAudioSettings(float masterVolume, float bgmVolume, float seVolume, bool mute)
        {
            EnsureInitialized();
            PlayerPrefs.SetFloat(MasterVolumeKey, SanitizeVolume(masterVolume, 0.7f));
            PlayerPrefs.SetFloat(BgmVolumeKey, SanitizeVolume(bgmVolume, 0.25f));
            PlayerPrefs.SetFloat(SeVolumeKey, SanitizeVolume(seVolume, 0.45f));
            PlayerPrefs.SetInt(MuteKey, mute ? 1 : 0);
            PlayerPrefs.Save();
        }

        public static bool TryGetDisplaySettings(out int width, out int height, out bool fullscreen)
        {
            width = PlayerPrefs.GetInt(DisplayWidthKey, 0);
            height = PlayerPrefs.GetInt(DisplayHeightKey, 0);
            fullscreen = PlayerPrefs.GetInt(FullscreenKey, 1) == 1;
            return PlayerPrefs.GetInt(DisplayConfiguredKey, 0) == 1 && width > 0 && height > 0;
        }

        public static void SaveDisplaySettings(int width, int height, bool fullscreen)
        {
            if (width <= 0 || height <= 0)
            {
                return;
            }

            EnsureInitialized();
            PlayerPrefs.SetInt(DisplayWidthKey, width);
            PlayerPrefs.SetInt(DisplayHeightKey, height);
            PlayerPrefs.SetInt(FullscreenKey, fullscreen ? 1 : 0);
            PlayerPrefs.SetInt(DisplayConfiguredKey, 1);
            PlayerPrefs.Save();
        }

        private static float GetVolume(string key, float fallback)
        {
            return SanitizeVolume(PlayerPrefs.GetFloat(key, fallback), fallback);
        }

        private static float SanitizeVolume(float value, float fallback)
        {
            if (float.IsNaN(value) || float.IsInfinity(value))
            {
                value = fallback;
            }

            return Mathf.Clamp01(value);
        }

        private static bool IsBetterRecord(StageBestRecord candidate, StageBestRecord currentBest)
        {
            var candidateRating = GetRatingScore(candidate.Rating);
            var currentRating = GetRatingScore(currentBest.Rating);
            if (candidateRating != currentRating)
            {
                return candidateRating > currentRating;
            }

            if (candidate.TurnNumber != currentBest.TurnNumber)
            {
                return candidate.TurnNumber < currentBest.TurnNumber;
            }

            if (candidate.Survivors != currentBest.Survivors)
            {
                return candidate.Survivors > currentBest.Survivors;
            }

            return candidate.HpTotal > currentBest.HpTotal;
        }

        private static int GetRatingScore(string rating)
        {
            switch (rating)
            {
                case "S": return 6;
                case "A": return 5;
                case "B": return 4;
                case "C": return 3;
                case "D": return 2;
                case "CLEAR": return 1;
                default: return 0;
            }
        }

        private static string GetStageKey(int stageNumber, string field)
        {
            return Prefix + "Progress.Stage" + stageNumber + "." + field;
        }
    }
}

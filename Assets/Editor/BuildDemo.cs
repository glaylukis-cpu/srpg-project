#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SRPG.EditorTools
{
    public static class BuildDemo
    {
        private const string BuildScenePath = "Assets/GeneratedBuildScenes/AutoBootstrap.unity";
        private const string WindowsBuildDirectory = "Builds/Windows/Release";
        private const string WindowsBuildPath = WindowsBuildDirectory + "/FinalEscapeTactics.exe";

        public static void BuildWindows()
        {
            ValidateReleaseSettings();
            PrepareBuildDirectory();
            string scenePath = EnsureBootstrapScene();

            var buildOptions = new BuildPlayerOptions
            {
                scenes = new[] { scenePath },
                locationPathName = WindowsBuildPath,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.StrictMode
            };

            Debug.Log($"Starting Windows build: {WindowsBuildPath}");
            var report = BuildPipeline.BuildPlayer(buildOptions);
            var summary = report.summary;

            Debug.Log($"Windows build result: {summary.result}, Size: {summary.totalSize} bytes, Time: {summary.totalTime}");
            if (summary.result != BuildResult.Succeeded)
            {
                throw new InvalidOperationException($"Windows build failed: {summary.result}");
            }
        }

        private static void ValidateReleaseSettings()
        {
            if (string.IsNullOrWhiteSpace(PlayerSettings.companyName) || PlayerSettings.companyName == "DefaultCompany")
            {
                throw new InvalidOperationException("Release build requires a non-default Company Name.");
            }

            if (string.IsNullOrWhiteSpace(PlayerSettings.productName) || PlayerSettings.productName.Contains("unity-"))
            {
                throw new InvalidOperationException("Release build requires a final Product Name.");
            }

            if (string.IsNullOrWhiteSpace(PlayerSettings.bundleVersion))
            {
                throw new InvalidOperationException("Release build requires a version.");
            }
        }

        private static void PrepareBuildDirectory()
        {
            var projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            var outputDirectory = Path.GetFullPath(WindowsBuildDirectory);
            if (!outputDirectory.StartsWith(projectRoot + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Release build directory must remain inside the project.");
            }

            if (Directory.Exists(outputDirectory))
            {
                Directory.Delete(outputDirectory, true);
            }

            Directory.CreateDirectory(outputDirectory);
        }

        private static string EnsureBootstrapScene()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(BuildScenePath));

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            EditorSceneManager.SaveScene(scene, BuildScenePath);
            AssetDatabase.Refresh();
            Debug.Log($"Bootstrap scene prepared: {BuildScenePath}");
            return BuildScenePath;
        }
    }
}
#endif

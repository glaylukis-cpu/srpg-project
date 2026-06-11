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
        private const string WindowsBuildPath = "Builds/Windows/FinalEscapeTacticsDemo.exe";

        public static void BuildWindows()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(WindowsBuildPath));
            string scenePath = EnsureBootstrapScene();

            var buildOptions = new BuildPlayerOptions
            {
                scenes = new[] { scenePath },
                locationPathName = WindowsBuildPath,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
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

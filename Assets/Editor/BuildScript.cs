using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using UnityEditor.Build.Reporting;

public class BuildScript
{
    [MenuItem("Build/Build Game")]
    public static void BuildGame()
    {
        // Get filename.
        string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");
        if (path.Length == 0)
            return;

        // Build player.
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = GetBuildScenes();
        buildPlayerOptions.locationPathName = Path.Combine(path, "PuzzleGame");
        buildPlayerOptions.target = BuildTarget.StandaloneOSX;
        buildPlayerOptions.options = BuildOptions.Development;

        Debug.Log("Starting build process...");
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        LogBuildReport(report);
    }

    public static void TerminalBuild()
    {
        Debug.Log("Starting terminal build process...");
        
        // Verify scene exists
        string scenePath = "Assets/Scenes/GameScene.unity";
        if (!File.Exists(scenePath))
        {
            Debug.LogError($"Scene not found at path: {scenePath}");
            return;
        }
        Debug.Log($"Found scene at path: {scenePath}");

        // Create build directory
        string buildPath = Path.Combine(Application.dataPath, "..", "Builds");
        if (!Directory.Exists(buildPath))
        {
            Debug.Log($"Creating build directory at: {buildPath}");
            Directory.CreateDirectory(buildPath);
        }

        // Configure build options
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = GetBuildScenes();
        buildPlayerOptions.locationPathName = Path.Combine(buildPath, "PuzzleGame");
        buildPlayerOptions.target = BuildTarget.StandaloneOSX;
        buildPlayerOptions.options = BuildOptions.Development;

        // Start build
        Debug.Log($"Building to: {buildPlayerOptions.locationPathName}");
        Debug.Log($"Included scenes: {string.Join(", ", buildPlayerOptions.scenes)}");
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        LogBuildReport(report);
    }

    private static string[] GetBuildScenes()
    {
        // Get all enabled scenes in build settings
        var scenes = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();

        Debug.Log($"Found {scenes.Length} enabled scenes in build settings");
        foreach (var scene in scenes)
        {
            Debug.Log($"Scene included in build: {scene}");
        }

        if (scenes.Length == 0)
        {
            // If no scenes are in build settings, use GameScene
            scenes = new[] { "Assets/Scenes/GameScene.unity" };
            Debug.Log("No scenes in build settings, defaulting to GameScene.unity");
        }

        return scenes;
    }

    private static void LogBuildReport(BuildReport report)
    {
        BuildSummary summary = report.summary;
        
        Debug.Log($"Build completed in {summary.totalTime.TotalSeconds:F2} seconds");
        Debug.Log($"Build size: {summary.totalSize / 1024 / 1024:F2}MB");
        Debug.Log($"Build result: {summary.result}");

        if (summary.result == BuildResult.Failed)
        {
            Debug.LogError("Build failed!");
            foreach (var step in report.steps)
            {
                Debug.LogError($"Build step '{step.name}' duration: {step.duration:F2}s");
            }
            foreach (var message in report.strippingInfo.includedModules)
            {
                Debug.LogError($"Included module: {message}");
            }
        }
        else
        {
            Debug.Log("Build succeeded!");
            Debug.Log($"Output path: {summary.outputPath}");
        }
    }
} 
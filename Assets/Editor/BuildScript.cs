using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using UnityEditor.Build.Reporting;

public class BuildScript
{
    private static void ValidateBuild()
    {
        // Check if main scene exists
        if (!File.Exists("Assets/Scenes/GameScene.unity"))
        {
            throw new System.Exception("Main scene (GameScene.unity) not found!");
        }

        // Check if required prefabs exist
        if (!File.Exists("Assets/Prefabs/CandyTile.prefab") || 
            !File.Exists("Assets/Prefabs/SpecialCandyTile.prefab") ||
            !File.Exists("Assets/Prefabs/MatchEffect.prefab"))
        {
            throw new System.Exception("Required prefabs are missing!");
        }

        // Check if required scripts exist
        if (!File.Exists("Assets/Scripts/GameManager.cs"))
        {
            throw new System.Exception("GameManager script is missing!");
        }
    }

    private static void PostBuildProcessing(string buildPath)
    {
        try
        {
            // Create a build info file
            string buildInfoPath = Path.Combine(buildPath, "build_info.txt");
            using (StreamWriter writer = new StreamWriter(buildInfoPath))
            {
                writer.WriteLine($"Build Time: {System.DateTime.Now}");
                writer.WriteLine($"Unity Version: {Application.unityVersion}");
                writer.WriteLine($"Build Target: {EditorUserBuildSettings.activeBuildTarget}");
            }

            Debug.Log($"Build info saved to: {buildInfoPath}");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Post-build processing failed: {e.Message}");
        }
    }

    [MenuItem("Build/Build All")]
    public static void BuildAll()
    {
        try
        {
            Debug.Log("Starting build process...");
            
            // Validate build
            ValidateBuild();
            Debug.Log("Build validation passed");

            // Set player settings
            PlayerSettings.productName = "Puzzle Game";
            PlayerSettings.companyName = "Your Company";
            PlayerSettings.applicationIdentifier = "com.yourcompany.puzzlegame";
            PlayerSettings.macOS.buildNumber = "1";
            Debug.Log("Player settings configured");
            
            // Optimize build settings
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.Mono2x);
            PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Standalone, ApiCompatibilityLevel.NET_4_6);
            PlayerSettings.stripEngineCode = true;
            PlayerSettings.allowUnsafeCode = false;
            Debug.Log("Build settings optimized");
            
            // Create build directory if it doesn't exist
            string buildPath = Path.Combine(Directory.GetCurrentDirectory(), "Build");
            if (!Directory.Exists(buildPath))
            {
                Directory.CreateDirectory(buildPath);
                Debug.Log($"Created build directory at: {buildPath}");
            }

            // Get scenes
            string[] scenes = new string[] { "Assets/Scenes/GameScene.unity" };
            Debug.Log($"Building with scene: {scenes[0]}");

            // Build options
            BuildOptions buildOptions = BuildOptions.None;
            
            // Development build options
            #if UNITY_EDITOR
            buildOptions |= BuildOptions.Development;
            #endif

            // Build for macOS
            string appPath = Path.Combine(buildPath, "PuzzleGame.app");
            Debug.Log($"Starting build to: {appPath}");
            
            BuildReport report = BuildPipeline.BuildPlayer(scenes, appPath, BuildTarget.StandaloneOSX, buildOptions);
            
            if (report.summary.result == BuildResult.Succeeded)
            {
                Debug.Log("Build completed successfully!");
            }
            else
            {
                Debug.LogError($"Build failed with result: {report.summary.result}");
                Debug.LogError($"Total errors: {report.summary.totalErrors}");
                Debug.LogError($"Total warnings: {report.summary.totalWarnings}");
                foreach (var step in report.steps)
                {
                    Debug.LogError($"Build step: {step.name}, Duration: {step.duration.TotalSeconds}s");
                    foreach (var message in step.messages)
                    {
                        Debug.LogError($"Message: {message.content}, Type: {message.type}");
                    }
                }
                throw new System.Exception($"Build failed with result: {report.summary.result}");
            }

            // Verify build
            if (!Directory.Exists(appPath))
            {
                throw new System.Exception($"Build verification failed: {appPath} not found");
            }

            // Post-build processing
            PostBuildProcessing(buildPath);

            Debug.Log($"Build completed successfully! App path: {appPath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Build failed: {e.Message}\nStackTrace: {e.StackTrace}");
            throw;
        }
    }

    [MenuItem("Build/Clean Build")]
    public static void CleanBuild()
    {
        try
        {
            // Delete existing build directory
            string buildPath = "Build";
            if (Directory.Exists(buildPath))
            {
                Directory.Delete(buildPath, true);
            }

            // Rebuild
            BuildAll();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Clean build failed: " + e.Message);
            throw;
        }
    }

    [MenuItem("Build/Development Build")]
    public static void DevelopmentBuild()
    {
        try
        {
            // Validate build
            ValidateBuild();

            // Set development build settings
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.IL2CPP);
            PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Standalone, ApiCompatibilityLevel.NET_4_6);
            
            // Create build directory if it doesn't exist
            string buildPath = "Build/Development";
            if (!Directory.Exists(buildPath))
            {
                Directory.CreateDirectory(buildPath);
            }

            // Get scenes
            string[] scenes = new string[] { "Assets/Scenes/GameScene.unity" };

            // Build options for development
            BuildOptions buildOptions = BuildOptions.Development | BuildOptions.AllowDebugging;

            // Build for macOS
            BuildPipeline.BuildPlayer(scenes, 
                Path.Combine(buildPath, "PuzzleGame_Dev.app"), 
                BuildTarget.StandaloneOSX, 
                buildOptions);

            // Post-build processing
            PostBuildProcessing(buildPath);

            Debug.Log("Development build completed successfully!");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Development build failed: " + e.Message);
            throw;
        }
    }

    [MenuItem("Build/Run Build")]
    public static void RunBuild()
    {
        try
        {
            // Build the game
            BuildAll();

            // Run the built game
            string buildPath = Path.Combine(Directory.GetCurrentDirectory(), "Build", "PuzzleGame.app");
            if (File.Exists(buildPath))
            {
                System.Diagnostics.Process.Start(buildPath);
                Debug.Log("Game started successfully!");
            }
            else
            {
                throw new System.Exception("Built game not found!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Run build failed: " + e.Message);
            throw;
        }
    }

    [MenuItem("Build/Terminal Build")]
    public static void TerminalBuild()
    {
        try
        {
            // Create build directory if it doesn't exist
            string buildPath = Path.Combine(Directory.GetCurrentDirectory(), "Build");
            if (!Directory.Exists(buildPath))
            {
                Directory.CreateDirectory(buildPath);
            }

            // Set build settings
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.Mono2x);
            PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Standalone, ApiCompatibilityLevel.NET_4_6);
            PlayerSettings.stripEngineCode = false; // Disable code stripping for stability
            PlayerSettings.allowUnsafeCode = false;

            // Get scenes
            string[] scenes = new string[] { "Assets/Scenes/GameScene.unity" };

            // Build options
            BuildOptions buildOptions = BuildOptions.None;

            // Build for macOS
            string appPath = Path.Combine(buildPath, "PuzzleGame.app");
            Debug.Log($"Starting build to: {appPath}");

            BuildReport report = BuildPipeline.BuildPlayer(scenes, appPath, BuildTarget.StandaloneOSX, buildOptions);

            if (report.summary.result == BuildResult.Succeeded)
            {
                Debug.Log("Build completed successfully!");
                // Run the built game
                if (Directory.Exists(appPath))
                {
                    System.Diagnostics.Process.Start("open", appPath);
                    Debug.Log("Game started successfully!");
                }
                else
                {
                    throw new System.Exception($"Built game not found at: {appPath}");
                }
            }
            else
            {
                Debug.LogError($"Build failed with result: {report.summary.result}");
                Debug.LogError($"Total errors: {report.summary.totalErrors}");
                Debug.LogError($"Total warnings: {report.summary.totalWarnings}");
                foreach (var step in report.steps)
                {
                    Debug.LogError($"Build step: {step.name}, Duration: {step.duration.TotalSeconds}s");
                    foreach (var message in step.messages)
                    {
                        Debug.LogError($"Message: {message.content}, Type: {message.type}");
                    }
                }
                throw new System.Exception($"Build failed with result: {report.summary.result}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Terminal build failed: {e.Message}");
            throw;
        }
    }
} 
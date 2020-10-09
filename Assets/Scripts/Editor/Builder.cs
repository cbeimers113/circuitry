using UnityEditor;

public class Builder
{
    public static void DevelopmentBuild()
    {
        BuildPipeline.BuildPlayer(new string[] { "Assets/Scenes/Design.unity"}, "Build/Techne.exe", BuildTarget.StandaloneWindows, BuildOptions.Development);
    }
}
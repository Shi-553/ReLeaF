using UnityEngine.SceneManagement;
/// <summary>
/// シーンの種類を管理する列挙型
/// <summary>
public enum SceneType
{
    SampleScene = 0,

    Manager = 1,

    Title = 2,

}
public static class SceneTypeExtension
{
    public static int GetBuildIndex(this SceneType type)
    {
        return type switch
        {
            SceneType.Title => 0,
            SceneType.SampleScene => 1,
            SceneType.Manager => 2,
            _ => 0,
        };
    }
    public static SceneType GetSceneType(this Scene scene)
    {
        return scene.buildIndex switch
        {
            0 => SceneType.Title,
            1 => SceneType.SampleScene,
            2 => SceneType.Manager,
            _ => 0,
        };
    }
}

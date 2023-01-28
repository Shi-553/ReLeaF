using UnityEngine.SceneManagement;
/// <summary>
/// シーンの種類を管理する列挙型
/// <summary>
public enum SceneType
{
    Title = 2,

    Manager = 1,

    Menu = 4,

    SampleScene = 0,

    Tutorial = 7,

    stage1 = 8,

    stage2 = 9,

    stage3 = 10,

    stage4 = 11,

    stage5 = 12,

    stage6 = 13,

    stage7 = 14,

}
public static class SceneTypeExtension
{
   public static int GetBuildIndex(this SceneType type)
   {
      return type switch                         
      {                                          
          SceneType.Title => 0,              
          SceneType.Manager => 1,              
          SceneType.Menu => 2,              
          SceneType.SampleScene => 3,              
          SceneType.Tutorial => 4,              
          SceneType.stage1 => 5,              
          SceneType.stage2 => 6,              
          SceneType.stage3 => 7,              
          SceneType.stage4 => 8,              
          SceneType.stage5 => 9,              
          SceneType.stage6 => 10,              
          SceneType.stage7 => 11,              
          _ => 0,                                
      };                                         
   }
   public static SceneType GetSceneType(this Scene scene)
   {
      return GetSceneType(scene.buildIndex);                         
   }
   public static SceneType GetSceneType(int buildIndex)
   {
      return buildIndex switch                         
      {                                          
          0 => SceneType.Title,              
          1 => SceneType.Manager,              
          2 => SceneType.Menu,              
          3 => SceneType.SampleScene,              
          4 => SceneType.Tutorial,              
          5 => SceneType.stage1,              
          6 => SceneType.stage2,              
          7 => SceneType.stage3,              
          8 => SceneType.stage4,              
          9 => SceneType.stage5,              
          10 => SceneType.stage6,              
          11 => SceneType.stage7,              
          _ => 0,                                
      };                                         
   }
}

#define DEFINE_SCENE_TYPE_ENUM
/// <summary>
/// シーンの種類を管理する列挙型
/// <summary>
public enum SceneType
{
    SampleScene = 0,

}
public static class SceneTypeExtension
{
   public static int GetBuildIndex(this SceneType type)
   {
      return type switch                         
      {                                          
          SceneType.SampleScene => 0,              
          _ => 0,                                
      };                                         
   }
}

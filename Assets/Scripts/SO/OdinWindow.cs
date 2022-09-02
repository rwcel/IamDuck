#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class OdinWindow : OdinMenuEditorWindow
{
    [MenuItem("AbleX/Level")]
    private static void OpenWindow()
    {
        GetWindow<OdinWindow>().Show();
    }

    // Adressable 사용 예정
    public static string Path_Floor = "Assets/GameDatas/1. Floors";
    public static string Path_InGameItem = "Assets/GameDatas/2. InGameItem";
    public static string Path_Obstacle = "Assets/GameDatas/3. Obstacle";
    public static string Path_Mission = "Assets/GameDatas/6. Mission";
    public static string Path_LevelData = "Assets/Resources/LevelData.asset";
    public static string Path_GameData = "Assets/Resources/GameData.asset";
    public static string Path_AudioData = "Assets/Resources/AudioData.asset";

    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree();

        tree.AddAssetAtPath("Game Data", Path_GameData);
        tree.AddAssetAtPath("Audio Data", Path_AudioData);
        tree.AddAssetAtPath("Level Data", Path_LevelData);

        tree.AddAllAssetsAtPath("Floor Data", Path_Floor, typeof(FloorData), true);
        tree.AddAllAssetsAtPath("Obstacle Data", Path_Obstacle, typeof(ObstacleData), true); //.AddIcon(data);
        tree.AddAllAssetsAtPath("GameItem Data", Path_InGameItem, typeof(InGameItemData)).AddThumbnailIcons();

        tree.AddAllAssetsAtPath("Mission Data", Path_Mission, typeof(MissionData));

        tree.EnumerateTree().AddThumbnailIcons();

        return tree;
    }

    //public class CreateFloorData
    //{
    //    public string dataName;
    //    [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
    //    public FloorData data;

    //    public CreateFloorData()
    //    {
    //        data = CreateInstance<FloorData>();
    //        dataName = "New Floor Data";
    //    }

    //    [Button("Add Floor SO")]
    //    private void CreateNewData()
    //    {
    //        AssetDatabase.CreateAsset(data, $"{Values.Path_Floor}/{dataName}.asset");
    //        AssetDatabase.SaveAssets();

    //        data = CreateInstance<FloorData>();
    //        dataName = "New Floor Data";
    //    }
    //}

    //public class CreateObstacleData
    //{
    //    public string dataName;
    //    [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
    //    public ObstacleData data;

    //    public CreateObstacleData()
    //    {
    //        data = CreateInstance<ObstacleData>();
    //        dataName = "New Obstacle Data";
    //    }

    //    [Button("Add Obstacle SO")]
    //    private void CreateNewData()
    //    {
    //        AssetDatabase.CreateAsset(data, $"{Values.Path_Obstacle}/{dataName}.asset");
    //        AssetDatabase.SaveAssets();

    //        data = CreateInstance<ObstacleData>();
    //        dataName = "New Obstacle Data";
    //    }
    //}

    //public class CreateInGameItemData
    //{
    //    public string dataName;
    //    [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
    //    public InGameItemData data;

    //    public CreateInGameItemData()
    //    {
    //        data = CreateInstance<InGameItemData>();
    //        dataName = "New InGameItem Data";
    //    }

    //    [Button("Add InGameItem SO")]
    //    private void CreateNewData()
    //    {
    //        AssetDatabase.CreateAsset(data, $"{Values.Path_InGameItem}/{dataName}.asset");
    //        AssetDatabase.SaveAssets();

    //        data = CreateInstance<InGameItemData>();
    //        dataName = "New InGameItem Data";
    //    }
    //}
}
#endif
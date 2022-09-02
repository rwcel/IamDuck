using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class GameSceneEditor : MonoBehaviour
{
#if UNITY_EDITOR

    private static readonly string _Scene_Path = "Assets/Scenes/";

    #region æ¿ ¿Ãµø

    [MenuItem("AbleX/Scene/CI")]
    private static void ToCIScene()
    {
        MoveSceneEditor(EScene.CI.ToString());
    }

    [MenuItem("AbleX/Scene/Intro")]
    private static void ToIntroScene()
    {
        MoveSceneEditor(EScene.Intro.ToString());
    }

    [MenuItem("AbleX/Scene/OutGame")]
    private static void ToLobbyScene()
    {
        MoveSceneEditor(EScene.OutGame.ToString());
    }

    [MenuItem("AbleX/Scene/InGame")]
    private static void ToInGameScene()
    {
        MoveSceneEditor(EScene.InGame.ToString());
    }


    private static void MoveSceneEditor(string sceneName)
    {
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene($"{_Scene_Path}{sceneName}.unity");
    }

    #endregion æ¿ ¿Ãµø

#endif
}

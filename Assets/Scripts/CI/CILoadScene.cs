using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CILoadScene : MonoBehaviour
{
    // CI 재생이 끝나면 애니메이션에서 호출. 
    public void LoadMyIntroScene()
    {
        GameSceneManager.Instance.MoveScene(EScene.Intro, ETransition.Vertical);
    }
}

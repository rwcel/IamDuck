using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CILoadScene : MonoBehaviour
{
    // CI ����� ������ �ִϸ��̼ǿ��� ȣ��. 
    public void LoadMyIntroScene()
    {
        GameSceneManager.Instance.MoveScene(EScene.Intro, ETransition.Vertical);
    }
}

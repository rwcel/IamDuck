using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class BackgroundScrolling : MonoBehaviour
{
    // 1f�� y = 4
    // y = 700, 175f
    // 175*2���϶� ������������ �ø��� : 700*5 = 3500

    [SerializeField] Transform[] backgrounds;
    [SerializeField] float perY = 700;
    float floorNum;
    int curMoveNum;


    private void Start()
    {
        floorNum = (int)(perY / Values.FloorPerY);
        curMoveNum = 0;

        // 175*2���� ����
        InGameManager.Instance.Height
            .Where(y => y % floorNum == 0 && y != floorNum && y != 0)
            .Subscribe(_ => Movebackground())
            .AddTo(this.gameObject);
    }

    void Movebackground()
    {
        Debug.Log($"{gameObject.name} : scrolling");
        backgrounds[curMoveNum++].position += Vector3.up * backgrounds.Length * perY;
    }
}

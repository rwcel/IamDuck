using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallController : MonoBehaviour
{
    [SerializeField] Transform decoParent;

    private List<GameObject> decoObjs;
    private int curDecoNum = -1;

    private void Awake()
    {
        decoObjs = new List<GameObject>(Values.Length_WallType);
        foreach (Transform deco in decoParent)
        {
            decoObjs.Add(deco.gameObject);
        }
    }

    public void InitialiseWithData(bool isActive)
    {
        if (isActive)
        {
            SetDeco(Random.Range(0, Values.Length_WallType));
        }

        gameObject.SetActive(isActive);
    }

    private void SetDeco(int arrayNum)
    {
        if(curDecoNum != -1)
        {
            decoObjs[curDecoNum].SetActive(false);
        }

        curDecoNum = arrayNum;
        decoObjs[curDecoNum].SetActive(true);
    }
}

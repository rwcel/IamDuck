using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EActionEvent
{
    Coin,
    Height,
}

public interface IObserver
{
    void OnNotify(int value, EActionEvent action);
}

public class Achievement : MonoBehaviour, IObserver
{
    public void OnNotify(int value, EActionEvent action)
    {
        switch (action)
        {
            case EActionEvent.Coin:
                // 
                break;
            case EActionEvent.Height:
                // 
                break;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamOutside : MonoBehaviour
{
    public System.Action OnInvisible;

    private bool isActive;

    private void OnEnable()
    {
        isActive = true;
    }

    private void OnDisable()
    {
        isActive = false;
    }

    void OnApplicationQuit()
    {
        isActive = false;
    }

    private void OnBecameInvisible()
    {
        if (!isActive)
            return;

        Debug.LogWarning("화면 밖으로 나감");

        OnInvisible?.Invoke();
    }
}

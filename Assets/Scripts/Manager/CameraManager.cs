using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : Singleton<CameraManager>
{
    [SerializeField] CinemachineVirtualCamera vcam;

    private Vector3 startPos;
    private System.Action resetAction;

    protected override void AwakeInstance()
    {
        // x = 1920 * 1920 / 1080 / [size]      DivValue = 380
        float camSize = Values.ScreenSize_Y * Screen.height / Screen.width / Values.ScreenDivValue;
        if (camSize < Values.MinScreenValue)
            camSize = Values.MinScreenValue;
        vcam.m_Lens.OrthographicSize = camSize;
    }

    private void Start()
    {
        startPos = transform.position;
    }

    protected override void DestroyInstance()
    {
        
    }

    public void ResetVCam(System.Action endAction)
    {
        resetAction = endAction;

        StartCoroutine(nameof(CoResetVCam));
    }

    /// <summary>
    /// **�ڿ������� Lerp �� �� �ִ� ���?
    /// </summary>
    /// <returns></returns>
    IEnumerator CoResetVCam()
    {
        vcam.gameObject.SetActive(false);
        yield return null;
        vcam.gameObject.SetActive(true);


        resetAction?.Invoke();
    }
}

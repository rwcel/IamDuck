using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameSceneManager : Singleton<GameSceneManager>
{
    [SerializeField] UnityEngine.UI.Image dissolveImage;
    [SerializeField] Transform verticalImage;

    private System.Action endLoadAction;
    public System.Action StartLoadAction;           // 씬 전환 할때마다 실행

    public System.Action RestartAction;

    private bool isLoading;
    private ETransition type;

    protected override void AwakeInstance()
    {
        isLoading = false;
    }

    protected override void DestroyInstance() { }

    /// <summary>
    /// * 씬에 따라서 tween을 다르게 진행할지
    /// </summary>
    public void MoveScene(EScene scene, ETransition type, System.Action loadAction = null)
    {
        if (isLoading)
            return;

        this.type = type;

        isLoading = true;
        this.endLoadAction = loadAction;

        StartLoadAction?.Invoke();
        StartLoadAction = null;

        switch (type)
        {
            case ETransition.Dissolve:
                dissolveImage.DOColor(Color.black, 0.5f).OnComplete(() =>
                {
                    StartCoroutine(LoadScene((int)scene));
                });
                break;
            case ETransition.Vertical:
                verticalImage.DOLocalMoveY(120f, 0.15f).OnComplete(() =>
                {
                    StartCoroutine(LoadScene((int)scene));
                });
                break;
        }
    }

    IEnumerator LoadScene(int idx)
    {
        yield return null;

        AsyncOperation op = SceneManager.LoadSceneAsync(idx);
        op.allowSceneActivation = false;
        while (!op.isDone)
        {
            yield return null;
            if (op.progress < 0.9f)
            {
                //this.load.text = "Loading... " + Mathf.RoundToInt(op.progress * 100) + "%";
            }
            else
            {
                op.allowSceneActivation = true;
                if (endLoadAction != null)
                    endLoadAction?.Invoke();
            }
        }

        switch (type)
        {
            case ETransition.Dissolve:
                dissolveImage.DOColor(Color.clear, 0.3f);
                break;
            case ETransition.Vertical:
                verticalImage.DOLocalMoveY(-6080f, 0.15f).OnComplete(() =>
                {
                    verticalImage.localPosition = Vector3.up * 6080f;
                });
                break;
        }

        isLoading = false;

        if(AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySceneBGM((EScene)idx);
        }
    }

    public void Restart()
    {
        RestartAction?.Invoke();
        type = ETransition.Dissolve;
        dissolveImage.DOColor(Color.black, 0.5f).OnComplete(() =>
        {
            RestartAction = null;
            StartCoroutine(LoadScene(0));
        });
    }
}

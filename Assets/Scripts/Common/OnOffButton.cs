using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// interactive 말고 Animation 사용하기 위해 작업
/// </summary>
[RequireComponent(typeof(Button))]
public class OnOffButton : MonoBehaviour
{
    [SerializeField] Sprite onSprite;
    [SerializeField] Sprite offSprite;
    [SerializeField] bool bPlayOffSound = true;

    TextMeshProUGUI text;
    Button button;
    // Anima

    private bool isActive;

    private System.Action onAction;
    private System.Action offAction;

    private void Awake()
    {
        button = GetComponent<Button>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        button.onClick.AddListener(OnClick);
    }

    public void ClickButton(System.Action onAction, System.Action offAction = null)
    {
        this.onAction = onAction;
        this.offAction = offAction;
    }

    public void SetButton(bool isActive)
    {
        if(button == null)
            button = GetComponent<Button>();

        this.isActive = isActive;
        button.image.sprite = isActive ? onSprite : offSprite;
    }

    public void SetText(string text)
    {
        if (this.text == null)
            this.text = GetComponentInChildren<TextMeshProUGUI>();
        this.text.text = text;
    }

    public void OnClick()
    {
        if(isActive)
        {
            AudioManager.Instance.PlaySFX(ESfx.Touch);
            onAction?.Invoke();
        }
        else
        {
            if(bPlayOffSound)
            {
                AudioManager.Instance.PlaySFX(ESfx.Disable);
            }
            else 
            {
                AudioManager.Instance.PlaySFX(ESfx.Touch);
            }
            offAction?.Invoke();
        }
    }
}

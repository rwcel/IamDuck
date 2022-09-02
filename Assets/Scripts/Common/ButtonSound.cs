using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 오브젝트 안에 버튼 있어야함. Require은 따로 안씀
public class ButtonSound : MonoBehaviour
{
    [SerializeField] ESfx sound;
    AudioManager audioManager;

    Button button;

    private void Start()
    {
        audioManager = AudioManager.Instance;
        button = GetComponent<Button>();

        button.onClick.AddListener(() => audioManager.PlaySFX(sound));
    }
}

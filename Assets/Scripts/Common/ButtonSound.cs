using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ������Ʈ �ȿ� ��ư �־����. Require�� ���� �Ⱦ�
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

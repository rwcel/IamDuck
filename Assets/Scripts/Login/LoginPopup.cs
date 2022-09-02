using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoginPopup : MonoBehaviour
{
    // type struct
    [System.Serializable]
    private struct FPopupData
    {
        public ELoginPopup type;
        //  public string table;                // Common
        public string titleEntry;
        public string contentEntry;
        public string leftButtonEntry;
        public string rightButtonEntry;
        public UnityAction leftAction;
        public UnityAction rightAction;
    }

    [SerializeField] FPopupData[] popupDatas;

    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI contentText;
    [SerializeField] TextMeshProUGUI leftButtonText;
    [SerializeField] TextMeshProUGUI rightButtonText;

    [SerializeField] Button leftButton;
    [SerializeField] Button rightButton;

    
    public void Openpopup(ELoginPopup type)
    {

    }

}

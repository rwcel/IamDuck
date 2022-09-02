using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class MailUI : PopupUI
{
    [SerializeField] Transform listParent;
    [SerializeField] GameObject elementPrefab;
    [SerializeField] GameObject nomailObj;

    private List<MailElement> mailElements = new List<MailElement>();         // 운영 + 테이블

    private readonly ReactiveCollection<MailElement> mailRC = new ReactiveCollection<MailElement>();
    public System.IObservable<CollectionReplaceEvent<MailElement>> MailObservable => mailRC.ObserveReplace();
    // public List<MailElement> Mails => mailRC.ToList();


    // 한번만 실행하기
    protected override void Awake()
    {
        base.Awake();

        BackendManager server = BackendManager.Instance;

        // Debug.Log($"총 메일 개수 : {server.MailList.Count}");
        foreach (var mail in server.MailList)
        {
            var element = Instantiate(elementPrefab, listParent).GetComponent<MailElement>();
            element.InitializeWithData(mail);
            element.OnRecv += () =>
            {
                mailElements.Remove(element);
                server.ReceiveMail(mail);
                UpdateData();
            };
            mailElements.Add(element);
        }


        // notifyRP.Value = mailElements.Count;
    }

    protected override void UpdateData()
    {
        nomailObj.SetActive(mailElements.Count <= 0);
    }
}

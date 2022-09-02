using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// **�����Ͱ� ������ ��쿡�� ����� �� ����
/// �⼮üũ ĳ���� & �����
/// </summary>
public class LastAttendanceElement : AttendanceElement
{
    [BoxGroup("Last")]
    [SerializeField] GameObject firstObj;
    [BoxGroup("Last")]
    [SerializeField] GameObject normalObj;

    public override void InitializeWithData(FDailyCheck data, int today, int count)
    {
        base.InitializeWithData(data, today, count);

        firstObj.SetActive(count <= today);
        normalObj.SetActive(count > today);
    }
}

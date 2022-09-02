using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// **데이터가 고정일 경우에만 사용할 수 있음
/// 출석체크 캐릭터 & 물고기
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

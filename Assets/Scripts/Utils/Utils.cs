using UnityEngine;
using DG.Tweening;

public static class Utils
{
    public static int GetBoolToInt(this bool value)
    {
        return value == true ? 1 : 0;
    }

    public static bool RandomBinary()
    {
        return Random.Range(0f, 1f) < 0.5f;
    }

    public static string CommaThousands(this int value)
    {
        return string.Format("{0:#,0}", value);
    }

    public static int RandomNum(int maxNum) => Random.Range(0, maxNum);

    public static void ButtonCurve(this Transform tr, FTween tween)
    {
        tr.DOScale(tween.value, tween.time)
            .SetEase(tween.curve);
    }

    public static string MailRemainTime(this System.TimeSpan time)
    {
        //if (time.Days > 0)
        //{
        //    return time.Days;
        //}
        //else
        //{
        //    return time.Hours;
        //}

        if (time.Days > 0)
        {
            return string.Format("{0}{1}", time.Days, "d");
        }
        else
        {
            return string.Format("{0}{1}", time.Hours, "h");
        }
    }

    // 서수 구하기
    public static string OrdinalNumber(this int value)
    {
        // 순위권 밖
        //if (value <= 0 || value > 100)
        if (value <= 0)
            return "-";

        if(PlayerPrefs.GetString(Values.Prefs_Locale) == ELanguage.en.ToString())
        {
            if ((value % 10 == 1) && (value != 11))
                return $"{value}st";
            else if ((value % 10 == 2) && (value != 12))
                return $"{value}nd";
            else if ((value % 10 == 3) && (value != 13))
                return $"{value}rd";
            else
                return $"{value}th";
        }

        return $"{value}등";
    }

    public static string ValueToTime(this int value)
    {
        int m = value / 60;
        int s = value % 60;

        return string.Format("{0}:{1:00}", m, s);
    }
}
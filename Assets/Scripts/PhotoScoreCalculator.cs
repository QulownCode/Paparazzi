using UnityEngine;

public static class PhotoScoreCalculator
{
    public static float CalculateBaseScore(float distance, float framing, float zoom)
    {
        float baseScore = distance + framing + zoom;
        return Mathf.Clamp(baseScore, 0f, 100f);
    }

    public static bool CanOverflow(float baseScore)
    {
        return baseScore >= 94f;
    }

    public static float CalculateFinalScore(float baseScore, float poseBonus)
    {
        bool overflow = CanOverflow(baseScore);

        if (overflow)
        {
            return baseScore + poseBonus;
        }
        else
        {
            return Mathf.Min(100f, baseScore + poseBonus);
        }
    }
}
using UnityEngine;
using TMPro;

public class ScoreDebugUI : MonoBehaviour
{
    public TextMeshProUGUI text;

    public void UpdateDebug(
        float distance,
        float framing,
        float zoom,
        float baseScore,
        float finalScore,
        bool overflow)
    {
        text.text =
            $"DIST: {distance:0.0}\n" +
            $"FRAME: {framing:0.0}\n" +
            $"ZOOM: {zoom:0.0}\n" +
            $"BASE: {baseScore:0}\n" +
            $"FINAL: {finalScore:0}\n" +
            $"OVERFLOW: {(overflow ? "YES" : "NO")}";
    }
}
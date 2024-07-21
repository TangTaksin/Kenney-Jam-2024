using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Score : MonoBehaviour
{
    float totalScore;
    bool bondPoint;
    int comboCount;

    public TextMeshProUGUI score_combo_txt, performance_txt;


    public int good_point = 10;
    public int late_point = 5;
    public int early_point = 5;

    public float bondDownRate;

    private void Start()
    {
        ChartPlayer.OnEarly += OnEarly;
        ChartPlayer.OnGood += OnGood;
        ChartPlayer.OnLate += OnLate;
        ChartPlayer.OnMiss += OnMiss;
    }

    private void OnDestroy()
    {
        ChartPlayer.OnEarly -= OnEarly;
        ChartPlayer.OnGood -= OnGood;
        ChartPlayer.OnLate -= OnLate;
        ChartPlayer.OnMiss -= OnMiss;
    }

    void OnEarly()
    {
        totalScore += early_point;
        comboCount += 1;
        UpdateDisplay();
        performance_txt.text = "EARLY +" + early_point;
    }

    void OnGood()
    {
        totalScore += good_point;
        comboCount += 1;
        UpdateDisplay();
        performance_txt.text = "GOOD! +" + good_point;
    }

    void OnLate()
    {
        totalScore += late_point;
        comboCount += 1;
        UpdateDisplay();
        performance_txt.text = "LATE +" + late_point;
    }

    void OnMiss()
    {
        comboCount = 0;
        UpdateDisplay();
        performance_txt.text = "MISS - COMBO BREAK";
    }

    void UpdateDisplay()
    {
        score_combo_txt.text = string.Format("{1} : COMBO\n{0} : SCORE", totalScore, comboCount);
    }
}

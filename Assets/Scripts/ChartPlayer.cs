using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChartPlayer : MonoBehaviour
{
    public Chart currentChart;
    bool isPlayerTurn;

    public NoteSkin guideSkin, playerSkin;

    public float lastbeat;
    public static float beatCount = 0;
    public static float barCount = 1;

    int phrase_num;
    int beat_array_num = 0;

    public Transform _promptRect;
    public Slider Line;
    Transform Runner;

    List<GameObject> promtsImg_list = new List<GameObject>();

    public TextMeshProUGUI TickCounterTxt;

    public void Start()
    {
        Runner = Line.handleRect;
    }

    private void Update()
    {
        BeatUpdate();
        LineUpdate();
    }

    void BeatUpdate()
    {
        if (Conductor.songposition > lastbeat + Conductor.crotchet)
        {
            beatCount++;
            lastbeat += Conductor.crotchet;

            if (beatCount > 4 * barCount)
            {
                barCount++;
            }
        }

        if (beatCount > 15)
        {
            NextPhrase();
        }

        TickCounterTxt.text = string.Format("beats: {0}\nbars: {1}", beatCount, barCount);
    }

    public float lineProg = 0;

    void LineUpdate()
    {
        lineProg += (Conductor.bpm / 60) * Time.deltaTime;
        Line.value = (lineProg / 16);
        ReadChart(lineProg);
    }

    public void StartPlaying()
    {
        Conductor.OnPlay(currentChart.clip, currentChart.bpm);
    }

    void ReadChart(float beat_num)
    {
        if (phrase_num > currentChart.phrases.Length)
            return;

        isPlayerTurn = currentChart.phrases[(int)phrase_num].playerTurn;

        if (!isPlayerTurn)
            PlaceBeat(beat_num);
    }
    
    void PlaceBeat(float beat_num)
    {
        if (currentChart.phrases[phrase_num].Notes.Length == 0)
            return;

        var cur_note = currentChart.phrases[phrase_num].Notes[beat_array_num];

        if (beat_num >= cur_note.onBeat)
        {
            GameObject inst_note = null;

            if (promtsImg_list.Count < beat_array_num + 1)
            {
                inst_note = new GameObject();
                var note_image = inst_note.AddComponent<Image>();

                note_image.sprite = playerSkin.GetDirectionSprite(cur_note.input);
                note_image.color = playerSkin.color;

                inst_note.transform.SetParent(_promptRect);
                inst_note.transform.position = Runner.position;

                promtsImg_list.Add(inst_note);
            }
            else
            {
                inst_note = promtsImg_list[beat_array_num].gameObject;
                var note_image = GetComponent<Image>();

                note_image.sprite = playerSkin.GetDirectionSprite(cur_note.input);
                note_image.color = playerSkin.color;

                inst_note.gameObject.SetActive(true);
            }

            //
            beat_array_num++;
        }
    }

    void NextPhrase()
    {
        lineProg = 0;
        beatCount = 0;
        barCount = 1;

        foreach(var p in promtsImg_list)
        {
            p.gameObject.SetActive(false);
        }

        if (phrase_num++ > currentChart.phrases.Length)
        {
            phrase_num = currentChart.phrases.Length;
            return;
        }
    }
}

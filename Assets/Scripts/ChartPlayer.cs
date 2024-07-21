using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ChartPlayer : MonoBehaviour
{
    public Chart currentChart;
    bool isPlayerTurn;

    public NoteSkin guideSkin, playerSkin;
    NoteSkin currentSkin;

    public float lastbeat;
    public static float beatCount = 0;
    public static float barCount = 1;

    int phrase_num;
    int beat_array_num = 0;
    Note cur_note;

    [Header("Input Window")]
    public float inputWindow_valid = .5f;
    public float inputWindow_good = .25f;
    [Space]

    public Transform _promptRect;
    public Slider Line;
    Image Runner;

    List<GameObject> promtsImg_list = new List<GameObject>();

    public TextMeshProUGUI TickCounterTxt;

    public delegate void TimingEvent();
    public static TimingEvent OnEarly, OnLate, OnGood, OnMiss;

    public void Start()
    {
        Runner = Line.handleRect.GetComponent<Image>();
    }

    private void Update()
    {
        if (!Conductor.isPlaying)
            return;

        BeatUpdate();
        LineUpdate();
       

        DetectPlayerInput();
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

        if (beatCount > currentChart.phrases[phrase_num].endAfterBeat)
        {
            NextPhrase();
        }

        TickCounterTxt.text = string.Format("beats: {0}\nbars: {1}", beatCount, barCount);
    }

    public float lineProg = 0;

    void LineUpdate()
    { 
        ReadChart(lineProg);
        lineProg += (Conductor.bpm / 60) * Time.deltaTime;
        Line.value = (lineProg / 16);
        
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

        switch(isPlayerTurn)
        {
            case false:
                currentSkin = guideSkin;
                Runner.color = guideSkin.color;
                break;
            case true:
                currentSkin = playerSkin;
                Runner.color = playerSkin.color;
                break;
        }

        CheckNote(beat_num);
    }

    void CheckNote(float beat_num)
    {
        if (currentChart.phrases[phrase_num].Notes.Length == 0
            || beat_array_num > currentChart.phrases[phrase_num].Notes.Length - 1)
        {
            return;
        }
        else
            cur_note = currentChart.phrases[phrase_num].Notes[beat_array_num];

        if (beat_num >= cur_note.onBeat)
        {
            if (!isPlayerTurn)
            {
                PlaceNote(cur_note.input);
            }
            else if (Mathf.Abs(lineProg - cur_note.onBeat) < inputWindow_valid)
            {
                TimingDisplay(Timing.miss);
            }

            beat_array_num++;
        }
    }

    void PlaceNote(input _input)
    {
        GameObject inst_note = null;

        if (promtsImg_list.Count < beat_array_num + 1)
        {
            inst_note = new GameObject();
            var note_image = inst_note.AddComponent<Image>();
            print(note_image);

            note_image.sprite = currentSkin.GetDirectionSprite(_input);
            note_image.color = currentSkin.color;

            inst_note.transform.SetParent(_promptRect);
            inst_note.transform.position = Runner.rectTransform.position;

            promtsImg_list.Add(inst_note);
        }
        else
        {
            inst_note = promtsImg_list[beat_array_num].gameObject;
            var note_image = inst_note.GetComponent<Image>();

            note_image.sprite = currentSkin.GetDirectionSprite(_input);
            note_image.color = currentSkin.color;

            inst_note.transform.position = Runner.rectTransform.position;
            inst_note.gameObject.SetActive(true);
        }

    }

    void DetectPlayerInput()
    {
        if (!isPlayerTurn)
            return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            CheckPlayerInput(input.Left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            CheckPlayerInput(input.Right);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            CheckPlayerInput(input.Top);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            CheckPlayerInput(input.Bottom);
        }
    }

    void CheckPlayerInput(input _input)
    {
        var timeOnInput = lineProg;
        print("input for note :" +cur_note);
        
        beat_array_num++;
        PlaceNote(_input);

        if (cur_note == null)
        {
            TimingDisplay(Timing.miss);
            return;
        }

        var runo_distance = timeOnInput - cur_note.onBeat;

        if (_input == cur_note.input )
        {

            if ((Mathf.Abs(runo_distance) < inputWindow_good))
            {
                TimingDisplay(Timing.great);
            }
            else if (runo_distance < -inputWindow_good)
            {
                TimingDisplay(Timing.early);
            }
            else if (runo_distance > inputWindow_good)
            {
                TimingDisplay(Timing.late);
            }
        }
        else
        {
            TimingDisplay(Timing.miss);
        }
    }

    enum Timing { miss, early, great, late }

    void TimingDisplay(Timing time)
    {
        switch (time)
        {
            case Timing.miss:
                OnMiss?.Invoke();
                break;

            case Timing.early:
                OnEarly?.Invoke();
                break;
            case Timing.great:
                OnGood?.Invoke();
                break;
            case Timing.late:
                OnLate?.Invoke();
                break;
        }
    }

    void NextPhrase()
    {
        lineProg = 0;
        beatCount = 0;
        barCount = 1;

        beat_array_num = 0;
        cur_note = null;

        if (currentChart.phrases[phrase_num].clearNode)
        {
            foreach (var p in promtsImg_list)
            {
                p.gameObject.SetActive(false);
            }
        }

        if (phrase_num++ >= currentChart.phrases.Length-1)
        {
            SceneManager.LoadScene("EndScene");
        }
    }
}

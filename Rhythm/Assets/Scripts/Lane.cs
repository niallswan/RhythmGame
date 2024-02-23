using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Lane : MonoBehaviour
{
    [SerializeField] private UnityEvent _trigger; 
    public GameObject hitEffect, goodEffect, perfectEffect, missEffect;
    public Melanchall.DryWetMidi.MusicTheory.NoteName noteRestriction;
    public KeyCode input;
    public GameObject notePrefab;
    List<NoteObject> notes = new List<NoteObject>();
    public List<double> timeStamps = new List<double>();

    int spawnIndex = 0;
    int inputIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void SetTimeStamps(Melanchall.DryWetMidi.Interaction.Note[] array)
    {
        foreach (var note in array)
        {
            if (note.NoteName == noteRestriction)
            {
                var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, SongManager.midiFile.GetTempoMap());
                timeStamps.Add((double)metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds + (double)metricTimeSpan.Milliseconds / 1000f);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (spawnIndex < timeStamps.Count)
        {
            if (SongManager.GetAudioSourceTime() >= timeStamps[spawnIndex] - SongManager.Instance.noteTime)
            {
                var note = Instantiate(notePrefab, transform);
                notes.Add(note.GetComponent<NoteObject>());
                note.GetComponent<NoteObject>().assignedTime = (float)timeStamps[spawnIndex];
                spawnIndex++;
            }
        }

        if (inputIndex < timeStamps.Count)
        {
            double timeStamp = timeStamps[inputIndex];
            double marginOfError = SongManager.Instance.marginOfError;
            double marginOfErrorGood = SongManager.Instance.marginOfErrorGood;
            double marginOfErrorPerfect = SongManager.Instance.marginOfErrorPerfect;
            double audioTime = SongManager.GetAudioSourceTime() - (SongManager.Instance.inputDelayInMilliseconds / 1000.0);

            if (Input.GetKeyDown(input))
            {
                _trigger.Invoke();
                
                if (Math.Abs(audioTime - timeStamp) < marginOfErrorPerfect)
                {
                    Hit("perfect");
                    print($"Perfect hit on {inputIndex} note");
                    Instantiate(perfectEffect, transform.position, perfectEffect.transform.rotation);
                    Destroy(notes[inputIndex].gameObject);
                    inputIndex++;
                }else if(Math.Abs(audioTime - timeStamp) < marginOfErrorGood){
                    Hit("good");
                    print($"Good hit on {inputIndex} note");
                    Instantiate(goodEffect, transform.position, goodEffect.transform.rotation);
                    Destroy(notes[inputIndex].gameObject);
                    inputIndex++;
                }else if(Math.Abs(audioTime - timeStamp) < marginOfError){
                    Hit(null);
                    print($"Hit on {inputIndex} note");
                    Instantiate(hitEffect, transform.position, hitEffect.transform.rotation);
                    Destroy(notes[inputIndex].gameObject);
                    inputIndex++;
                }else{
                    print($"Hit inaccurate on {inputIndex} note with {Math.Abs(audioTime - timeStamp)} delay");
                    Instantiate(missEffect, transform.position, missEffect.transform.rotation);
                }
            }
            if (timeStamp + marginOfError <= audioTime)
            {
                Miss();
                print($"Missed {inputIndex} note");
                inputIndex++;
            }
        }       
    
    }
    private void Hit(string hit)
    {
        if(hit == "perfect"){
            GameManager.instance.PerfectHit();
        }else if(hit == "good"){
            GameManager.instance.GoodHit();
        }else{
            GameManager.instance.NormalHit();
        }
        
    }
    private void Miss()
    {
        GameManager.instance.NoteMissed();
    }
}
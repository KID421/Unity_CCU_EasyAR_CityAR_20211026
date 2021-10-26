using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class DelayRun : MonoBehaviour {
    bool IsRun;
    public float DelayTime;
    public UnityEvent DelayRunEvent;

    public float CountTimenow;
    void OnEnable() {
        IsRun = true;
        CountTimenow = DelayTime;
        //Invoke("RunDelayRun", DelayTime);
    }

    public void RunDelayRun() {
        DelayRunEvent.Invoke();
    }

    void Update() {
        if (IsRun) {
            if (CountTimenow <= 0) {
                RunDelayRun();
                IsRun = false;
            }
            else {
                CountTimenow -= Time.deltaTime;
            }
        }
    }

}

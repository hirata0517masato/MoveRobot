using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 
using System;

public class playtime : MonoBehaviour {

	float start_time_s,play_time;
	
	// Use this for initialization
	void Start () {
		var components = this.gameObject.GetComponentsInChildren<Text>();
    	components[0].text = "00:00:00";
		start_time_s = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if(obstacle.flagnum > 0){
			play_time = Time.time - start_time_s;
			var components = this.gameObject.GetComponentsInChildren<Text>();
			components[0].text = make_time(play_time);
		}
	}

	string make_time(float t){
        int h,m,s;
        h = (int)t/3600;
        t %= 3600;

        m = (int)t/60;
        t %= 60;

        s = (int)t;

        return String.Format("{0:00}", h) + ":"+ String.Format("{0:00}", m) +":"+ String.Format("{0:00}", s);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 
using System;

public class result : MonoBehaviour {

	// Use this for initialization
	void Start () {
		var components = this.gameObject.GetComponentsInChildren<Text>();
    	components[0].text = "";
	}
	
	// Update is called once per frame
	void Update () {
		if(obstacle.flagnum <= 0){
			var components = this.gameObject.GetComponentsInChildren<Text>();
			components[0].text = "やったね！";
		}
	}
}

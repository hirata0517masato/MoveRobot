using System; 
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class result : MonoBehaviour {

	bool db_update = false;

	// Use this for initialization
	void Start () {
		var components = this.gameObject.GetComponentsInChildren<Text>();
    	components[0].text = "";
	}
	
	[DllImport("__Internal")]
  	public static extern string DbUpdateJS();

	// Update is called once per frame
	void Update () {
		if(obstacle.flagnum <= 0){
			var components = this.gameObject.GetComponentsInChildren<Text>();
			components[0].text = "やったね！";
			
			if(db_update == false){
				db_update = true;
			 	if(Application.platform == RuntimePlatform.WebGLPlayer) {
					//db update
					string ret = DbUpdateJS();
				}
			}
		}
	}
}

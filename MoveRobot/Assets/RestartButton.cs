using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class RestartButton : MonoBehaviour {

	void Start () {
    	
		//transform.position = new Vector3(70, Screen.height-15, 0f);

		var components = this.gameObject.GetComponentsInChildren<Text>();
    	components[0].text = "はじめから";
		
	}

	/* void Update () {
		var components = this.gameObject.GetComponentsInChildren<Text>();
    	components[0].text = "はじめから";
	}*/

	/// ボタンをクリックした時の処理
  	public void OnClick() {
    	//Application.LoadLevel( Application.loadedLevelName );
    	SceneManager.LoadScene(SceneManager.GetActiveScene().name);

  	}
}



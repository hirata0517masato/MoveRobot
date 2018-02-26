using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement; 

public class RestartButton : MonoBehaviour {

	void Start () {
    	
		transform.position = new Vector3(90, Screen.height-25, 0f);
	}

	/// ボタンをクリックした時の処理
  	public void OnClick() {
    	//Application.LoadLevel( Application.loadedLevelName );
    	SceneManager.LoadScene(SceneManager.GetActiveScene().name);
  	}
}



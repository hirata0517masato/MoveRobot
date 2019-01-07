using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class obstacle : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
    	
        for(int i = 0; i < 15; i++){
            Vector3 CreatePoint = new Vector3(Random.Range(-14,14), 0, Random.Range(-14,14)); //位置

            var obj = Instantiate(GameObject.Find("box"),CreatePoint,Quaternion.identity) as GameObject;
            obj.name = GameObject.Find("box").name + i;

            GameObject.Find("box" + i).transform.Rotate(new Vector3(0, Random.Range(0,45), 0));//回転

            //GameObject.Find("box" + i).transform.localScale = new Vector3(Random.Range(1,3), 1, Random.Range(1,3));//サイズ 当たり判定がおかしくなる
        }   
    }
   

	// Update is called once per frame
	void Update () {
		
    }
    
}

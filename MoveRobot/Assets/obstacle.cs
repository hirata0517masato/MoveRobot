//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

using System; 
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Runtime.InteropServices;

public class obstacle : MonoBehaviour {

    int num = 0;
    // Use this for initialization
    void Start()
    {
    	List<string> texts = new List<string>(ReadFileObstacle().Split('\n'));
        for(int i = 0; i < texts.Count; i++){
            List<string> pos = new List<string>(texts[i].Split(','));
            Make("box",float.Parse(pos[0]), float.Parse(pos[1]) );
        }

        /*for(int i = 0; i < 15; i++){
            Make("box",UnityEngine.Random.Range(-14,14), UnityEngine.Random.Range(-14,14));
        } */  
    }
   

    void Make(string name,float x,float z){
        Vector3 CreatePoint = new Vector3(x, 0, z); //位置

        var obj = Instantiate(GameObject.Find(name),CreatePoint,Quaternion.identity) as GameObject;
        obj.name = GameObject.Find(name).name + num;

        //GameObject.Find(name + num).transform.Rotate(new Vector3(0, UnityEngine.Random.Range(0,45), 0));//回転

        //GameObject.Find(name + num).transform.localScale = new Vector3(Random.Range(1,3), 1, Random.Range(1,3));//サイズ 当たり判定がおかしくなる
        num++;
    }
	
    string ReadFileObstacle(){

		string path = "",text = "";

		/*if(Application.platform == RuntimePlatform.WebGLPlayer) {
 			WebGLInput.captureAllKeyboardInput = false;
			text = ReadJS();
		}else{*/
        	FileInfo fi;
			if (Application.platform == RuntimePlatform.OSXEditor
					|| Application.platform == RuntimePlatform.WindowsEditor){
				fi = new FileInfo(Application.dataPath + "/" + "ReadPathOB.txt");
			}else{
				fi = new FileInfo("ReadPathOB.txt");
			}
			
			try{
				// ??s????????
				using (StreamReader sr = new StreamReader(fi.OpenRead(), Encoding.UTF8))
				{
					path = sr.ReadToEnd();
				}
			}catch (Exception e){
				// ???s?R?[?h
				path += SetDefaultText();
			}

			if (Application.platform == RuntimePlatform.OSXEditor
					|| Application.platform == RuntimePlatform.WindowsEditor){
			
				fi = new FileInfo(Application.dataPath + "/" + path);
			}else{
				fi = new FileInfo(path);
			}
			
			try{
				// ??s????????
				using (StreamReader sr = new StreamReader(fi.OpenRead(), Encoding.UTF8))
				{
					text = sr.ReadToEnd();
				}
			}catch (Exception e){
				// ???s?R?[?h
				text += SetDefaultText();
			}
		//}

        return text; 
    }

    static string SetDefaultText(){
        return "C#??\n";
    }
    
}

using System; 
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Runtime.InteropServices;

class Read {
	static List<string> NonFree(string text){//?\?[?X?R?[?h???`
		text = text.Replace("\t", " ");
		text = CurlyBracket(text);//???????????u????
		
		List<string> list = new List<string>(text.Split('\n'));
		
		for(int i = 0; i < list.Count; i++){
			if(list[i].Trim().Length <= 0){
				list.RemoveAt(i);
				i--;
				continue;
			}
			
			if(list[i].IndexOf("#include") != -1) list[i] = "";//include???????
			
			list[i] = list[i].Replace(","," , ");
			list[i] = list[i].Replace("="," = ").Replace("<"," < ").Replace(">"," > ").Replace("!"," ! ");
			list[i] = list[i].Replace("&"," & ").Replace("|"," | ");
			list[i] = list[i].Replace("+"," + ").Replace("-"," - ").Replace("*"," * ").Replace("/"," / ").Replace("%"," % ").Replace(";"," ; ");
        	list[i] = Regex.Replace(list[i].Replace(")", " ) ").Replace("(", " ( ").Replace("{", " {").Replace("}", "} ").Trim()," +", " ");
			list[i] = list[i].Replace("= =","==").Replace("< =","<=").Replace("> = ",">=").Replace("! =","!=");
			list[i] = list[i].Replace("& &","&&").Replace("| |","||");

			//list[i] = list[i].Replace("( -","( 0 -").Replace(" = -"," = 0 -");

			if(list[i] == "void loop ( ) {")list[i] = "while ( 1 ) {";//loop?????while(1)???X
        }
        list = ForToWhile(list);
        list = FormulaTransform(list);
        return list;
	}

	static string CurlyBracket(string text){//???????????u????
		text = text.Replace(")", " ) ").Replace("(", " ( ").Replace("{", " { ").Replace("}", " } ");
		string[] temp = {"if ","else", "while ", "for " };
		
		foreach (string s in temp){//?J?n??????
			int n = 0;
			while(true){
				int pos = text.IndexOf(s,n);	
				if(pos == -1)break;
				
				int CBpos = text.IndexOf("{",pos);

				string target = text.Substring(pos, CBpos - pos + 1);
				target = target.Replace('\n',' ');

				text = text.Remove(pos, CBpos - pos + 1);
				text = text.Insert(pos, target);

				n = pos+1;
			}
		}

		int nn = text.Length-1;
		while(true){//?????????
			int pos = text.LastIndexOf("else",nn);	
			if(pos == -1)break;
				
			int CBpos = text.LastIndexOf("}",pos);

			string target = text.Substring(CBpos, pos - CBpos + 1);
			target = target.Replace('\n',' ');

			text = text.Remove(CBpos, pos - CBpos + 1);
			text = text.Insert(CBpos, target);

			nn = CBpos-1;
		}

		return text;
	}
	
	static List<string> ForToWhile(List<string> list){//for -> while
		for(int i = 0; i < list.Count; i++){
        	string[] words = list[i].Trim().Split(' ');
			if(words[0] == "for"){
				string[] for_words = list[i].Trim().Split(new char [] {'(', ';' , ')'});
				
				list.Insert(i, for_words[1] + " ;");//????????
				list.Insert(i, "{");
				i+=2;
				list[i] = "while (" + for_words[2] + ") {"; //???

				int k = i+1,stack = 1;
				do{
					string[] for2_words = list[k].Trim().Split(' ');
					foreach(String s in for2_words){
						if(s == "{"){
							stack++;
						}else if(s == "}"){
							stack--;
							if(stack <= 0)break;
						}
					}	
					k++;
				}while(stack > 0);
				list.Insert(k-1, for_words[3] + " ;");//??????	
				if(list.Count < k+2)list.Add("}");
				else list.Insert(k+1, "}");
			}
        }
		return list;
	}

	static List<string> FormulaTransform(List<string> list){//?C???N???????g??????
		for(int i = 0; i < list.Count; i++){
        	string[] words = list[i].Trim().Split(' ');
			if(words.Length >= 3 && words[0] != "delay" && words[0] != "motor" && words[0] != "int" && words[0] != "double" && words[0] != "print" && words[0] != "if" && words[0] != "while" && words[0] != "}"){
				if(words[1] != "="){
					if(words[1] == words[2]){//??u
						list[i] = words[0] + " = " + words[0] + " " +  words[1] + " 1 ;";
					}else if(words[0] == words[1]){//?O?u
						list[i] = words[2] + " = " + words[2] + " " +  words[1] + " 1 ;";
					}else if(words[2] == "="){//+=???
						list[i] = words[0] + " = " + words[0] + " " +  words[1] + " ( ";
						for(int j = 3; j < words.Length-1; j++){
							list[i] += words[j]; 
						}
						list[i] += " ) ;";
					}
				}
			}
		}
		return list;
	}
	
	static string del_comment(string text){//?e?L?X?g????C???C?N??R?????g???

		while(true){
			int start = text.IndexOf("/*");
			if(start == -1)break;
			int end = text.IndexOf("*/");
			if(end == -1)break;
			text = text.Remove(start, end - start +2);
		}
		return text;
	}

	[DllImport("__Internal")]
  	public static extern string ReadJS();

	// ????????
    public static string[] ReadFile(){
        // FileReadTest.txt?t?@?C?????????

		string path = "",text = "";

		if(Application.platform == RuntimePlatform.WebGLPlayer) {
 			//WebGLInput.captureAllKeyboardInput = false;
			#if UNITY_WEBGL && !(UNITY_EDITOR)//！UNITY_EDITOR && UNITY_WEBGL
				WebGLInput.captureAllKeyboardInput = false; 
			#endif
			text = ReadJS();
			//text = "motor(100,100);";
		}else{
        	FileInfo fi;
			if (Application.platform == RuntimePlatform.OSXEditor
					|| Application.platform == RuntimePlatform.WindowsEditor){
				fi = new FileInfo(Application.dataPath + "/" + "ReadPath.txt");
			}else{
				fi = new FileInfo("ReadPath.txt");
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
		}

        text = del_comment(text);
        List<string> list = NonFree(text);   
        return list.ToArray(); 
    }

    // ???s?R?[?h????
    static string SetDefaultText(){
        return "C#??\n";
    }
}

public class Read_WebGL : MonoBehaviour {
	public string result = "";

	public string read_webgl() {
        StartCoroutine(textLoad() );
        return result;
    }
    
	IEnumerator textLoad() { 
    	//string filepath = Application.streamingAssetsPath + "/code/code.ino";
    	string filepath = "/code/code.ino";
    	if (filepath.Contains ("://") || filepath.Contains (":///"))
    	{
        	WWW www = new WWW (filepath);
        	yield return www;
        	result = www.text;
        	//print (result);
    	} else {
        	result = File.ReadAllText (filepath);
        	//print (result);
    	}
	}
}
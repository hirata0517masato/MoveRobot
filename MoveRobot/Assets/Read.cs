using System; 
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

class Read {
	static List<string> NonFree(string text){//ソースコード整形
		text = text.Replace("\t", " ");
		text = CurlyBracket(text);//中かっこの位置を調整
		
		List<string> list = new List<string>(text.Split('\n'));
		
		for(int i = 0; i < list.Count; i++){
			if(list[i].Trim().Length <= 0){
				list.RemoveAt(i);
				i--;
				continue;
			}
			
			if(list[i].IndexOf("#include") != -1) list[i] = "";//include文は消去
			
			list[i] = list[i].Replace(","," , ");
			list[i] = list[i].Replace("="," = ").Replace("<"," < ").Replace(">"," > ").Replace("!"," ! ");
			list[i] = list[i].Replace("&"," & ").Replace("|"," | ");
			list[i] = list[i].Replace("+"," + ").Replace("-"," - ").Replace("*"," * ").Replace("/"," / ").Replace("%"," % ").Replace(";"," ; ");
        	list[i] = Regex.Replace(list[i].Replace(")", " ) ").Replace("(", " ( ").Replace("{", " {").Replace("}", "} ").Trim()," +", " ");
			list[i] = list[i].Replace("= =","==").Replace("< =","<=").Replace("> = ",">=").Replace("! =","!=");
			list[i] = list[i].Replace("& &","&&").Replace("| |","||");

			//list[i] = list[i].Replace("( -","( 0 -").Replace(" = -"," = 0 -");

			if(list[i] == "void loop ( ) {")list[i] = "while ( 1 ) {";//loop関数はwhile(1)に変更
        }
        list = ForToWhile(list);
        list = FormulaTransform(list);
        return list;
	}

	static string CurlyBracket(string text){//中かっこの位置調整
		text = text.Replace(")", " ) ").Replace("(", " ( ").Replace("{", " { ").Replace("}", " } ");
		string[] temp = {"if ","else", "while ", "for " };
		
		foreach (string s in temp){//開始かっこ
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
		while(true){//閉じかっこ
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
				
				list.Insert(i, for_words[1] + " ;");//初期化式
				list.Insert(i, "{");
				i+=2;
				list[i] = "while (" + for_words[2] + ") {"; //条件式

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
				list.Insert(k-1, for_words[3] + " ;");//増加式	
				if(list.Count < k+2)list.Add("}");
				else list.Insert(k+1, "}");
			}
        }
		return list;
	}

	static List<string> FormulaTransform(List<string> list){//インクリメントなどの変換
		for(int i = 0; i < list.Count; i++){
        	string[] words = list[i].Trim().Split(' ');
			if(words.Length >= 3 && words[0] != "motor" && words[0] != "int" && words[0] != "double" && words[0] != "print" && words[0] != "if" && words[0] != "while" && words[0] != "}"){
				if(words[1] != "="){
					if(words[1] == words[2]){//後置
						list[i] = words[0] + " = " + words[0] + " " +  words[1] + " 1 ;";
					}else if(words[0] == words[1]){//前置
						list[i] = words[2] + " = " + words[2] + " " +  words[1] + " 1 ;";
					}else if(words[2] == "="){//+=とか
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
	
	static string del_comment(string text){//テキストからCライクなコメントを削除

		while(true){
			int start = text.IndexOf("/*");
			if(start == -1)break;
			int end = text.IndexOf("*/");
			if(end == -1)break;
			text = text.Remove(start, end - start +2);
		}
		return text;
	}
	
	// 読み込み関数
    public static string[] ReadFile(){
        // FileReadTest.txtファイルを読み込む
        FileInfo fi;
        if (Application.platform == RuntimePlatform.OSXEditor ||Application.platform == RuntimePlatform.WindowsEditor) {
 			fi = new FileInfo(Application.dataPath + "/" + "ReadPath.txt");
		}else{
			fi = new FileInfo("ReadPath.txt");
		}
        string path= "";
        try{
            // 一行毎読み込み
            using (StreamReader sr = new StreamReader(fi.OpenRead(), Encoding.UTF8))
            {
                path = sr.ReadToEnd();
            }
        }catch (Exception e){
            // 改行コード
            path += SetDefaultText();
        }

		if (Application.platform == RuntimePlatform.OSXEditor ||Application.platform == RuntimePlatform.WindowsEditor) {
        	fi = new FileInfo(Application.dataPath + "/" + path);
        }else{
        	fi = new FileInfo(path);
        }
        string text= "";
        try{
            // 一行毎読み込み
            using (StreamReader sr = new StreamReader(fi.OpenRead(), Encoding.UTF8))
            {
                text = sr.ReadToEnd();
            }
        }catch (Exception e){
            // 改行コード
            text += SetDefaultText();
        }
        
        text = del_comment(text);
        List<string> list = NonFree(text);   
        return list.ToArray(); 
    }

    // 改行コード処理
    static string SetDefaultText(){
        return "C#あ\n";
    }
}

using System; 
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

class variable{
	public static Dictionary<string, pair<double,string>> dic = new Dictionary<string, pair<double,string>>();
	
	public static string name(string name, int block){//•Ï”–¼ŒŸõ
		while(block >= 0){
			if(dic.ContainsKey(block.ToString() + name)){
				return block.ToString() + name;
			}
			block--;
		}
		return "";
	}

	public static string formula(int block,string[] words,int n,int length){//®ì¬
		string ret = "";
        for(int j = n; j < length; j++){
        	string var_name = name(words[j],block);
        	
			if(var_name != ""){
				pair<double,string> r = read(var_name);
				if(r.second == "double") ret += r.first.ToString("F10");
				else ret += ((int)(r.first)).ToString();
			}else{
            	ret += words[j];
			}
        }
        return ret;
	}
	
	public static pair<double,string> read(string var_name){//•Ï”‚Ì’l‚ğ•Ô‹p
		return dic[var_name];
	}
	
	public static void sub(string var_name,double num){//•Ï”‚É‘ã“ü
		dic[var_name] = new pair<double,string>(num,dic[var_name].second);
	}

	public static void make(int block,string[] words){//•Ï”‚ÌéŒ¾
		string s = "",name = "";
		bool flag = false;
		for(int j = 1; j<words.Length; j++){
			if(words[j] != ""){
				if(flag == false){
					name = words[j];
					flag = true;
					s = "";
				}else if(words[j] == "," || words[j] == ";"){
					if(s != ""){
						string[] ss = s.Split(' ');
						pair<double,string> tmp = new pair<double,string>(new Parser(formula(block,ss,0,ss.Length)).expr().first,words[0]);
						dic.Add( block.ToString() + name,tmp);
								
					}else dic.Add( block.ToString() + name,new pair<double,string>(0,words[0]));
					s = "";
					flag = false;
				}else{
					if(words[j] != "=")s += words[j] + " ";
				}
			}
		}
	}
	
	public static void del(int block){//•s—v‚È•Ï”‚Ìíœ
		string del = block.ToString();
		var del_list = new List<string>();
				
		foreach (string key in dic.Keys) {
			if(key.IndexOf(del) == 0){
				del_list.Add(key);
			}
        }
        foreach (string key in del_list)dic.Remove(key);
	}	
}
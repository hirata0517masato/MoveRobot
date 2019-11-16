using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;


public class RobotRun : MonoBehaviour {

    float S_speed = 0,R_speed = 0;
    float L_moter = 0, R_moter = 0;
    string[] lines;//テキスト読み込み
    int block = 0,line_num = 0;
    Stack<int> b_stack = new Stack<int>();
    Stack<int> jump_stack = new Stack<int>();
	int touch = 0;//0 = non, 1 = body, 2 = bamper
	float[] distance = new float[4];              // 計測距離 f,b,l,r
	double compass; //方位センサー
	string output = "";
    float fps_clock = 0.017f;
    int delay_time,delay_start_time_ms;
    bool delay_flag = false;
	

    void Awake()
    {
        // PC向けビルドだったらサイズ変更
        if (Application.platform == RuntimePlatform.WindowsPlayer ||
            Application.platform == RuntimePlatform.OSXPlayer ||
            Application.platform == RuntimePlatform.LinuxPlayer ){

                int ScreenWidth = Screen.width;
                int ScreenHeight = ScreenWidth /16 *9 ;

                Screen.SetResolution(ScreenWidth, ScreenHeight, false);
        }
    }
	 // 読み込んだ情報をGUIとして表示 ただし変換後のコード
    void OnGUI()
    {
        if(output.Length != 0)GUI.TextArea(new Rect(Screen.width/4, 5, Screen.width/2, 50), output);
    }
    
    // Use this for initialization
    void Start()
    {
        
    	//DontDestroyOnLoad(transform.gameObject);
    	variable.del(0);//不要な変数の削除
        lines = Read.ReadFile();//テキスト読み込み

        InvokeRepeating("RobotMain", fps_clock, fps_clock);
    }

	void OnCollisionStay(Collision other)//接触中
    {
        if(other.gameObject.tag == "flag"){
            Destroy(other.gameObject);
            obstacle.flagnum--;
            return;
        }
    	//if (other.gameObject.tag == "wall" || true){
        	//Destroy(other.gameObject);  	

        	//はじめの衝突位置
    		float other_x = other.contacts[0].point.x;
			float other_z = other.contacts[0].point.z;
			
    		Vector3 robot = this.transform.position;
    		//output += "(" + robot.x + "," + robot.z + ")";

			double angle = -(Math.Atan2((robot.x-other_x),(other_z-robot.z)) * (180/Math.PI));
			if(angle < 0)angle += 360.0;
			double robot_angle = this.transform.localEulerAngles.y;
			
			int bumper_size = 80;
			double sa = angle - robot_angle;

			if(( -bumper_size <= sa && sa <= bumper_size)
				|| ( -bumper_size <= sa-360 && sa-360  <= bumper_size)
				|| ( -bumper_size <= sa+360 && sa+360  <= bumper_size)){
				touch = 2;
			}else{
    			touch = 1;
    		}

    		//output = ""+(int)angle + " "+(int)robot_angle + " " +touch;
        //}
	}

	void OnCollisionExit(Collision other)//非接触
    {
    	//if (other.gameObject.tag == "wall" || true){
        //	Destroy(other.gameObject);  	
        	touch = 0;
        //}
	}
     
    void UltrasonicSensor(){
		const float NOTHING = -1;    // 計測不能
 
    	float maxDistance = 50;      // 計測可能な最大距離
    	
    	// ベクトル計算
        Vector3[] v = new Vector3[4];
        v[0] = transform.TransformDirection(Vector3.forward);//forward back left right
        v[1] = transform.TransformDirection(Vector3.back);//forward back left right
        v[2] = transform.TransformDirection(Vector3.left);//forward back left right
        v[3] = transform.TransformDirection(Vector3.right);//forward back left right
 
        // 距離計算
        for(int i = 0;i < 4;i++){
        	RaycastHit hit;
        	if ( Physics.Raycast(transform.position, v[i], out hit, maxDistance) ) {
            	distance[i] = hit.distance;
        	} else {
            	distance[i] = NOTHING;
        	}
        }        
    }

	void CompassSensor(){
    	compass = this.transform.localEulerAngles.y;
    }
	
	// Update is called once per frame
	void Update () {
    
    }

   // 定時起動関数
    void RobotMain(){
       if(obstacle.flagnum <= 0){
            //soutput = "ミッションクリア！！";

        } else{
            if(delay_flag){
                if((int)(Time.time * 1000) - delay_start_time_ms >= delay_time)delay_flag = false;
            }else{
                UltrasonicSensor();
                CompassSensor();
            
                arduino();
                line_num++;
                if (line_num >= lines.Length){
                    line_num = lines.Length-1;
                    L_moter = 0;
                    R_moter = 0;
                }
            }
            //speed_set
            S_speed = (L_moter + R_moter) / 2;
            R_speed = L_moter - R_moter;

            if(touch == 1){//body
                if(S_speed < 0)S_speed =  0;
                R_speed /= 2;
            }else if(touch == 2){//bamper
                if(S_speed > 0)S_speed =  0;
                R_speed /= 2;
            }
                
            //move
            this.transform.position += this.transform.forward * S_speed * 0.0012f; 
            transform.Rotate(new Vector3(0, 0.015f* R_speed, 0));

            //this.transform.position += this.transform.forward * Time.deltaTime * 0.080f * S_speed; 
            //transform.Rotate(new Vector3(0, Time.deltaTime * 1.0f* R_speed, 0));


           //浮かばないように
            Vector3 pos = transform.position;
            pos.y = 0;
            transform.position = pos;
                
            //for(int i = 0; i < 100000000;i++);
        }
    }
    

    void arduino(){
        
		string[] words = sensor_value(lines[line_num].Trim().Split(' '));
		
		//output = lines[line_num];

		/*output = "";
		foreach(string i in words){
			output += i + " ";
		}*/
		
       //ブロック確認
        if (words[words.Length - 1] == "{"){
            block++;
            b_stack.Push(line_num);
        }

        if (words[0] == "}"){
            variable.del(block);//不要な変数の削除
            block--;
            b_stack.Pop();
        }

        if (jump_stack.Count > 0 && jump_stack.Peek() == line_num){//ジャンプ
            jump_stack.Pop();
            line_num = jump_stack.Pop() - 1;
            return;
        }

        if (words[0] == "int" || words[0] == "double") {//変数宣言
            //Console.WriteLine(lines[i]);
            variable.make(block, words);

        } else if (words[0] == "print") {//出力
            string r = variable.formula(block, words, 2, words.Length - 1);
            //Console.WriteLine(new Parser(r).expr().first);

			output = new Parser(r).expr().first.ToString();
			
            //条件分岐
        } else if ((words[0] == "if")
                    || (words.Length >= 3 && words[0] == "}" && words[1] == "else" && words[2] == "if")) {// if || else if
            string r;
            if (words[0] == "if") r = variable.formula(block, words, 1, words.Length);//if
            else r = variable.formula(block, words, 3, words.Length);//else if

            if (new Parser(r).expr().first != 0) {//条件成立
                string[] ifend;
                int flag = 0;
                int k = line_num + 1, stack = 1;
                do {
                    ifend = lines[k].Trim().Split(' ');
                    for (int j = 0; j < ifend.Length; j++) {
                        ifend[j] = ifend[j].Trim();

                        if (ifend[j] == "}") stack--;
                        if (flag == 0 && stack <= 0) flag = 1;
                        if (ifend[j] == "{") stack++;
                    }

                    if (flag == 1 && ifend[0] == "}") {//ifの終わり位置
                        if (ifend.Length != 1) {
                            jump_stack.Push(k);
                            flag = 2;
                        } else {
                            break;
                        }
                    }
                    k++;
                } while (!(ifend[0] == "}" && ifend.Length == 1 && stack <= 0));
                if (flag >= 2) {//if ,else if, else の終わり位置
                    k--;
                    int tmp = jump_stack.Pop();
                    jump_stack.Push(k);
                    jump_stack.Push(tmp);
                }
            } else {//条件不成立
                string[] ifend;
                int stack = 1;
                bool flag = true;
                line_num++;
                do {//ifの間とばす
                    ifend = lines[line_num].Trim().Split(' ');
                    for (int j = 0; j < ifend.Length; j++) {
                        ifend[j] = ifend[j].Trim();

                        if (ifend[j] == "}") stack--;
                        if (stack <= 0) flag = false;
                        if (ifend[j] == "{") stack++;
                    }
                    line_num++;
                } while (flag);
                line_num -= 2;
            }
        } else if (words[0] == "while") {//ループ
            string r = variable.formula(block, words, 1, words.Length);

            if (new Parser(r).expr().first != 0) {//条件成立
                jump_stack.Push(line_num);//while開始位置

                string[] whileend;
                int stack = 1;
                bool flag = true;
                int k = line_num + 1;
                do {
                    whileend = lines[k].Trim().Split(' ');
                    for (int j = 0; j < whileend.Length; j++) {
                        whileend[j] = whileend[j].Trim();

                        if (whileend[j] == "}") stack--;
                        if (stack <= 0) flag = false;
                        if (whileend[j] == "{") stack++;
                    }
                    k++;
                } while (flag);
                k--;
                jump_stack.Push(k); //while終了位置
            } else {//条件不成立
                string[] whileend;
                int stack = 1;
                bool flag = true;
                line_num++;
                do {//whileの間とばす
                    whileend = lines[line_num].Trim().Split(' ');
                    for (int j = 0; j < whileend.Length; j++) {
                        whileend[j] = whileend[j].Trim();

                        if (whileend[j] == "}") stack--;
                        if (stack <= 0) flag = false;
                        if (whileend[j] == "{") stack++;
                    }
                    line_num++;
                } while (flag);
                line_num -= 2;
            }
        }else if(words[0] == "delay"){
            string r = variable.formula(block, words, 2, words.Length - 1);
            
			delay_time = int.Parse(new Parser(r).expr().first.ToString());
            delay_flag = true;
            delay_start_time_ms = (int)(Time.time * 1000);
        }else if (words[0] == "motor"){
        	string[] moter_f = lines[line_num].Trim().Split(',');
        	string[] motor_l = moter_f[0].Trim().Split(' ');
        	string[] motor_r = moter_f[1].Trim().Split(' ');
        	
        	L_moter = (float)(new Parser(variable.formula(block, motor_l, 2, motor_l.Length)).expr().first);
            R_moter = (float)(new Parser(variable.formula(block, motor_r, 0, motor_r.Length-1)).expr().first);

            if(L_moter < -100)L_moter = -100;
            if(L_moter > 100)L_moter = 100;
            if(R_moter < -100)R_moter = -100;
            if(R_moter > 100)R_moter = 100;
           
        /*    string l = words[2], r = words[4];
            L_moter = Int32.Parse(l);
            R_moter = Int32.Parse(r);
*/
        }else if (words[0] != ""){//代入
            string var_name = variable.name(words[0], block);

			//output = var_name;
			
            if (var_name != ""){
                string r = variable.formula(block, words, 2, words.Length);

                variable.sub(var_name, new Parser(r).expr().first);
            }
        }
    }

    string[] sensor_value(string[] words){//センサー値を取得
		List<string> sl = new List<string>();
		sl.AddRange(words);

		while(true){
			int index = sl.IndexOf("bumper");
			if(index == -1)break;

			sl[index] = (touch == 2)? "1" : "0";
			sl.RemoveRange(index+1,2); // "( )" の削除 
		}

		while(true){
			int index = sl.IndexOf("compass");
			if(index == -1)break;

			sl[index] = "" + compass;
			sl.RemoveRange(index+1,2); // "( )" の削除 
		}

		while(true){
			int index = sl.IndexOf("distance");
			if(index == -1)break;
		
			string t = sl[index+2];//引数	
			sl[index] = "" + distance[Int32.Parse(t)];

			sl.RemoveRange(index+1,3); // "( 引数 )" の削除 	
		}
		
		return sl.ToArray();
    }
}

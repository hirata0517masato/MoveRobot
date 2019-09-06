using System; 

class Source {

    private String str;
    private int pos;

    public Source(String str) {
    	this.str = str.Replace(")", " )").Replace("(", "( ");
        this.str = this.str.Replace("( -","( 0 -").Replace(" = -"," = 0 -");
       
       	if((this.str[0] < '0' || '9' < this.str[0]) && this.str[0] != '(' && this.str[0] != ')')this.str = "0" + this.str;
       	//Console.WriteLine(str);
        this.pos = 0;
    }

    public int peek() {
        if (pos < str.Length) {
            return str[pos];
        }
        return -1;
    }

    public void next() {
        ++pos;
    }
}

class Parser : Source {

    public Parser(String str) : base(str) {
        
    }

	private string CalcType(pair<double,string> a ,pair<double,string> b){
		if(a.second == "int" && b.second == "int")return "int";
		return "double";
	}
	
	private string TypeCheak(string s){
		if(s.IndexOf(".") == -1){
			return "int";
		}
		return "double";
	}
	
    public pair<double,string> number() {
        String s = "";
        int ch;
         
        while ((ch = peek()) >= 0 && (('0'<= ch && ch <= '9') || (char)ch == '.')) {
            s += (char) ch;
            next();
        }
        pair<double,string> p = new pair<double,string>();

		p.second = TypeCheak(s);
        if(p.second == "int")p.first = int.Parse(s);
		else p.first = double.Parse(s);
        
        return p;
    }

	public pair<double,string> expr() {
        pair<double,string> x = comp();           // 項を取得
        for (;;) {
            switch (peek()) {
                case '&':
                    next();
                    switch(peek()){
						case '&':// &&
							next();
							bool a,b;
							if(x.first == 1.0)a = true;
							else a = false;
							
							if(comp().first == 1.0)b = true;
							else b = false;
							
							if( a && b)x.first = 1;
                    		else x.first = 0;
                    		x.second = "int";
                    		continue;
                    	default:
                    		//x &= comp();
                    		break;
                    }
                    continue;
                case '|':
                    next();
                    switch(peek()){
						case '|':// ||
							next();

							bool a,b;
							if(x.first == 1.0)a = true;
							else a = false;
							
							if(comp().first == 1.0)b = true;
							else b = false;
							
							if( a || b)x.first = 1;
                    		else x.first = 0;
                    		x.second = "int";
                    		continue;
                    	default:
                    		//x |= comp();
                    		break;
                    }
                    continue;
            }
            break;
        }
        return x;
    }
    
	public pair<double,string> comp() {
        pair<double,string> x = calc(),y;           // 項を取得
        for (;;) {
            switch (peek()) {
                case '<':
                    next();
                    switch(peek()){
						case '=':// <=
							next();
							y = calc();
							if( x.first <= y.first)x.first = 1;
							else x.first = 0;
							
							x.second = "int";
                    		continue;
                    	default:// <
                    		y = calc();
							if( x.first < y.first)x.first = 1;
							else x.first = 0;
							
							x.second = "int";
                    		continue;
                    }
                case '>':
                    next();
                    switch(peek()){
						case '=':// >=
							next();
							y = calc();
							if( x.first >= y.first)x.first = 1;
							else x.first = 0;
							
							x.second = "int";
                    		continue;
                    	default:// >
                    		y = calc();
							if( x.first > y.first)x.first = 1;
							else x.first = 0;
							
							x.second = "int";
                    		continue;
                    }
                case '=':
                    next();
                    switch(peek()){
						case '=':// ==
							next();
							y = calc();
							if( x.first == y.first)x.first = 1;
							else x.first = 0;
							
							x.second = "int";
                    		continue;
                    }
                    continue;
                case '!':
                    next();
                    switch(peek()){
						case '=':// !=
							next();
							y = calc();
							if( x.first != y.first)x.first = 1;
							else x.first = 0;
							
							x.second = "int";
                    		continue;
                    }
                    continue;
            }
            break;
        }
        return x;
    }
    
    public pair<double,string> calc() {
        pair<double,string> x = term(),y;           // 項を取得
        for (;;) {
            switch (peek()) {
                case '+':
                    next();
                    y = term();
                    x.first += y.first;
                    
                    if(CalcType(x,y) == "int"){
                    	x.second = "int";
                    	x.first = (int)x.first;
                    }else x.second = "double";
                    
                    continue;
                case '-':
                    next();
                    y = term();
                    x.first -= y.first;
                    
                    if(CalcType(x,y) == "int"){
                    	x.second = "int";
                    	x.first = (int)x.first;
                    }else x.second = "double";
                    
                    continue;
            }
            break;
        }
        return x;
    }

     public pair<double,string> term() {
        pair<double,string> x = factor(),y;
        for (;;) {
            switch (peek()) {
                case '*':
                    next();
                    y = factor();
                    x.first *= y.first;
                    
                    if(CalcType(x,y) == "int"){
                    	x.second = "int";
                    	x.first = (int)x.first;
                    }else x.second = "double";
                    
                    continue;
                case '/':
                    next();
                    y = factor();
                    x.first /= y.first;
                    
                    if(CalcType(x,y) == "int"){
                    	x.second = "int";
                    	x.first = (int)x.first;
                    }else x.second = "double";
                    
                    continue;
                case '%':
                    next();
                    y = factor();
                    x.first %= y.first;
                    
                    if(CalcType(x,y) == "int"){
                    	x.second = "int";
                    	x.first = (int)x.first;
                    }else x.second = "double";
                    
                    continue;
            }
            break;
        }
        return x;
    }

    // factor = ("(", expr, ")") | number
    public pair<double,string> factor() {
        pair<double,string> ret = new pair<double,string>();
        spaces();
        if (peek() == '(') {
            next();
            ret = expr();
            if (peek() == ')') {
                next();
            }
        }else if(peek() == '!'){
            next();
            ret = expr();
            if(ret.first == 0)ret.first = 1;
            else ret.first = 0;
            ret.second = "int";
            
        
        } else {
            ret = number();
        }
        spaces();
        return ret;
    }

	public void spaces() {
        while (peek() == ' ') {
            next();
        }
    }

}

class formula { 
	static void test(String s) {
        Console.WriteLine(s + " = " + new Parser(s).expr().first);
    }
    
/*	static void Main(string[] args){ 
      	test("(3*2 )/2 ");
      	test("4.5 / 2");
        test("1 == 1*2");
        test("1 == 1 ");
        test("2 > 0");
        test("1 == 1 && 2 > 0");
        test("1 == 0 || 2 > 0");

        test("3*2 /2 ");
        test("(3*2 )/2 ");
        test("3/2");
        test("3.0/2");
        test("3/2.0");


       // pair<double,string> p = new pair<double,string>();
       // p.first = 1.3;
       // p.second = "ok";
       
	} */
} 

#include"MoveRobot.h"

void loop() { 
	for(int i = 0; i < 53; i++)
    {
        motor(100,0);
    }
    for(int i = 0; i < 53; i++){
        motor(0,100);
    }
}


/*
#include"MoveRobot.h"

void loop() { 
	if(bumper() == 1){
		motor(50,-50);
	}else{
		motor(50,50);
	}
}*/



/*
while(1){
    for(int i = 0; i < 53; i++)
    {
        motor(100,0);
    }
    for(int i = 0; i < 53; i++){
        motor(0,100);
    }
}*/


/*
while(1){
    motor(100,-100);
}*/

/*
while(1){
    for(int i = 0; i < 10; i++){
        motor(50,50);
    }
    for(int i = 0; i < 5; i++){
        motor(-100,100);
    }
}*/

/*for(int i = 0; i < 7; i++){
	motor(-100,100);
}*/
/*
while(1){
	for(int i = 0; i < 50; i++){
		motor(50,90);
	}
	for(int i = 0; i < 80; i++){
		motor(90,50);
	}
}*/
/*
while(1){
	
	if(bumper() == 1){
		motor(50,-50);
	}else{
		motor(50,50);
	}
}*/
/*
while(1){
	
	int c = compass();

	int p = 90 - c;

	motor(p,-p);
}*/
/*
while(1){
	
	int f = distance(0),b = distance(1);

	int p = 4 * (10 + f - b);

	motor(p,p);
}*/


#include"MoveRobot.h"

void loop() { 
	/* 正面がぶつかっていたら */
	if(bumper() == 1){
		/* 右回転 */
		motor(50,-50);
	}else{
		/* 直進 */
		motor(100,100);
	}
}




#include"MoveRobot.h"

void loop() { 
  if(bumper() == 1){
    motor(-50,50);
  }else{
    motor(50,50);
  }
}




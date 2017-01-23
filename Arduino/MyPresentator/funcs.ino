void hello(){  
  pinMode(13, OUTPUT);
  for(int i=0; i<30; i++){
    digitalWrite(13, HIGH);
    delay(10);
    digitalWrite(13, LOW);
    delay(20);
  }
}

void test(){
  if (Serial.available()) {
    String readString = "";
    delay(5);  //delay to allow buffer to fill 
    while (Serial.available() >0) {
      char c = Serial.read();  //gets one byte from serial buffer
      readString += c; //makes the string readString
    } 
    if(readString.equals("test")){
      Serial.println("OK.");
    }
  }
}

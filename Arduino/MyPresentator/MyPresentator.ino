#include <IRremote.h>
#include <IRremoteInt.h>

int RECV_PIN = 2;
IRrecv irrecv(RECV_PIN);
decode_results results;

void setup()
{
  Serial.begin(9600);
  irrecv.enableIRIn(); // Start the receiver
  hello();//"hello" signal to user 
}

void loop()
{
  //receive IR
  if (irrecv.decode(&results))
  {
    Serial.println(results.value, HEX);
    irrecv.resume(); // Receive the next value
    digitalWrite(13, HIGH);
    delay(100);
    digitalWrite(13, LOW);
  }
  test();//answer to the "test" string
}

#include <Wire.h>
#include "nunchuck_funcs.h" // Library needed to read Nunchuck

int count=0;
byte accx,accy,accz,joyx,joyy,zbut,cbut;

void setup()
{
    // Initialise Serial and Nunchuck
    Serial.begin(115200);
    nunchuck_setpowerpins();
    nunchuck_init();
}

void loop()
{
    if( count > 10 ) { // Read Nunchuck every 10ms
        
        nunchuck_get_data(); //Get Data from Nunchuck
        
        // Assign values to varibles for writing to serial
        accx  = nunchuck_accelx();
        accy  = nunchuck_accely();
        accz  = nunchuck_accelz();
        joyx = nunchuck_joyx();
        joyy = nunchuck_joyy();
        zbut = nunchuck_zbutton();
        cbut = nunchuck_cbutton(); 
        
        // Write Data to serial port
        Serial.print(accx,DEC);Serial.print(',');
        Serial.print(accy,DEC);Serial.print(',');
        Serial.print(accz,DEC);Serial.print(',');
        Serial.print(joyx,DEC);Serial.print(',');
        Serial.print(joyy,DEC);Serial.print(',');
        Serial.print(cbut,DEC);Serial.print(',');
        Serial.println(zbut,DEC);

        count = 0;      
    }
    count++;
    delay(1);
}


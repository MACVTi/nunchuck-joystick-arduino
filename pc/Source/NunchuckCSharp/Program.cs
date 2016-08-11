using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Ports;
using vJoyInterfaceWrap;
using System.Threading;

namespace NunchuckCSharp
{
    class Program
    {
        private const uint joyID = 1;

        static void Main(string[] args)
        {
            vJoy joystick = new vJoy(); //Create Joystick Object
            joystick.vJoyEnabled();
            joystick.AcquireVJD(joyID); //Set Joystick to ID 1
            if(args.Length == 0) //Check if no arguments provided
            {
                Console.WriteLine("Error: No COM Port Selected");
                System.Environment.Exit(1);
            }
            SerialPort nunchuck = new SerialPort(args[0], 115200); //Create Serial Port Object
            try //Check to see if we can access the serial port
            {
                nunchuck.Open();
            }
            catch (IOException ex)
            {
                Console.WriteLine("Error: The COM Port could not be Opened");
                Console.WriteLine(ex);
                System.Environment.Exit(1);
            }
            nunchuck.RtsEnable = true;
            nunchuck.DtrEnable = true;
            nunchuck.Handshake = Handshake.XOnXOff; //Set up handshaking with the arduino, mainly used to reset the arduino
            Console.WriteLine("Nunchuck Joystick Program");
            Console.WriteLine("Waiting for COM port to come up...");
            try
            {
                string line = nunchuck.ReadExisting(); //Use this to clear buffer
                Console.WriteLine("COM Port Successfully Opened!");
                joystick.ResetAll(); //Reset Joystick Values
                Console.WriteLine("Press ESC To Exit...");

                do
                {
                    while (!Console.KeyAvailable) //Closes the program on ESC key
                    {
                        Thread.Sleep(10); //Slow read to match arduino
                        line = nunchuck.ReadLine(); //Read Data from Serial port
                        //Format: ACCX,ACCY,ACCZ,JOYX,JOYY,CBUT,ZBUT
                        var numbers = line.Split(',').Select(Int32.Parse).ToList();
                        //Assign values from nunchuck to virtual joystick
                        joystick.SetAxis(Convert.ToInt16(numbers[0]), 1, HID_USAGES.HID_USAGE_RX);
                        joystick.SetAxis(Convert.ToInt16(numbers[1]), 1, HID_USAGES.HID_USAGE_RY);
                        joystick.SetAxis(Convert.ToInt16(numbers[2]), 1, HID_USAGES.HID_USAGE_RZ);
                        joystick.SetAxis(Convert.ToInt16(numbers[3]), 1, HID_USAGES.HID_USAGE_X);
                        joystick.SetAxis(Convert.ToInt16(numbers[4]), 1, HID_USAGES.HID_USAGE_Y);
                        joystick.SetBtn(Convert.ToBoolean(numbers[5]), 1, 1);
                        joystick.SetBtn(Convert.ToBoolean(numbers[6]), 1, 2);
                    }
                } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
            }
            catch (IOException ex)
            {
                Console.WriteLine("Error: A General Error has Occured"); //General error. I haven't encountered this yet in my testing but just in case.
                                                                         //Probably would be caused by bad parsing of the serial data on line 56.
                Console.WriteLine(ex);
                nunchuck.Close(); //Close Serial Port
                System.Environment.Exit(1);
            }
            nunchuck.Close(); //Close Serial Port
            joystick.ResetAll(); //Reset joystick before exit
            System.Environment.Exit(0); //Clean Exit
        }
    }
}

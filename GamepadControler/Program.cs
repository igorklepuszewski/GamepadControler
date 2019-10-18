using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.DirectInput;

namespace GamepadControler
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                SharpDX.DirectInput.DirectInput directInput = new SharpDX.DirectInput.DirectInput();
                Emulator emulator = new Emulator();
                Joystick gamepad = emulator.CheckDevices();
                gamepad.Acquire();
                emulator.Emulate(gamepad);
            }
            catch(Exception e)
            {
                System.Console.WriteLine("Main problem occured");
            }
        }
    }
}

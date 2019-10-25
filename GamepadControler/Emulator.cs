using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.DirectInput;
using System.Runtime.InteropServices;

namespace GamepadControler
{
    class Emulator
    {
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);

        [Flags]
        public enum MouseEventFlags
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010
        }
        public Joystick CheckDevices()
        {
            try {
                SharpDX.DirectInput.DirectInput directInput = new SharpDX.DirectInput.DirectInput();
                var gamepads = directInput.GetDevices(DeviceType.Joystick, SharpDX.DirectInput.DeviceEnumerationFlags.AttachedOnly);
                if (gamepads.Count != 0)
                {
                    System.Console.WriteLine("Gamepad detected: ");
                    foreach (DeviceInstance instance in gamepads)
                    System.Console.WriteLine(instance.InstanceName);
                    Joystick gamepad = new Joystick(directInput, gamepads[0].InstanceGuid);
                    return gamepad;
                }               
            }
            catch(ArgumentOutOfRangeException e)
            {
                System.Console.WriteLine("Nie wykryto pada!");
            }
            return null;
        }

        public bool test(Joystick joystick)
        {
            System.Console.WriteLine("Aby wyjsc wcisnij 7");

            var iks = 10;
            var ygrek = 10;
            var zet = 10;
            var obecny_zet = 0;
            var obecny_iks = 0;
            var obecny_ygrek = 0;

            while (1 != 2)
            {
                if (!joystick.GetCurrentState().Buttons[6])
                {
                    ////START WYKRYCIA ZMIANY POLOZENIA///
                    obecny_iks = iks;
                    obecny_ygrek = ygrek;
                    obecny_zet = zet;
                    iks = joystick.GetCurrentState().X;
                    ygrek = joystick.GetCurrentState().Y;
                    zet = joystick.GetCurrentState().Z;
                    System.Console.WriteLine("Stan joysticka: (" + iks + "," + ygrek + ")");
                    if (obecny_iks != iks || obecny_ygrek != ygrek)
                        System.Console.WriteLine("Stan joysticka: (" + iks + "," + ygrek + ")");
                    //KONIEC WYKRYCIA ZMIANY POLOZENIA////


                    ///START WYKRYCIA PRZYCISKU LPM I PPM////

                    if (joystick.GetCurrentState().Buttons[0])
                        System.Console.WriteLine("Wcisniety LPM! ");
                    if (joystick.GetCurrentState().Buttons[1])
                        System.Console.WriteLine("Wcisniety PPM! ");



                    ///KONIEC WYKRYWANIA PRZYCISKU LPM I PPM///

                    ///START WYKRYCIA RUCHU SUWAKIEM ///
                    if (obecny_zet != zet)
                        System.Console.WriteLine("Stan suwaka: " + joystick.GetCurrentState().Z);
                    ///KONIEC WYKRYCIA RUCHU SUWAKIEM ///
                }
                else
                {
                    return false;
                }

            }
            return true;
        }
        private float spowolnienie = 1;
        private float posX = 0, posY = 0;
        public bool Emulate(Joystick gamepad)
        {
            try
            {
                SharpDX.DirectInput.DirectInput directInput = new SharpDX.DirectInput.DirectInput();
                bool isRightPressed = false;
                System.Console.WriteLine("Want exit? Press button 9 !");
                while (true)
                {
                    if (!gamepad.GetCurrentState().Buttons[8])
                    {
                        int przesuniecie = (1 << 15) - 1;
                        int x = gamepad.GetCurrentState().X - przesuniecie;
                        int y = gamepad.GetCurrentState().Y - przesuniecie;

                        float xKorekta = (float)x / przesuniecie;
                        float yKorekta = (float)y / przesuniecie;

                        int slider = gamepad.GetCurrentState().Z;
                        spowolnienie = 150 / ((1 << 16) - 1) + 1;
                        posX += xKorekta * spowolnienie*2;
                        posY += yKorekta * spowolnienie*2;
                        uint flags = (uint)(MouseEventFlags.ABSOLUTE | MouseEventFlags.MOVE);
                        bool ppm = gamepad.GetCurrentState().Buttons[1];
                        if (gamepad.GetCurrentState().Buttons[0])
                            flags |= (uint)MouseEventFlags.LEFTDOWN;
                        else
                            flags |= (uint)MouseEventFlags.LEFTUP;
                        if (gamepad.GetCurrentState().Buttons[1])
                        {
                            if (!isRightPressed)
                            {
                                flags |= (uint)MouseEventFlags.RIGHTUP;
                                isRightPressed = true;
                            }
                        }
                        else
                        {
                            if (isRightPressed)
                            {
                                flags |= (uint)MouseEventFlags.RIGHTUP;
                                isRightPressed = false;
                            }
                        }               
                        mouse_event(flags, (uint)posX, (uint)posY, 0, 0);
                    }
                    else
                    {
                        return false;
                    }
                }

            }
            catch (ArgumentOutOfRangeException e)
            {
                System.Console.WriteLine(e);
            }
            return true;
        }
    }
}  


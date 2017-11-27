using System.Net.Configuration;
using System.Resources;
using System.Runtime.InteropServices;
using System.Security.Authentication.ExtendedProtection;
using Aimtec.SDK.Events;

namespace ZoomHack
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using Aimtec;
    using Aimtec.SDK.Damage;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Menu;
    using Aimtec.SDK.Menu.Components;
    using Aimtec.SDK.Orbwalking;
    using Aimtec.SDK.TargetSelector;
    using Aimtec.SDK.Util.Cache;
    using Aimtec.SDK.Prediction.Skillshots;
    using Aimtec.SDK.Util;
    using System.Diagnostics;

    internal class Zoomhack
    {

        // Yes Sir

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(int hProcess, int lpBaseAddress, [In, Out] byte[] buffer, int nSize, out int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern Int32 CloseHandle(IntPtr hProcess);


        [DllImport("kernel32.dll")]
        public static extern IntPtr ReadProcessMemory(int hProcess, int lpBaseAddress,
                  [In, Out] byte[] buffer, int size, ref int lpNumberOfBytesRead);

        public static Menu Menu = new Menu("Zoomhack", "Zoomhack", true);

        public static IntPtr BaseAddr = new IntPtr();

        public static IntPtr b1g = new IntPtr(0x16D36A8);
        Process p;
        Int32 processHandle;

        int zoomAddy;

        private void Write(Int32 Handle, int Address, float Value)
        {
            byte[] Buffer = BitConverter.GetBytes(Value);
            Int32 Zero = 0;
            WriteProcessMemory(Handle, Address, Buffer, Buffer.Length, out Zero);
        }

        public void WriteFloat(Int32 Handle, int Address, float Value)
        {
            Write(Handle, Address, Value);
        }

        public Zoomhack()
        {
            p = Process.GetCurrentProcess();
  
            var MainMenu = new Menu("main", "Main");
            {
                MainMenu.Add(new MenuBool("enabled", "Enabled"));
                MainMenu.Add(new MenuSlider("value", "Zoom Value", 2550, 1, 10000));
            }
            Menu.Add(MainMenu);

            Menu.Attach();

            BaseAddr = GetModuleHandle(null);        

            Byte[] buffer = new Byte[4];
            Byte[] floatBuffer = new Byte[sizeof(float)];
            Int32 bytesRead = 0;
            processHandle = (Int32)p.Handle;

            Int32 baseAddress = (BaseAddr.ToInt32() + (int)b1g);
            ReadProcessMemory(processHandle, baseAddress, buffer, buffer.Length, ref bytesRead);
            Int32 baseValue = BitConverter.ToInt32(buffer, 0);

            Int32 firstAddress = baseValue + 0x54;
            ReadProcessMemory(processHandle, firstAddress, buffer, buffer.Length, ref bytesRead);
            Int32 firstValue = BitConverter.ToInt32(buffer, 0);

            Int32 secondAddress = firstValue + 0xC;
            ReadProcessMemory(processHandle, secondAddress, buffer, buffer.Length, ref bytesRead);
            Int32 secondValue = BitConverter.ToInt32(buffer, 0);

            zoomAddy = secondValue + 0x238;
            ReadProcessMemory(processHandle, zoomAddy, floatBuffer, sizeof(float), ref bytesRead);
            float thirdValue = BitConverter.ToSingle(floatBuffer, 0);

            

            Render.OnPresent += Render_OnPresent;
            Console.WriteLine("[PerfectionEnds] Zoomhack [Loaded]");
        }

        private void Render_OnPresent()
        {
            if (Menu["main"]["enabled"].Enabled)
            {
                WriteFloat(processHandle, zoomAddy, Menu["main"]["value"].Value);
            }
            
        }
    }

}             




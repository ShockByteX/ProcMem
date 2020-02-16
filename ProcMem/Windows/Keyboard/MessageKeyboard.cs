using System;
using System.Collections.Generic;
using System.Threading;
using ProcMem.Native;

namespace ProcMem.Windows.Keyboard
{
    public class MessageKeyboard : IKeyboard
    {
        private static MessageKeyboard _instance;
        private static readonly object Lock = new object();

        public static MessageKeyboard Instance
        {
            get
            {
                lock (Lock)
                {
                    if (_instance == null) _instance = new MessageKeyboard();
                    return _instance ?? new MessageKeyboard();
                }
            }
        }

        private readonly Dictionary<KeyboardKey, short> _keyToScan = new Dictionary<KeyboardKey, short>();

        private MessageKeyboard()
        {
            var vkValues = Enum.GetValues(typeof(KeyboardKey));
            foreach (KeyboardKey key in vkValues)
            {
                if (!_keyToScan.ContainsKey(key)) _keyToScan.Add(key, (short)User32.MapVirtualKey((uint)key, VirtualKeyMapType.KeyToScanCode));
            }
        }

        public void KeyDown(KeyboardKey key) => User32.SendInput(1, new[] { GetVirtualKeyInput(key, false) }, Input.Size);
        public void KeyUp(KeyboardKey key) => User32.SendInput(1, new[] { GetVirtualKeyInput(key, true) }, Input.Size);

        public void PressKey(KeyboardKey key) => User32.SendInput(2, new Input[] { GetVirtualKeyInput(key, false), GetVirtualKeyInput(key, true) }, Input.Size);
        public void PressKeys(KeyboardKey[] keys, int delay)
        {
            foreach (var key in keys)
            {
                PressKey(key);
                Thread.Sleep(delay);
            }
        }
        public void PressKeys(string text)
        {
            var inputs = new List<Input>();
            foreach (char c in text)
            {
                inputs.Add(GetUnicodeKeyInput(c, false));
                inputs.Add(GetUnicodeKeyInput(c, true));
            }
            User32.SendInput((uint)inputs.Count, inputs.ToArray(), Input.Size);
        }

        private static Input GetUnicodeKeyInput(char c, bool keyUp)
        {
            var input = new Input()
            {
                Type = InputType.Keyboard,
                Union = new InputUnion()
                {
                    KeyboardInput = new KeyboardInput()
                    {
                        VirtualKey = VirtualKeyShort.None,
                        ScanCode = (short)c,
                        DwFlags = KeyEventF.Unicode | (keyUp ? KeyEventF.KeyUp : 0),
                        DwExtraInfo = UIntPtr.Zero
                    }
                }
            };
            return input;
        }
        private Input GetVirtualKeyInput(KeyboardKey key, bool keyUp)
        {
            var input = new Input()
            {
                Type = InputType.Keyboard,
                Union = new InputUnion()
                {
                    KeyboardInput = new KeyboardInput()
                    {
                        VirtualKey = (VirtualKeyShort)key,
                        ScanCode = _keyToScan[key],
                        DwFlags = KeyEventF.Scancode | (key.IsExtended() ? KeyEventF.ExtendedKey : 0) | (keyUp ? KeyEventF.KeyUp : 0),
                        DwExtraInfo = UIntPtr.Zero
                    }
                }
            };
            return input;
        }
    }
}
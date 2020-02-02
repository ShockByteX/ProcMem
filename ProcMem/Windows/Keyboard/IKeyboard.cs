namespace ProcMem.Windows.Keyboard
{
    public interface IKeyboard
    {
        void KeyDown(KeyboardKey key);
        void KeyUp(KeyboardKey key);
        void PressKey(KeyboardKey key);
        void PressKeys(KeyboardKey[] keys, int delay);
        void PressKeys(string text);
    }
}
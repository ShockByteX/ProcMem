namespace ProcMem.Windows.Keyboard
{
    public static class KeyboardExtensions
    {
        public static bool IsExtended(this KeyboardKey key) => (key == KeyboardKey.RMenu | key == KeyboardKey.RControl | key == KeyboardKey.Left | key == KeyboardKey.Right | key == KeyboardKey.Up | key == KeyboardKey.Down | key == KeyboardKey.Home | key == KeyboardKey.Delete | key == KeyboardKey.PageUp | key == KeyboardKey.PageDown | key == KeyboardKey.End | key == KeyboardKey.Insert | key == KeyboardKey.NumLock | key == KeyboardKey.Snapshot | key == KeyboardKey.Divide);
    }
}
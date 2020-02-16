namespace ProcMem.Windows.Mouse
{
    public interface IMouse
    {
        void PressLeft();
        void PressMiddle();
        void PressRight();
        void ReleaseLeft();
        void ReleaseMiddle();
        void ReleaseRight();
        void ClickLeft();
        void ClickMiddle();
        void ClickRight();
        void DoubleClickLeft();
        //void MoveTo(int x, int y);
        //void ScrollHorizontally(int delta = 120);
        //void ScrollVertically(int delta = 120);
    }
}
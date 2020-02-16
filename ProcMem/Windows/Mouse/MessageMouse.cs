using System.Threading;
using ProcMem.Native;

namespace ProcMem.Windows.Mouse
{
    public class MessageMouse : IMouse
    {
        private static MessageMouse _instance;
        private static readonly object Lock = new object();

        public static MessageMouse Instance
        {
            get
            {
                lock (Lock)
                {
                    _instance = _instance ?? new MessageMouse();
                    return _instance;
                }
            }
        }

        private MessageMouse() { }

        public void PressLeft()
        {
            User32.SendInput(1, new[]
            {
                CreateInput(MouseEventF.LeftDown)
            }, Input.Size);
        }

        public void PressMiddle()
        {
            User32.SendInput(1, new[]
            {
                CreateInput(MouseEventF.MiddleDown)
            }, Input.Size);
        }

        public void PressRight()
        {
            User32.SendInput(1, new[]
            {
                CreateInput(MouseEventF.RightDown)
            }, Input.Size);
        }

        public void ReleaseLeft()
        {
            User32.SendInput(1, new[]
            {
                CreateInput(MouseEventF.LeftUp)
            }, Input.Size);
        }

        public void ReleaseMiddle()
        {
            User32.SendInput(1, new[]
            {
                CreateInput(MouseEventF.MiddleUp)
            }, Input.Size);
        }

        public void ReleaseRight()
        {
            User32.SendInput(1, new[]
            {
                CreateInput(MouseEventF.RightUp)
            }, Input.Size);
        }

        public void ClickLeft()
        {
            User32.SendInput(2, new[]
            {
                CreateInput(MouseEventF.LeftDown),
                CreateInput(MouseEventF.LeftUp)
            }, Input.Size);
        }

        public void ClickMiddle()
        {
            User32.SendInput(2, new[]
            {
                CreateInput(MouseEventF.MiddleDown),
                CreateInput(MouseEventF.MiddleUp)
            }, Input.Size);
        }

        public void ClickRight()
        {
            User32.SendInput(2, new[]
            {
                CreateInput(MouseEventF.RightDown),
                CreateInput(MouseEventF.RightUp)
            }, Input.Size);
        }

        public void DoubleClickLeft()
        {
            ClickLeft();
            Thread.Sleep(10);
            ClickLeft();
        }

        private static Input CreateInput(MouseEventF mouseEvent)
        {
            var input = new Input()
            {
                Type = InputType.Mouse,
                Union = new InputUnion()
                {
                    MouseInput = new MouseInput()
                    {
                        DwFlags = mouseEvent
                    }
                }
            };

            return input;
        }
    }
}
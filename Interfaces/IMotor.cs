namespace AutomationControls.Interfaces
{
    public interface IMotor
    {
        void MoveTo(int degrees);
        void SetSpeed(int speed); // 0-10
        void Enable();
        void Disable();
        void Home();
    }
}

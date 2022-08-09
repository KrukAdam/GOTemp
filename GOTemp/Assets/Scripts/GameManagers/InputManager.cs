public class InputManager
{
    public InputActions InputActions
    {
        get; 
        private set;
    }

    public void Init()
    {
        InputActions = new InputActions();
    }

    public void Enable()
    {
        InputActions.Enable();
    }

    public void Disable()
    {
        InputActions.Disable();
    }
}

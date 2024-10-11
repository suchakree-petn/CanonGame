public interface ICommand
{
    public int Piority
    {
        get;
        set;
    }

    void Execute();
}

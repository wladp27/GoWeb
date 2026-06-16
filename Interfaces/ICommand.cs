namespace GoWeb.Interfaces
{
    public interface ICommand
    {
        string CommandName { get; }
        void Execute(int idEvent);
    }
}

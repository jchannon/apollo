namespace Apollo
{
    using System.Threading.Tasks;

    public interface ICommandHandler
    {
        (object Data, Error Error) Execute(object command);

        Task<Error> Handle(object command);
    }
}

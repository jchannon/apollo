namespace Apollo
{
    using System.Threading.Tasks;

    public abstract class CommandHandler<T> : ICommandHandler
    {
        (object Data, Error Error) ICommandHandler.Execute(object command)
        {
            return this.Execute((T)command);
        }

        Task<Error> ICommandHandler.Handle(object command)
        {
            return this.Handle((T)command);
        }

        protected virtual Task<Error> Handle(T command)
        {
            return default;
        }

        protected virtual (object Data, Error Error) Execute(object command)
        {
            return default;
        }
    }
}

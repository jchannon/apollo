namespace Apollo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class Handler
    {
        private readonly IEnumerable<ICommandHandler> handlers;

        public Handler(IEnumerable<ICommandHandler> handlers)
        {
            this.handlers = handlers;
        }

        public Task<Error> Handle<T>(T command) where T : class
        {
            var handler = this.handlers.FirstOrDefault(x => x.GetType().BaseType.GenericTypeArguments.Any(y => y == typeof(T)));

            if (handler == null)
            {
                throw new Exception($"Unfound handler for {typeof(T).Name}");
            }

            return handler.Handle(command);
        }

        public (U Data, Error Error) Execute<T, U>(T command) where T : class
        {
            var handler = this.handlers.FirstOrDefault(x => x.GetType().BaseType.GenericTypeArguments.Any(y => y == typeof(T)));

            if (handler == null)
            {
                throw new Exception($"Unfound handler for {typeof(T).Name}");
            }

            var res = handler.Execute(command);

            return ((U)res.Data, res.Error);
        }
    }
}

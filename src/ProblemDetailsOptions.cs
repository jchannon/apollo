// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Apollo
{
    using System;
    using System.Collections.Generic;

    public class ProblemDetailsOptions
    {
        private readonly List<ExceptionMapper> mappers;

        public ProblemDetailsOptions()
        {
            this.mappers = new List<ExceptionMapper>();
        }

        public void Map<TException>(Func<TException, ProblemDetails> mapping)
            where TException : Exception
        {
            this.mappers.Add(new ExceptionMapper(typeof(TException), ex => mapping((TException)ex)));
        }

        internal bool TryMapProblemDetails(Exception exception, out ProblemDetails problem)
        {
            foreach (var mapper in this.mappers)
            {
                if (mapper.TryMap(exception, out problem))
                {
                    return true;
                }
            }

            problem = default;
            return false;
        }

        private sealed class ExceptionMapper
        {
            public ExceptionMapper(Type type, Func<Exception, ProblemDetails> mapping)
            {
                this.Type = type;
                this.Mapping = mapping;
            }

            private Type Type { get; }

            private Func<Exception, ProblemDetails> Mapping { get; }

            public bool CanMap(Type type)
            {
                return this.Type.IsAssignableFrom(type);
            }

            public bool TryMap(Exception exception, out ProblemDetails problem)
            {
                if (this.CanMap(exception.GetType()))
                {
                    try
                    {
                        problem = this.Mapping(exception);
                        return true;
                    }
                    catch
                    {
                        problem = default;
                        return false;
                    }
                }

                problem = default;
                return false;
            }
        }
    }
}

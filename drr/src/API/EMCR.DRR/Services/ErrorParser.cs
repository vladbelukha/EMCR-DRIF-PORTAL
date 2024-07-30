using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace EMCR.DRR.API.Services
{
    public class ServerException : Exception
    {
        public ServerException(string type, string message) : base(message)
        {
            Type = type;
        }

        public string Type { get; }

        public override string ToString()
        {
            return $"{Type}: {Message}";
        }
    }

    [KnownType(typeof(NotFoundException))]
    public abstract class DrrApplicationException : Exception
    {
        protected DrrApplicationException(string message) : base(message)
        {
        }
    }

    public class NotFoundException : DrrApplicationException
    {
        public NotFoundException(string message) : base(message)
        {
        }

        public NotFoundException(string message, string id) : base(message)
        {
            Id = id;
        }

        public string Id { get; }
    }

    public class UnauthorizedException : DrrApplicationException
    {
        public UnauthorizedException(string message) : base(message)
        {
        }

        public UnauthorizedException(string message, string id) : base(message)
        {
            Id = id;
        }

        public string Id { get; }
    }

    public class ErrorParser
    {
        public ActionResult Parse(DrrApplicationException ex)
        {
            return ex switch
            {
                NotFoundException e => new NotFoundObjectResult(new ProblemDetails { Type = "NotFoundException", Title = "Not Found", Detail = e.Message }),
                UnauthorizedException e => new UnauthorizedObjectResult(new ProblemDetails { Type = "UnauthorizedException", Title = "Not Authorized", Detail = e.Message }),
                _ => throw ex
            };
        }
    }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

// ProLeague.Application/Result.cs
namespace ProLeague.Application
{
    public record Result(bool Succeeded, IEnumerable<string>? Errors = null)
    {
        public static Result Success() => new(true);
        public static Result Failure(IEnumerable<string> errors) => new(false, errors);
    }
}
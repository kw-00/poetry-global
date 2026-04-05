namespace PoetryGlobal.Application.Exceptions
{
    public class EnvironmentVariableNotSetException(string variableName) : Exception($"Environment variable '{variableName}' is not set.");
}
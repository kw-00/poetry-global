namespace PoetryGlobal.Exceptions
{
    public class EnvironmentVariableNotSetException(string variableName) 
        : Exception($"Environment variable '{variableName}' is not set.");
}
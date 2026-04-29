using System.Text.Json;
using PoetryGlobal.Config;

namespace PoetryGlobal.Infrastructure
{
    public class LimitedMyMemoryTranslationService(
        HttpClient httpClient,
        IConfigWithValidation configuration,
        JsonSerializerOptions jsonSerializerOptions
    ) : MyMemoryTranslationService(
        httpClient,
        configuration,
        jsonSerializerOptions
    )
    {

        public override Task<string> TranslatePoemAsync(
            string poemLinesMerged, string sourceLanguage, string targetLanguage
        )
        {
            var firstLine = poemLinesMerged.Split("\n").First();
            return base.TranslatePoemAsync(firstLine, sourceLanguage, targetLanguage);
        }
    }
}

namespace PoetryGlobal.Features.Languages
{
    public interface ILanguagesService
    {
        Task<GetAllLanguagesResponse> GetAllLanguagesAsync();
    }
}
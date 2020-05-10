using System.Threading.Tasks;

namespace hrmcHelloWorld.Services
{
    public interface ITokenService
    {
        Task<string> GetToken();
    }
}
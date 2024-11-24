using System.Threading.Tasks;
namespace ABC123_HSZF_2024251.Application.Interfaces
{
    public interface IDataImporterService
    {
        Task ImportDataAsync(string filePath);
    }
}


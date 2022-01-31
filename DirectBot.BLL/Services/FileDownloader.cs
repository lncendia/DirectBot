using DirectBot.Core.Interfaces;
using DirectBot.Core.Services;
using Telegram.Bot;

namespace DirectBot.BLL.Services;

public class FileDownloader : IFileDownloader
{
    private readonly ITelegramBotClient _client;

    public FileDownloader(ITelegramBotClient client) => _client = client;

    public async Task<IResult<Stream>> DownloadFileAsync(string fileId, CancellationToken token)
    {
        try
        {
            var ms = new MemoryStream();
            await _client.GetInfoAndDownloadFileAsync(fileId, ms, token);
            ms.Position = 0;
            return Result<Stream>.Ok(ms);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return Result<Stream>.Fail(ex.Message);
        }
    }
}
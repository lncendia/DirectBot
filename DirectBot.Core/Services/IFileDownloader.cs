using DirectBot.Core.Interfaces;

namespace DirectBot.Core.Services;

public interface IFileDownloader
{
   Task<IResult<Stream>> DownloadFileAsync(string fileId, CancellationToken token);
}
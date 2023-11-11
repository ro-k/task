namespace Lib.DataAccess;

public class LocalFileStorageDataAccess : IFileStorageDataAccess
{
    public interface IFileStreamWrapper
    {
        public Stream GetNew(string filePath, FileMode fileMode, FileAccess fileAccess);
        public void Delete(string filePath);
    }

    public class FileStreamWrapper : IFileStreamWrapper
    {
        public Stream GetNew(string filePath, FileMode fileMode, FileAccess fileAccess) =>
            new FileStream(filePath, fileMode, fileAccess);

        public void Delete(string filePath)
        {
            File.Delete(filePath);
        }
    }

    private readonly IFileStreamWrapper _fileStreamWrapper;

    public LocalFileStorageDataAccess(IFileStreamWrapper fileStreamWrapper)
    {
        _fileStreamWrapper = fileStreamWrapper;
    }

    public async Task Store(string filePath, Stream contents)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return;
        }
        
        await using var fs = _fileStreamWrapper.GetNew(filePath, FileMode.Create, FileAccess.Write);
        await contents.CopyToAsync(fs);
    }

    public async Task<Stream> Retrieve(string filePath)
    {
        await using var fileStream = _fileStreamWrapper.GetNew(filePath, FileMode.Open, FileAccess.Read);
        return fileStream;
    }

    public Task Delete(string filePath)
    {
        _fileStreamWrapper.Delete(filePath);
        return Task.CompletedTask;
    }
}
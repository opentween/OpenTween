namespace OpenTween
{
    public interface IMultimediaShareService
    {
        string Upload(ref string filePath,
                        ref string message,
                        long reply_to);
        bool CheckValidExtension(string ext) ;
        string GetFileOpenDialogFilter();
        MyCommon.UploadFileType GetFileType(string ext);
        bool IsSupportedFileType(MyCommon.UploadFileType type);
        bool CheckValidFilesize(string ext, long fileSize);
        bool Configuration(string key, object value);
    }
}

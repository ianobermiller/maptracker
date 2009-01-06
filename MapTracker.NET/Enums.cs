namespace MapTracker.NET
{
    enum SPECIAL_BYTES
    {
        NODE_START = 0xFE,
        NODE_END = 0xFF,
        ESCAPE_CHAR = 0xFD,
    };

    enum FILELOADER_ERRORS
    {
        ERROR_NONE,
        ERROR_INVALID_FILE_VERSION,
        ERROR_CAN_NOT_OPEN,
        ERROR_CAN_NOT_CREATE,
        ERROR_EOF,
        ERROR_SEEK_ERROR,
        ERROR_NOT_OPEN,
        ERROR_INVALID_NODE,
        ERROR_INVALID_FORMAT,
        ERROR_TELL_ERROR,
        ERROR_COULDNOTWRITE,
        ERROR_CACHE_ERROR,
    };
}
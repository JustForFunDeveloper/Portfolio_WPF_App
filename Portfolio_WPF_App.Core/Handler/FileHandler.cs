using System;
using System.IO;
using System.Threading;

namespace Portfolio_WPF_App.Core.Handler
{
    /// <summary>
    /// This class should handle basic file / folder methods.
    /// All calls should be handled asynchron with threads.
    /// All calls should have progress, finish and error events.
    /// </summary>
    public class FileHandler
    {
        private int progressUpdateInterval = 500;
        /// <summary>
        /// This variable sets the interval of all progress threads.
        /// </summary>
        public int ProgressUpdateInterval { get => progressUpdateInterval; set => progressUpdateInterval = value; }

        #region Event members
        #region Files
        public event EventHandler<ProgressArgs> CopyFileProgress;
        public event EventHandler CopyFileFinished;
        public event EventHandler<ExceptionArgs> CopyFileError;

        public event EventHandler DeleteFileFinished;
        public event EventHandler<ExceptionArgs> DeleteFileError;

        public event EventHandler<ProgressArgs> MoveFileProgress;
        public event EventHandler MoveFileFinished;
        public event EventHandler<ExceptionArgs> MoveFileError;
        #endregion

        #region Folders
        public event EventHandler<ProgressArgs> CopyFolderProgress;
        public event EventHandler CopyFolderFinished;
        public event EventHandler<ExceptionArgs> CopyFolderError;

        public event EventHandler<ProgressArgs> DeleteFolderProgress;
        public event EventHandler DeleteFolderFinished;
        public event EventHandler<ExceptionArgs> DeleteFolderError;

        public event EventHandler<ProgressArgs> MoveFolderProgress;
        public event EventHandler MoveFolderFinished;
        public event EventHandler<ExceptionArgs> MoveFolderError;
        #endregion
        #endregion

        #region Public methods
        /// <summary>
        /// This method overwrites a given text to the given file.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <param name="text">The given text to overwrite.</param>
        /// <param name="path">The given path. Should end with '\'.</param>
        /// <returns>Returns -1 if something went wrong.</returns>
        public short OverwriteFile(string fileName, string text, string path = "")
        {
            try
            {
                FileInfo fileInfo = new FileInfo(path + fileName);
                if (!fileInfo.Exists)
                    throw new Exception("File doesn't exist!");

                File.WriteAllText(path + fileName, text);
            }
            catch (Exception e)
            {
                LogHandler.WriteLog(e.StackTrace, LogLevel.ERROR);
                return -1;
            }
            return 0;
        }

        /// <summary>
        /// Reads all lines of the text and returns it
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="path"></param>
        /// <returns>Returns null if something went wrong.</returns>
        public string ReadAllText(string fileName, string path = "")
        {
            FileInfo fileInfo = new FileInfo(path + fileName);
            if (!fileInfo.Exists)
                return null;

            return File.ReadAllText(path + fileName);
        }

        /// <summary>
        /// This methods creates a file and adds a basic header if withHeader is true.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <param name="path">The given path. Should end with '\'.</param>
        /// <param name="withHeader">Adds a header if true.</param>
        /// <returns>Returns -1 if something went wrong and returns 1 if file did exist.</returns>
        public short CreateFileIfNotExist(string fileName, string path = "", bool withHeader = true)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(path + fileName);
                if (fileInfo.Exists)
                    return 1;

                DateTime dt = DateTime.Now;
                string[] lines = { fileName, "Created at: " + dt.ToLocalTime().ToString() };

                if (withHeader)
                    File.WriteAllLines(path + fileName, lines);
                else
                    File.WriteAllText(path + fileName, "");
            }
            catch (Exception e)
            {
                LogHandler.WriteLog(e.StackTrace, LogLevel.ERROR);
                return -1;
            }
            return 0;
        }

        /// <summary>
        /// This method appends a given line to the given file.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <param name="text">The given text to append.</param>
        /// <param name="path">The given path. Should end with '\'.</param>
        /// <returns>Returns -1 if something went wrong.</returns>
        public short AppendText(string fileName, string text, string path = "", bool withNewline = true)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(path + fileName);
                if (!fileInfo.Exists)
                    throw new Exception("File doesn't exist!");

                using (StreamWriter file = new StreamWriter(path + fileName, true))
                {
                    if (withNewline)
                        file.WriteLine(text);
                    else
                        file.Write(text);
                }
            }
            catch (Exception e)
            {
                LogHandler.WriteLog(e.StackTrace, LogLevel.ERROR);
                return -1;
            }
            return 0;
        }

        /// <summary>
        /// This methods adss all lines from a given string array to the given file.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <param name="lines">The lines in a strin array whizch sould be appended.</param>
        /// <param name="path">The given path. Should end with '\'.</param>
        /// <returns>Returns -1 if something went wrong.</returns>
        public short AppendAll(String fileName, string[] lines, string path = "")
        {
            try
            {
                FileInfo fileInfo = new FileInfo(path + fileName);
                if (!fileInfo.Exists)
                    throw new Exception("File doesn't exist!");

                using (StreamWriter file = new StreamWriter(path + fileName, true))
                {
                    foreach (string line in lines)
                    {
                        file.WriteLine(line);
                    }
                }
            }
            catch (Exception e)
            {
                LogHandler.WriteLog(e.StackTrace, LogLevel.ERROR);
                return -1;
            }
            return 0;
        }

        /// <summary>
        /// This method takes the sourceFile and copies it asynchron to the given destination.
        /// The progress is sent periodically through an event.
        /// </summary>
        /// <param name="sourceFile">The complete sourcefile path which should be copied.</param>
        /// <param name="destination">The destination path.</param>
        /// <param name="overwrite">If true an already existing file will be overwritten.</param>
        /// <returns>Returns -1 if something went wrong otherwise 0.</returns>
        public short CopyFile(string sourceFile, string destination, Boolean overwrite)
        {
            try
            {
                FileInfo sourceFileInfo = new FileInfo(sourceFile);
                string destinationFile = destination + "\\" + sourceFileInfo.Name;
                FileInfo destinationFileInfo = new FileInfo(destinationFile);

                if (!sourceFileInfo.Exists)
                    throw new Exception("File doesn't exist!");

                if (destinationFileInfo.Exists && !overwrite)
                    throw new Exception("Not allowed to overwrite file!");

                AsyncCopy(sourceFile, sourceFileInfo, destination, destinationFile);
            }
            catch (Exception e)
            {
                LogHandler.WriteLog(e.StackTrace, LogLevel.ERROR);
                return -1;
            }
            return 0;
        }

        /// <summary>
        /// This method deletes the given file asynchron.
        /// </summary>
        /// <param name="sourceFile">The complete sourcefile path which should be deleted.</param>
        /// Returns -1 if something went wrong otherwise 0.</returns>
        public short DeleteFile(string sourceFile)
        {
            try
            {
                FileInfo sourceFileInfo = new FileInfo(sourceFile);

                if (!sourceFileInfo.Exists)
                    throw new Exception("File doesn't exist!");

                AsyncDelete(sourceFile);
            }
            catch (Exception e)
            {
                LogHandler.WriteLog(e.StackTrace, LogLevel.ERROR);
                return -1;
            }
            return 0;
        }

        /// <summary>
        /// This method takes the sourceFile and moves it asynchron to the given destination.
        /// The progress is sent periodically through an event.
        /// </summary>
        /// <param name="sourceFile">The complete sourcefile path which should be copied.</param>
        /// <param name="destination">The destination path.</param>
        /// <param name="overwrite">If true an already existing file will be overwritten.</param>
        /// <returns>Returns -1 if something went wrong otherwise 0.</returns>
        public short MoveFile(string sourceFile, string destination, Boolean overwrite)
        {
            try
            {
                FileInfo sourceFileInfo = new FileInfo(sourceFile);
                string destinationFile = destination + "\\" + sourceFileInfo.Name;
                FileInfo destinationFileInfo = new FileInfo(destinationFile);

                if (!sourceFileInfo.Exists)
                    throw new Exception("File doesn't exist!");

                if (destinationFileInfo.Exists && !overwrite)
                    throw new Exception("Not allowed to overwrite file!");

                AsyncMoveCopy(sourceFile, sourceFileInfo, destination, destinationFile);
            }
            catch (Exception e)
            {
                LogHandler.WriteLog(e.StackTrace, LogLevel.ERROR);
                return -1;
            }
            return 0;
        }

        /// <summary>
        /// Not implemented yet.
        /// </summary>
        /// <param name="sourceFolder"></param>
        /// <param name="destFolder"></param>
        public void CopyFolder(string sourceFolder, string destFolder)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented yet.
        /// </summary>
        /// <param name="folder"></param>
        public void DeleteFolder(string folder)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented yet.
        /// </summary>
        /// <param name="sourceFolder"></param>
        /// <param name="destFolder"></param>
        public void MoveFolder(string sourceFolder, string destFolder)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region private Methods
        /// <summary>
        /// <see cref="CopyFile(string, string, bool)"/>
        /// </summary>
        /// <param name="sourceFile">The complete sourcefile path which should be copied.</param>
        /// <param name="sourceFileInfo">The file info of the source file</param>
        /// <param name="destination">The destination path.</param>
        /// <param name="destinationFile">The file info of the destination file</param>
        private async void AsyncCopy(string sourceFile, FileInfo sourceFileInfo, string destination, string destinationFile)
        {
            long sourceSize = sourceFileInfo.Length;

            Thread progressThread = new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(progressUpdateInterval);
                    try
                    {
                        long currentSize = new FileInfo(destinationFile).Length;
                        OnCopyFileProgress(new ProgressArgs(currentSize, sourceSize));

                        if (sourceSize.Equals(currentSize))
                            break;
                    }
                    catch (Exception e)
                    {
                        LogHandler.WriteLog(e.StackTrace, LogLevel.ERROR);
                        OnCopyFileError(new ExceptionArgs(e));
                    }
                }
            });
            progressThread.IsBackground = true;
            progressThread.Start();

            try
            {
                using (FileStream SourceStream = File.Open(sourceFile, FileMode.Open))
                {
                    using (FileStream DestinationStream = File.Create(destination + sourceFile.Substring(sourceFile.LastIndexOf('\\'))))
                    {
                        await SourceStream.CopyToAsync(DestinationStream);
                        progressThread.Join();
                        OnCopyFileFinished();
                    }
                }
            }
            catch (Exception e)
            {
                LogHandler.WriteLog(e.StackTrace, LogLevel.ERROR);
                OnCopyFileError(new ExceptionArgs(e));
                progressThread.Abort();
            }
        }

        /// <summary>
        /// <see cref="DeleteFile(string)"/>
        /// </summary>
        /// <param name="sourceFile">The complete sourcefile path which should be deleted.</param>
        private async void AsyncDelete(string sourceFile)
        {
            try
            {
                using (FileStream stream = new FileStream(sourceFile, FileMode.Truncate, FileAccess.Write, FileShare.Delete, 4096, true))
                {
                    await stream.FlushAsync();
                    File.Delete(sourceFile);
                    OnDeleteFileFinished();
                }
            }
            catch (Exception e)
            {
                LogHandler.WriteLog(e.StackTrace, LogLevel.ERROR);
                OnDeleteFileError(new ExceptionArgs(e));
            }
        }

        /// <summary>
        /// <see cref="MoveFile(string, string, bool)"/>
        /// </summary>
        /// <param name="sourceFile">The complete sourcefile path which should be copied.</param>
        /// <param name="sourceFileInfo">The file info of the source file</param>
        /// <param name="destination">The destination path.</param>
        /// <param name="destinationFile">The file info of the destination file</param>
        private async void AsyncMoveCopy(string sourceFile, FileInfo sourceFileInfo, string destination, string destinationFile)
        {
            long sourceSize = sourceFileInfo.Length;

            Thread progressThread = new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(progressUpdateInterval);
                    try
                    {
                        long currentSize = new FileInfo(destinationFile).Length;
                        OnMoveFileProgress(new ProgressArgs(currentSize, sourceSize));

                        if (sourceSize.Equals(currentSize))
                            break;
                    }
                    catch (Exception e)
                    {
                        LogHandler.WriteLog(e.StackTrace, LogLevel.ERROR);
                        OnMoveFileError(new ExceptionArgs(e));
                    }
                }
            });
            progressThread.IsBackground = true;
            progressThread.Start();

            try
            {
                using (FileStream SourceStream = File.Open(sourceFile, FileMode.Open))
                {
                    using (FileStream DestinationStream = File.Create(destination + sourceFile.Substring(sourceFile.LastIndexOf('\\'))))
                    {
                        await SourceStream.CopyToAsync(DestinationStream);
                        progressThread.Join();
                    }
                }
                AsyncMoveDelete(sourceFile);
            }
            catch (Exception e)
            {
                LogHandler.WriteLog(e.StackTrace, LogLevel.ERROR);
                OnMoveFileError(new ExceptionArgs(e));
                progressThread.Abort();
            }
        }

        /// <summary>
        /// <see cref="MoveFile(string, string, bool)"/>
        /// </summary>
        /// <param name="sourceFile">The complete sourcefile path which should be deleted.</param>
        private async void AsyncMoveDelete(string sourceFile)
        {
            try
            {
                using (FileStream stream = new FileStream(sourceFile, FileMode.Truncate, FileAccess.Write, FileShare.Delete, 4096, true))
                {
                    await stream.FlushAsync();
                    File.Delete(sourceFile);
                    OnMoveFileFinished();
                }
            }
            catch (Exception e)
            {
                LogHandler.WriteLog(e.StackTrace, LogLevel.ERROR);
                OnMoveFileError(new ExceptionArgs(e));
            }
        }
        #endregion

        #region Event Methods
        #region Files
        #region CopyFile
        protected virtual void OnCopyFileProgress(ProgressArgs e)
        {
            CopyFileProgress?.Invoke(this, e);
        }

        protected virtual void OnCopyFileFinished()
        {
            CopyFileFinished?.Invoke(this, null);
        }

        protected virtual void OnCopyFileError(ExceptionArgs e)
        {
            CopyFileError?.Invoke(this, e);
        }
        #endregion

        #region DeleteFile
        protected virtual void OnDeleteFileFinished()
        {
            DeleteFileFinished?.Invoke(this, null);
        }

        protected virtual void OnDeleteFileError(ExceptionArgs e)
        {
            DeleteFileError?.Invoke(this, e);
        }
        #endregion

        #region MoveFile
        protected virtual void OnMoveFileProgress(ProgressArgs e)
        {
            MoveFileProgress?.Invoke(this, e);
        }

        protected virtual void OnMoveFileFinished()
        {
            MoveFileFinished?.Invoke(this, null);
        }

        protected virtual void OnMoveFileError(ExceptionArgs e)
        {
            MoveFileError?.Invoke(this, e);
        }
        #endregion
        #endregion

        #region Folders
        #region CopyFolders
        protected virtual void OnCopyFolderProgress(ProgressArgs e)
        {
            CopyFolderProgress?.Invoke(this, e);
        }

        protected virtual void OnCopyFolderFinished()
        {
            CopyFolderFinished?.Invoke(this, null);
        }

        protected virtual void OnCopyFolderError(ExceptionArgs e)
        {
            CopyFolderError?.Invoke(this, e);
        }
        #endregion

        #region DeleteFolder
        protected virtual void OnDeleteFolderProgress(ProgressArgs e)
        {
            DeleteFolderProgress?.Invoke(this, e);
        }

        protected virtual void OnDeleteFolderFinished()
        {
            DeleteFolderFinished?.Invoke(this, null);
        }

        protected virtual void OnDeleteFolderError(ExceptionArgs e)
        {
            DeleteFolderError?.Invoke(this, e);
        }
        #endregion

        #region MoveFolder
        protected virtual void OnMoveFolderProgress(ProgressArgs e)
        {
            MoveFolderProgress?.Invoke(this, e);
        }

        protected virtual void OnMoveFolderFinished()
        {
            MoveFolderFinished?.Invoke(this, null);
        }

        protected virtual void OnMoveFolderError(ExceptionArgs e)
        {
            MoveFolderError?.Invoke(this, e);
        }
        #endregion
        #endregion
        #endregion
    }

    #region EventArgs classes
    public class ProgressArgs : EventArgs
    {
        public long CurrentSize;
        public long TargetSize;

        public ProgressArgs(long currentSize, long targetSize)
        {
            CurrentSize = currentSize;
            TargetSize = targetSize;
        }
    }

    public class ExceptionArgs : EventArgs
    {
        public Exception Exception;

        public ExceptionArgs(Exception exception)
        {
            Exception = exception;
        }
    }
    #endregion
}

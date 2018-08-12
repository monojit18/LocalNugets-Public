/*
 * 
 * Copyright 2018 Monojit Datta

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 *
 */

using System;
using Diagnostics = System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using Subsystems.CustomFileManager.External;

namespace Subsystems.CustomFileManager.Internal                    
{

    public class CMPFileManager
    {

        #region Private/Protected Variables
        private const string kCopySubscriptString = "_copy";
        private const long kSizeInKB = 1024;
        private string _folderPathString;
        private SemaphoreSlim _fileSemaphore;
        #endregion

        #region Private/Protected Methods
        private string PrepareFilePath(string fileNameString)
        {

            if (string.IsNullOrEmpty(fileNameString))
                return null;

            var couldPrepare = PrepareFoldePath(_folderPathString);
            if (couldPrepare == false)
                return null;

            var filePathString = string.Concat(_folderPathString, "/", fileNameString);
            return filePathString;

        }

        private Tuple<FileStream, byte[]> PrepareFileStream(string filePathString, byte[] contentBytesArray,
                                                            FileCreationType creationType)
        {

            if ((contentBytesArray == null) || (contentBytesArray.Length == 0))
                return null;

            if (string.IsNullOrEmpty(filePathString))
                return null;

            FileStream fileStream = null;
            switch (creationType)
            {

                case FileCreationType.Overwrite:
                    {

                        fileStream = File.Create(filePathString);
                        return (new Tuple<FileStream, byte[]>(fileStream, contentBytesArray));

                    }

                case FileCreationType.Copy:
                    {

                        var fileCopyPathString = string.Concat(filePathString, kCopySubscriptString);
                        File.Copy(filePathString, fileCopyPathString);
                        fileStream = File.Create(filePathString);
                        return (new Tuple<FileStream, byte[]>(fileStream, contentBytesArray));

                    }

                case FileCreationType.Append:
                    {

                        var existingBytesArray = (DoesFileExist(filePathString) == true)
                                ? File.ReadAllBytes(filePathString) : null;

                        using (fileStream = File.Create(filePathString))
                        {

                            if (fileStream == null)
                                return null;

                            byte[] appendedBytesArray = null;
                            if ((existingBytesArray == null) || (existingBytesArray.Length == 0))
                            {

                                appendedBytesArray = new byte[contentBytesArray.Length];
                                contentBytesArray.CopyTo(appendedBytesArray, contentBytesArray.Length);
                                return (new Tuple<FileStream, byte[]>(fileStream, contentBytesArray));

                            }

                            appendedBytesArray = new byte[existingBytesArray.Length + contentBytesArray.Length];
                            existingBytesArray.CopyTo(appendedBytesArray, 0);
                            contentBytesArray.CopyTo(appendedBytesArray, existingBytesArray.Length);
                            return (new Tuple<FileStream, byte[]>(fileStream, appendedBytesArray));

                        }

                    }
            }

            return null;

        }
        #endregion


        #region Public Methods
        public static bool PrepareFoldePath(string folderPathString)
        {

            try
            {

                if (string.IsNullOrEmpty(folderPathString))
                    return false;

                var doesExist = Directory.Exists(folderPathString);
                if (doesExist == true)
                    return true;

                var directoryInfo = Directory.CreateDirectory(folderPathString);
                return (directoryInfo != null);


            }
            catch (AggregateException exception)
            {

                Diagnostics.Debug.WriteLine(exception.StackTrace);
                return false;

            }

        }

        public CMPFileManager(string folderPathString)
        {

            _folderPathString = string.Copy(folderPathString);
            _fileSemaphore = new SemaphoreSlim(1);

        }

        public bool DoesFileExist(string fileNameString)
        {

            var filePathString = PrepareFilePath(fileNameString);
            if (string.IsNullOrEmpty(filePathString))
                return false;

            return (File.Exists(filePathString));

        }

        public byte[] GetContents(string fileNameString)
        {

            try
            {

                var filePathString = PrepareFilePath(fileNameString);
                if (string.IsNullOrEmpty(filePathString))
                    return null;

                using (var fileStream = File.OpenRead(filePathString))
                {

                    if (fileStream == null)
                        return null;

                    int bytesOffset = 0;
                    int bytesRead = 0;
                    var contentBytesArray = new byte[fileStream.Length];

                    while ((bytesRead = fileStream.Read(contentBytesArray, bytesOffset,
                                                        contentBytesArray.Length - bytesRead)) > 0)
                        bytesOffset += bytesRead;

                    return contentBytesArray;

                }

            }
            catch (FileNotFoundException exception)
            {

                Diagnostics.Debug.WriteLine(exception.StackTrace);
                return null;

            }
            catch (IOException exception)
            {

                Diagnostics.Debug.WriteLine(exception.StackTrace);
                return null;

            }

        }

        public async Task<byte[]> GetContentsAsync(string fileNameString)
        {

            try
            {

                var fileContentsArray = await Task.Run(async () =>
               {

                   await _fileSemaphore.WaitAsync();

                   var filePathString = PrepareFilePath(fileNameString);
                   if (string.IsNullOrEmpty(filePathString))
                   {

                       _fileSemaphore.Release();
                       return null;

                   }

                   using (var fileStream = File.OpenRead(filePathString))
                   {

                       if (fileStream == null)
                           return null;

                       int bytesOffset = 0;
                       int bytesRead = 0;
                       var contentBytesArray = new byte[fileStream.Length];

                       while ((bytesRead = await fileStream.ReadAsync(contentBytesArray, bytesOffset,
                                                                      contentBytesArray.Length - bytesRead)) > 0)
                       {

                           bytesOffset += bytesRead;

                       }

                       _fileSemaphore.Release();
                       return contentBytesArray;

                   }

               });

                return fileContentsArray;

            }
            catch (FileNotFoundException exception)
            {

                Diagnostics.Debug.WriteLine(exception.StackTrace);
                _fileSemaphore.Release();
                return null;

            }
            catch (IOException exception)
            {

                Diagnostics.Debug.WriteLine(exception.StackTrace);
                _fileSemaphore.Release();
                return null;

            }
            catch (Exception exception)
            {

                Diagnostics.Debug.WriteLine(exception.StackTrace);
                _fileSemaphore.Release();
                return null;

            }

        }

        public bool CreateFile(string fileNameString, byte[] contentBytesArray, FileCreationType creationType)
        {

            try
            {

                if ((contentBytesArray == null) || (contentBytesArray.Length == 0))
                    return false;

                var filePathString = PrepareFilePath(fileNameString);
                if (string.IsNullOrEmpty(filePathString))
                    return false;

                var fileStreamInfo = PrepareFileStream(filePathString, contentBytesArray, creationType);
                if (fileStreamInfo == null)
                    return false;

                using (var fileStream = fileStreamInfo.Item1)
                {

                    var streamBytesArray = fileStreamInfo.Item2;
                    if ((streamBytesArray == null) || (streamBytesArray.Length == 0))
                        return false;

                    fileStream.Write(streamBytesArray, 0, streamBytesArray.Length);

                }

                return true;

            }
            catch (DirectoryNotFoundException exception)
            {

                Diagnostics.Debug.WriteLine(exception.StackTrace);
                return false;

            }
            catch (IOException exception)
            {

                Diagnostics.Debug.WriteLine(exception.StackTrace);
                return false;

            }

        }

        public async Task<bool> CreateFileAsync(string fileNameString, byte[] contentBytesArray,
                                                FileCreationType creationType)
        {

            try
            {

                if ((contentBytesArray == null) || (contentBytesArray.Length == 0))
                    return false;

                await _fileSemaphore.WaitAsync();

                var filePathString = PrepareFilePath(fileNameString);
                if (string.IsNullOrEmpty(filePathString))
                {

                    _fileSemaphore.Release();
                    return false;

                }

                var fileStreamInfo = PrepareFileStream(filePathString, contentBytesArray, creationType);
                if (fileStreamInfo == null)
                {

                    _fileSemaphore.Release();
                    return false;

                }

                using (var fileStream = fileStreamInfo.Item1)
                {

                    var streamBytesArray = fileStreamInfo.Item2;
                    if ((streamBytesArray == null) || (streamBytesArray.Length == 0))
                        return false;

                    await fileStreamInfo.Item1.WriteAsync(fileStreamInfo.Item2, 0, fileStreamInfo.Item2.Length);
                    _fileSemaphore.Release();

                }

                return true;

            }
            catch (DirectoryNotFoundException exception)
            {

                Diagnostics.Debug.WriteLine(exception.StackTrace);
                _fileSemaphore.Release();
                return false;

            }
            catch (IOException exception)
            {

                Diagnostics.Debug.WriteLine(exception.StackTrace);
                _fileSemaphore.Release();
                return false;

            }
            catch (Exception exception)
            {

                Diagnostics.Debug.WriteLine(exception.StackTrace);
                _fileSemaphore.Release();
                return false;

            }

        }

        public bool RemoveFile(string fileNameString)
        {

            try
            {

                var filePathString = PrepareFilePath(fileNameString);
                if (string.IsNullOrEmpty(filePathString))
                    return false;

                File.Delete(filePathString);
                return DoesFileExist(fileNameString);

            }
            catch (FileNotFoundException exception)
            {

                Diagnostics.Debug.WriteLine(exception.StackTrace);
                return false;

            }
            catch (Exception exception)
            {

                Diagnostics.Debug.WriteLine(exception.StackTrace);
                return false;

            }
        }

        public async Task<bool> RemoveFileAsync(string fileNameString)
        {

            try
            {

                var couldDelete = await Task.Run(async () =>
                {

                    await _fileSemaphore.WaitAsync();

                    var filePathString = PrepareFilePath(fileNameString);
                    if (string.IsNullOrEmpty(filePathString))
                    {

                        _fileSemaphore.Release();
                        return false;

                    }

                    File.Delete(filePathString);
                    _fileSemaphore.Release();
                    return !DoesFileExist(fileNameString);

                });

                return couldDelete;

            }
            catch (FileNotFoundException exception)
            {

                Diagnostics.Debug.WriteLine(exception.StackTrace);
                _fileSemaphore.Release();
                return false;

            }
            catch (Exception exception)
            {

                Diagnostics.Debug.WriteLine(exception.StackTrace);
                _fileSemaphore.Release();
                return false;

            }

        }

        public bool CheckExpiry(string fileNameString, int days, int months, int years, int hours, int minutes,
                                int seconds)
        {

            try
            {

                var filePathString = PrepareFilePath(fileNameString);
                if (string.IsNullOrEmpty(filePathString))
                    return false;

                DateTime lastWriteDateTime = File.GetLastWriteTime(filePathString);
                DateTime currentDateTime = DateTime.Now;
                DateTime expiryDateTime = lastWriteDateTime.AddDays(days)
                                                           .AddMonths(months)
                                                           .AddYears(years)
                                                           .AddHours(hours)
                                                           .AddMinutes(minutes)
                                                           .AddSeconds(seconds);
                int result = expiryDateTime.CompareTo(currentDateTime);
                return (result < 0);

            }
            catch (FileNotFoundException exception)
            {

                Diagnostics.Debug.WriteLine(exception.StackTrace);
                return false;

            }
            catch (Exception exception)
            {

                Diagnostics.Debug.WriteLine(exception.StackTrace);
                return false;

            }

        }

        public bool CheckExpiry(string fileNameString, double timeInMilliSeconds)
        {

            try
            {

                var filePathString = PrepareFilePath(fileNameString);
                if (string.IsNullOrEmpty(filePathString))
                    return false;

                DateTime lastWriteDateTime = File.GetLastWriteTime(filePathString);
                DateTime currentDateTime = DateTime.Now;
                DateTime expiryDateTime = lastWriteDateTime.AddMilliseconds(timeInMilliSeconds);
                int result = expiryDateTime.CompareTo(currentDateTime);
                return (result < 0);

            }
            catch (FileNotFoundException exception)
            {

                Diagnostics.Debug.WriteLine(exception.StackTrace);
                return false;

            }
            catch (Exception exception)
            {

                Diagnostics.Debug.WriteLine(exception.StackTrace);
                return false;

            }

        }

        public FileInfo GetLRUFileItem(string folderPathString)
        {

            try
            {

                List<string> fileNamesList = Directory.EnumerateFiles(folderPathString, "*",
                                                                      SearchOption.AllDirectories).ToList();
                int maxMilliSeconds = 0;
                int maxIndex = 0;
                int index = 0;

                foreach (var fileNameString in fileNamesList)
                {

                    DateTime lastWriteDateTime = File.GetLastWriteTime(fileNameString);
                    DateTime currentDateTime = DateTime.Now;
                    TimeSpan diffTimeSpan = currentDateTime - lastWriteDateTime;
                    int diffMilliSeconds = (int)(diffTimeSpan.TotalMilliseconds);

                    if (diffMilliSeconds > maxMilliSeconds)
                    {
                        maxMilliSeconds = diffMilliSeconds;
                        maxIndex = index;
                    }

                    ++index;

                }

                var lruFileNameString = fileNamesList[maxIndex];
                FileInfo lruFileInfo = new FileInfo(lruFileNameString);
                return lruFileInfo;

            }
            catch (DirectoryNotFoundException exception)
            {

                Diagnostics.Debug.WriteLine(exception.StackTrace);
                return null;

            }
            catch (IOException exception)
            {

                Diagnostics.Debug.WriteLine(exception.StackTrace);
                return null;

            }
        }

        public long GetFolderSize(string folderPathString)
        {

            try
            {

                var directoryInfo = new DirectoryInfo(folderPathString);
                if (directoryInfo == null)
                    return -1;

                FileInfo[] fileInfoArray = directoryInfo.GetFiles();
                if ((fileInfoArray == null) || (fileInfoArray.Length == 0))
                    return -1;

                long totalsize = 0;
                foreach (var file in fileInfoArray)
                    totalsize += file.Length;

                DirectoryInfo[] dirInfoArray = directoryInfo.GetDirectories();
                if ((dirInfoArray == null) || (dirInfoArray.Length == 0))
                    return totalsize;

                foreach (var directory in dirInfoArray)
                    totalsize += GetFolderSize(directory.FullName);

                return totalsize;

            }
            catch (DirectoryNotFoundException exception)
            {

                Diagnostics.Debug.WriteLine(exception.StackTrace);
                return -1;

            }
            catch (IOException exception)
            {

                Diagnostics.Debug.WriteLine(exception.StackTrace);
                return -1;

            }
        }

        public long AdjustFolderSizeWithLRULimit(long sizeLimitInKB)
        {

            if (string.IsNullOrEmpty(_folderPathString) == true)
                return -1;

            try
            {
                
                long folderSize = GetFolderSize(_folderPathString);
                while (folderSize > sizeLimitInKB * kSizeInKB)
                {

                    FileInfo lruItemInfo = GetLRUFileItem(_folderPathString);
                    File.Delete(lruItemInfo.FullName);
                    folderSize = GetFolderSize(_folderPathString);

                }

                return folderSize;

            }
            catch (DirectoryNotFoundException exception)
            {

                Diagnostics.Debug.WriteLine(exception.StackTrace);
                return -1;

            }
            catch (IOException exception)
            {

                Diagnostics.Debug.WriteLine(exception.StackTrace);
                return -1;

            }
        }
        #endregion

    }
}

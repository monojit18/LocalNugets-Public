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

using System.Threading.Tasks;
using System.IO;
using Subsystems.CustomFileManager.Internal;

namespace Subsystems.CustomFileManager.External
{

    public enum FileCreationType
    {

        Overwrite = 1,
        Append,
        Copy

    };

    public class CMPFileManagerProxy
    {

        #region Private Variables
        private readonly CMPFileManager _fileManager;
        #endregion

        #region Public Methods
        public static bool PrepareFoldePath(string folderPathString)
        {

            if (string.IsNullOrEmpty(folderPathString) == true)
                return false;

            var couldPrepare = CMPFileManager.PrepareFoldePath(folderPathString);
            return couldPrepare;

        }

        public CMPFileManagerProxy(string folderPathString)
        {

            if (string.IsNullOrEmpty(folderPathString) == true)
                return;

            _fileManager = new CMPFileManager(folderPathString);

        }

        public bool DoesFileExist(string fileNameString) => (_fileManager.DoesFileExist(fileNameString));

        public byte[] GetContents(string fileNameString) => (_fileManager.GetContents(fileNameString));

        public async Task<byte[]> GetContentsAsync(string fileNameString) => (await _fileManager.GetContentsAsync(fileNameString));


        public bool CreateFile(string fileNameString, byte[] contentBytes, FileCreationType creationType) =>
        (_fileManager.CreateFile(fileNameString, contentBytes, creationType));

        public async Task<bool> CreateFileAsync(string fileNameString, byte[] contentBytes,
                                                FileCreationType creationType) =>
        (await _fileManager.CreateFileAsync(fileNameString, contentBytes, creationType));

        public bool RemoveFile(string fileNameString) => (_fileManager.RemoveFile(fileNameString));

        public async Task<bool> RemoveFileAsync(string fileNameString) => (await _fileManager.RemoveFileAsync(fileNameString));

        public bool CheckExpiry(string fileNameString, int days, int months, int years, int hours, int minutes,
                                int seconds) => (_fileManager.CheckExpiry(fileNameString, days, months, years, hours,
                                                                          minutes, seconds));

        public bool CheckExpiry(string fileNameString, double timeInMilliSeconds) =>
        (_fileManager.CheckExpiry(fileNameString, timeInMilliSeconds));

        public FileInfo GetLRUFileItem(string folderNameString) => (_fileManager.GetLRUFileItem(folderNameString));

        public long GetFolderSize(string folderNameString) => (_fileManager.GetFolderSize(folderNameString));

        public long AdjustFolderSizeWithLRULimit(long sizeLimitInKB) =>
        (_fileManager.AdjustFolderSizeWithLRULimit(sizeLimitInKB));
        #endregion
    }
}


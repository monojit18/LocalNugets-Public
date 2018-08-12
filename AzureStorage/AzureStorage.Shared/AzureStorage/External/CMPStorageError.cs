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

namespace Subsystems.AzureStorage.External
{
    public class CMPStorageError
    {

        public string CodeString { get; set; }
        public string MessageString { get; set; }
        public string DescriptionString { get; set; }

        public static CMPStorageError CreateError(string errorCodeString, string errorMessageString)
        {

            var error = new CMPStorageError()
            {

                CodeString = errorCodeString,
                MessageString = errorMessageString,
                DescriptionString = errorMessageString

            };

            return error;

        }

        public static CMPStorageError CreateErrorFromException(Exception exception)
        {
            
            var error = new CMPStorageError()
            {

                CodeString = String.Empty,
                MessageString = exception?.Message,
                DescriptionString = exception?.InnerException?.Message

            };

            return error;

        }

    }
}

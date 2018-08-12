using System;
using Diagonostics = System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Foundation;
using AVFoundation;
using Speech;

namespace Subsystems.CustomSpeechToText.iOS.External
{

    public enum SpeechToTextErrorEnum
    {

        eNoInputNode,
        eCanNotStartAudioEngine

    };

    public class CMPSpeechToTextIOS
    {

        private const string KDeniedAuthoriztionString = "Denied";
        private const string KRestrictedAuthoriztionString = "Restricted";
        private const string KNotDeterminedAuthoriztionString = "NotDetermined";
        private const string KAuthorizedString = "Authorized";
        private const string KErrorDomainString = ".SpeechToText";
        private const string KBundleIdentifierString = "CFBundleIdentifier";
        private const string KNoInputNodeErrorString = "NoInputNode";
        private const string KCanNotStartAudioEngineErrorString = "CanNotStartAudioEngine";

        private SFSpeechRecognizer _speechRecognizer;
        private SFSpeechAudioBufferRecognitionRequest _speechRecognitionRequest;
        private SFSpeechRecognitionTask _speechRecognitionTask;
        private AVAudioEngine _speechAudioEngine;
        private SemaphoreSlim _speechSemaphore;

        private static string GetStatusMessageString(SFSpeechRecognizerAuthorizationStatus status)
        {

            switch(status)
            {

                case SFSpeechRecognizerAuthorizationStatus.Denied:
                return KDeniedAuthoriztionString;

                case SFSpeechRecognizerAuthorizationStatus.Restricted:
                return KRestrictedAuthoriztionString;

                case SFSpeechRecognizerAuthorizationStatus.NotDetermined:
                return KNotDeterminedAuthoriztionString;
                    
            }

            return KAuthorizedString;

        }

        private static string GetErrorMessageString(SpeechToTextErrorEnum errorEnum)
        {

            switch (errorEnum)
            {

                case SpeechToTextErrorEnum.eNoInputNode:
                return KNoInputNodeErrorString;

                case SpeechToTextErrorEnum.eCanNotStartAudioEngine:
                return KCanNotStartAudioEngineErrorString;
                    
            }

            return null;

        }

		private static string GetErrorDomain()
        {

            var bundleIdentifierString = NSBundle.MainBundle.InfoDictionary[KBundleIdentifierString] as NSString;
            var errorDomainString = bundleIdentifierString.ToString() + KErrorDomainString;
            return errorDomainString;

        }

		private CMPSpeechError PrepareAuthorizationError(SFSpeechRecognizerAuthorizationStatus status)
        {

			string errorDomainString = GetErrorDomain ();
			string erorMessageString = GetStatusMessageString (status);

			var genericError = new CMPSpeechError ();
			genericError.Code = (int)(status);
			genericError.Domain = GetErrorDomain ();
			genericError.Message = erorMessageString;
            return genericError;

        }

		private CMPSpeechError PrepareAudioEngineError(NSError recognitionError)
        {

			var genericError = new CMPSpeechError (recognitionError);
            genericError.Code = (int)(recognitionError.Code);
            genericError.Domain = GetErrorDomain();
            genericError.Message = recognitionError.LocalizedDescription;
            return genericError;

        }

		private void ResetSpeechToText ()
		{

			_speechRecognizer = new SFSpeechRecognizer ();
			_speechAudioEngine = new AVAudioEngine ();

			_speechRecognitionRequest = new SFSpeechAudioBufferRecognitionRequest ();
			_speechRecognitionRequest.ShouldReportPartialResults = true;
            _speechRecognitionRequest.TaskHint = SFSpeechRecognitionTaskHint.Search;
            _speechRecognitionRequest.ContextualStrings = new string[] { "for", "the", "a", "an"};

			_speechRecognitionTask = null;

		}

		private async Task<Tuple<bool, CMPSpeechError>> CheckAuthorizationAsync()
        {

            Tuple<bool, CMPSpeechError> authorizationStatus = null;
            await Task.Run(() => 
            {

                try
                {

                    SFSpeechRecognizer.RequestAuthorization((SFSpeechRecognizerAuthorizationStatus status) =>
                    {

                        if (status != SFSpeechRecognizerAuthorizationStatus.Authorized)
                        {

                            var genericError = PrepareAuthorizationError(status);
                            authorizationStatus = new Tuple<bool, CMPSpeechError>(false, genericError);
                            return;

                        }

                        authorizationStatus = new Tuple<bool, CMPSpeechError>(true, null);
                        _speechSemaphore.Release();

                    });

                }
                catch(Exception exception)
                {

                    Diagonostics.Debug.WriteLine(exception.Message);
                    _speechSemaphore.Release();

                }


            });

            await _speechSemaphore.WaitAsync();
            return authorizationStatus;

        }

        public CMPSpeechToTextIOS()
        {

            ResetSpeechToText();
			_speechSemaphore = new SemaphoreSlim (0);

        }

        public bool IsRecording()
        {

            if (_speechAudioEngine == null)
                return false;

            return (_speechAudioEngine.Running);

        }

		public async Task<Tuple<string, Tuple<bool, CMPSpeechError>>> StartRecordingAsync()
        {

			if (IsRecording () == true)
				return new Tuple<string, Tuple<bool, CMPSpeechError>> (string.Empty, null);

            var authorizationResult = await CheckAuthorizationAsync();
            if (authorizationResult == null)
                return new Tuple<string, Tuple<bool, CMPSpeechError>>(string.Empty, null);

            if (authorizationResult.Item1 == false)
                return new Tuple<string, Tuple<bool, CMPSpeechError>>(string.Empty, authorizationResult);

            CMPSpeechError genericError = null;
            var inputNode = _speechAudioEngine.InputNode;

            if (inputNode == null)
            {

				var audioEngineError = new NSError (new NSString(string.Empty), nint.Parse(SpeechToTextErrorEnum.
                                                                                           eNoInputNode.ToString()));
				genericError = PrepareAudioEngineError(audioEngineError);
				ResetSpeechToText();
                return (new Tuple<string, Tuple<bool, CMPSpeechError>>(string.Empty, new Tuple<bool, CMPSpeechError>
                                                                       (false, genericError)));

            }

			Tuple<string, Tuple<bool, CMPSpeechError>> recognitionResult = null;
            await Task.Run(() =>
            {

                try
                {

                    _speechRecognitionTask = _speechRecognizer.GetRecognitionTask(_speechRecognitionRequest,
                                                                                  (SFSpeechRecognitionResult result,
                                                                                   NSError speechError) =>
                      {

                          if (speechError != null)
                          {

                              _speechAudioEngine.Stop();

                              genericError = new CMPSpeechError(speechError);
                              recognitionResult = new Tuple<string, Tuple<bool, CMPSpeechError>>(string.Empty,
                                                                          new Tuple<bool, CMPSpeechError>(false,
                                                                                                          genericError));
                              ResetSpeechToText();
                              _speechSemaphore.Release();
                              return;

                          }

                          if (result.Final == true)
                          {

                              _speechAudioEngine.Stop();
                              inputNode.RemoveTapOnBus(0);

                              recognitionResult = new Tuple<string, Tuple<bool, CMPSpeechError>>(result.BestTranscription.
                                                                                                 FormattedString,
                                                                                                 new Tuple<bool,
                                                                                                 CMPSpeechError>(true,
                                                                                                                 null));

                              ResetSpeechToText();
                              _speechSemaphore.Release();
                              return;

                          }

                      });

                    var audioFormat = inputNode.GetBusOutputFormat(0);
                    inputNode.InstallTapOnBus(0, 2048, audioFormat, (AVAudioPcmBuffer buffer, AVAudioTime when) =>
                    {

                        var state = _speechRecognitionTask.State;
                        _speechRecognitionRequest.Append(buffer);

                    });

                    _speechAudioEngine.Prepare();

                    NSError audioEngineError = null;
                    bool couldStart = _speechAudioEngine.StartAndReturnError(out audioEngineError);

                    if (couldStart == false)
                    {

                        genericError = PrepareAudioEngineError(audioEngineError);
                        recognitionResult = new Tuple<string, Tuple<bool, CMPSpeechError>>(string.Empty,
                                                                        new Tuple<bool, CMPSpeechError>(false,
                                                                                                        genericError));
                        ResetSpeechToText();
                        _speechSemaphore.Release();
                        return;

                    }

                }
                catch(Exception exception)
                {

                    Diagonostics.Debug.WriteLine(exception.Message);
                    ResetSpeechToText();
                    _speechSemaphore.Release();

                }


            });

            await _speechSemaphore.WaitAsync ();
            return recognitionResult;

        }

		public bool StopRecording()
        {

            if (IsRecording() == false)
				return false;

            if (_speechRecognitionRequest != null)
                _speechRecognitionRequest.EndAudio();

			return true;

        }
    }
}

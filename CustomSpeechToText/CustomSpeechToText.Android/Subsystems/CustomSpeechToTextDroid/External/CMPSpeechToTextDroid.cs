using System;
using Java.Util;
using Android.Content;
using Android.Content.PM;
using Android.Speech;

namespace Subsystems.CustomSpeechToText.Droid.External
{

	public enum SpeechToTextErrorEnum
	{

		eNoMicrophone,
		eSpeechNotRecognized

	};

	public class CMPSpeechToTextDroid
	{

		private const string KMicrophoneString = "android.hardware.microphone";
		private const string KNoMicrophoneString = "No Microphone";
		private const string KSpeechNotRecognizedString = "Speech not recognized";
		private const string KSpeechPromptString = "Please speak to record";
		private const string KErrorDomainString = ".SpeechToText";
		private string _packageNameString;

		private static string GetErrorMessageString (SpeechToTextErrorEnum errorEnum)
		{

			switch (errorEnum) {

			case SpeechToTextErrorEnum.eNoMicrophone:
				return KNoMicrophoneString;

			case SpeechToTextErrorEnum.eSpeechNotRecognized:
				return KSpeechNotRecognizedString;

			}

			return null;

		}

		private string GetErrorDomain ()
		{

			var errorDomainString = _packageNameString + KErrorDomainString;
			return errorDomainString;

		}

		private CMPSpeechError PrepareSpeechToTextError (SpeechToTextErrorEnum errorEnum)
		{

			var genericError = new CMPSpeechError ();
			genericError.Code = (int)errorEnum;
			genericError.Domain = GetErrorDomain ();
			genericError.Message = GetErrorMessageString (errorEnum);
			return genericError;

		}

		private bool CheckMicrophone ()
		{

			string featureString = PackageManager.FeatureMicrophone;
			return (featureString == KMicrophoneString);

		}

		public Locale SpeechLocale { get; set; }
		public string SpeechDialogPrompt { get; set; }

		public CMPSpeechToTextDroid(string packageNameString)
		{

			_packageNameString = string.Copy (packageNameString);
			SpeechLocale = Locale.Default;
			SpeechDialogPrompt = KSpeechPromptString;

		}

		public Intent GetSpeechIntent()
		{

			bool hasMicrophone = CheckMicrophone ();
			if (hasMicrophone == false)
				return null;

			var voiceIntent = new Intent (RecognizerIntent.ActionRecognizeSpeech);
            voiceIntent.PutExtra (RecognizerIntent.ExtraLanguage, SpeechLocale);
			voiceIntent.PutExtra (RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
			voiceIntent.PutExtra (RecognizerIntent.ExtraPrompt, SpeechDialogPrompt);
			return voiceIntent;

		}

		public Tuple<string, CMPSpeechError> ProcessSpeechOutput(Intent speechIntent)
		{

			var speechOutputList = speechIntent.GetStringArrayListExtra (RecognizerIntent.ExtraResults);
			if (speechOutputList == null || speechOutputList.Count == 0)
			{

				var genericError = PrepareSpeechToTextError (SpeechToTextErrorEnum.eSpeechNotRecognized);
				return new Tuple<string, CMPSpeechError> (string.Empty, genericError);

			}

			var speechOutputString = speechOutputList[0] as string;
			return new Tuple<string, CMPSpeechError> (speechOutputString, null);

		}
	}
}

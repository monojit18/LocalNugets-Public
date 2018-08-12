using System;
using Foundation;

namespace Subsystems.CustomSpeechToText.iOS.External
{
	public class CMPSpeechError
	{

		public int Code { get; set; }
		public string Domain { get; set; }
		public string Message { get; set; }

		public CMPSpeechError () {}

		public CMPSpeechError(NSError error)
		{

			Code = (int)(error.Code);
			Domain = error.Domain;
			Message = error.Description;

		}

    }
}

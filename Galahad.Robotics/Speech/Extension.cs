using System;

namespace Galahad.Robotics.Speech
{
    internal static class Extension
    {
        public static Galatea.Speech.Gender Convert(this Windows.Media.SpeechSynthesis.VoiceGender value)
        {
            switch (value)
            {
                case Windows.Media.SpeechSynthesis.VoiceGender.Male: return Galatea.Speech.Gender.Male;
                case Windows.Media.SpeechSynthesis.VoiceGender.Female: return Galatea.Speech.Gender.Female;
                default: throw new NotImplementedException();
            }
        }

        public static Galatea.Speech.SpeechRecognitionStatus Convert(this Windows.Media.SpeechRecognition.SpeechRecognitionResultStatus value)
        {
            switch (value)
            {
                case Windows.Media.SpeechRecognition.SpeechRecognitionResultStatus.Success:
                    return Galatea.Speech.SpeechRecognitionStatus.Success;
                case Windows.Media.SpeechRecognition.SpeechRecognitionResultStatus.UserCanceled:
                    return Galatea.Speech.SpeechRecognitionStatus.UserCanceled;
                case Windows.Media.SpeechRecognition.SpeechRecognitionResultStatus.MicrophoneUnavailable:
                    return Galatea.Speech.SpeechRecognitionStatus.MicrophoneUnavailable;
                default: return Galatea.Speech.SpeechRecognitionStatus.Unknown;
            }
        }
    }
}

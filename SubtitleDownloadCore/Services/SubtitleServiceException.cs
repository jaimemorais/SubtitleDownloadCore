using System;

namespace SubtitleDownloadCore.Services
{
    [System.Serializable]
    public class SubtitleServiceException : Exception
    {
        public SubtitleServiceException() { }
        public SubtitleServiceException(string message) : base(message) { }
        public SubtitleServiceException(string message, Exception inner) : base(message, inner) { }
        protected SubtitleServiceException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

}

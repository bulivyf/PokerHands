using System;
using System.Runtime.Serialization;

namespace ProjectEulerSolutions
{
    [Serializable]
    internal class IllegalNumberOfCards : Exception
    {
        public IllegalNumberOfCards()
        {
        }

        public IllegalNumberOfCards(string message) : base(message)
        {
        }

        public IllegalNumberOfCards(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected IllegalNumberOfCards(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Runtime.Serialization;

namespace EdFi.Dashboards.Common
{
	[Serializable]
	public class ConventionsException : Exception
	{
		//
		// For guidelines regarding the creation of new exception types, see
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
		// and
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
		//

		public ConventionsException()
		{
		}

		public ConventionsException(string message) : base(message)
		{
		}

		public ConventionsException(string message, Exception inner) : base(message, inner)
		{
		}

		protected ConventionsException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{
		}
	}

	[Serializable]
	public class SecurityConfigurationException : Exception
	{
		//
		// For guidelines regarding the creation of new exception types, see
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
		// and
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
		//

		public SecurityConfigurationException()
		{
		}

		public SecurityConfigurationException(string message) : base(message)
		{
		}

		public SecurityConfigurationException(string message, Exception inner) : base(message, inner)
		{
		}

		protected SecurityConfigurationException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{
		}
	}

	[Serializable]
	public class SessionExpiredException : Exception
	{
		//
		// For guidelines regarding the creation of new exception types, see
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
		// and
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
		//

		public SessionExpiredException()
		{
		}

		public SessionExpiredException(string message) : base(message)
		{
		}

		public SessionExpiredException(string message, Exception inner) : base(message, inner)
		{
		}

		protected SessionExpiredException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{
		}
	}

	/// <summary>
	/// Indicates that the operation attempted by the user has been denied, and the message is
	/// a user-facing message.
	/// </summary>
	[Serializable]
	public class UserAccessDeniedException : Exception
	{
		//
		// For guidelines regarding the creation of new exception types, see
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
		// and
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
		//

		public UserAccessDeniedException()
		{
		}

		public UserAccessDeniedException(string message) : base(message)
		{
		}

		public UserAccessDeniedException(string message, Exception inner) : base(message, inner)
		{
		}

		protected UserAccessDeniedException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{
		}
	}

	[Serializable]
	public class UnhandledSignatureException : Exception
	{
		//
		// For guidelines regarding the creation of new exception types, see
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
		// and
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
		//

		/// <summary>
		/// Gets whether the parameter combination was explicitly unhandled, that is, support has not been implemented because 
		/// methods with the supplied parameter combination should never be called by a user in the given role.
		/// </summary>
		public bool ExplicitlyUnhandled { get; private set; }

		/// <summary>
		/// Constructs and initializes a new instance of <see cref="UnhandledSignatureException"/> indicating whether the unhandled combination
		/// was explicitly unhandled as being (not an error condition for testing security coverage).
		/// </summary>
		/// <param name="explicitlyUnhandled">Indicates whether the parameter combination was explicitly unhandled, that is, support has not been implemented because 
		/// methods with the supplied parameter combination should never be called by a user in the given role.</param>
		/// <param name="message">Message associated with the exception.</param>
		public UnhandledSignatureException(bool explicitlyUnhandled, string message) : base(message)
		{
			ExplicitlyUnhandled = explicitlyUnhandled;
		}

		public UnhandledSignatureException(bool explicitlyUnhandled, string message, Exception inner)
			: base(message, inner)
		{
			ExplicitlyUnhandled = explicitlyUnhandled;
		}

        protected UnhandledSignatureException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ExplicitlyUnhandled = info.GetBoolean("ExplicitlyUnhandled");
        }

		//protected UnhandledSignatureException(
		//    SerializationInfo info,
		//    StreamingContext context) : base(info, context)
		//{
		//}

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ExplicitlyUnhandled", ExplicitlyUnhandled);
        }
    }

	/// <summary>
	/// Indicates that the operation attempted by the user has been denied, and the message is
	/// a user-facing message.
	/// </summary>
	[Serializable]
	public class ControllerNotFoundException : Exception
	{
		//
		// For guidelines regarding the creation of new exception types, see
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
		// and
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
		//

		public ControllerNotFoundException()
		{
		}

		public ControllerNotFoundException(string message) : base(message)
		{
		}

		public ControllerNotFoundException(string message, Exception inner) : base(message, inner)
		{
		}

        protected ControllerNotFoundException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{
		}
	}
}

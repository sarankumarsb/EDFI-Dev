// *************************************************************************
// Copyright (C) 2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Runtime.Serialization;

namespace EdFi.Dashboards.Resources.Models.Application
{
    [Serializable]
    public class ExceptionModel
    {
        public ExceptionModel() {}
		
        public ExceptionModel(Exception ex)
        {
            Message = ex.Message;
            StackTrace = ex.ToString();
            Type = ex.GetType();

            if (ex.InnerException != null)
            {
                InnerExceptionMessage = ex.InnerException.Message;
                InnerExceptionStackTrace = ex.InnerException.ToString();
            }
        }

        public string Message { get; set; }
        public string StackTrace { get; set; }
        
        [IgnoreDataMember] // Shouldn't serialize entire type, but perhaps type name as a string?
        public Type Type { get; set; }
        
        public string InnerExceptionMessage { get; set; }
        
        public string InnerExceptionStackTrace { get; set; }
    }
}
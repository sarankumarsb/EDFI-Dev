// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Linq;

namespace EdFi.Dashboards.Resources.Models.Student
{
    public interface IStudent
    {
        long StudentUSI { get; }
    }

    //public class StudentCopyInstanceProvider : IInstanceProvider
    //{
    //    public Type Provided
    //    {
    //        get { return typeof(IStudent); }
    //    }

    //    public bool CanCreateCopy(Type type)
    //    {
    //        return (type.GetInterfaces().Contains(typeof (IStudent)) && type.GetConstructor(new[] {typeof (int)}) != null);
    //    }

    //    public object CreateCopy(object toBeCopied)
    //    {
    //        var constructor = toBeCopied.GetType().GetConstructor(new[] {typeof (int)});
    //        var studentToBeCopied = toBeCopied as IStudent;
    //        return constructor != null && studentToBeCopied != null ? constructor.Invoke(new object[] {studentToBeCopied.StudentUSI}) : null;
    //    }

    //}
}

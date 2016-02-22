// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Security.Tests.StudentFilter
{
    #region Test Service
    public interface ITestService
    {
        NoStudentData GetNoStudentData(string filler);
        IEnumerable<NoStudentData> GetNoStudentDataEnumerable(IEnumerable<NoStudentData> data);
        IEnumerable<NoStudentData> GetNoStudentDataQueryable(IEnumerable<NoStudentData> data);
        List<NoStudentData> GetNoStudentDataList(IEnumerable<NoStudentData> data);

        StudentData GetStudentData(long studentUSI, string filler);
        IEnumerable<StudentData> GetStudentDataEnumerable(IEnumerable<StudentData> data);
        IEnumerable<StudentData> GetStudentDataQueryable(IEnumerable<StudentData> data);
        List<StudentData> GetStudentDataList(IEnumerable<StudentData> data);

        NestedStudentData GetNestedStudentData(long studentUSI, string filler, string filler2);
        IEnumerable<NestedStudentData> GetNestedStudentDataEnumerable(IEnumerable<NestedStudentData> data);
        IEnumerable<NestedStudentData> GetNestedStudentDataQueryable(IEnumerable<NestedStudentData> data);
        List<NestedStudentData> GetNestedStudentDataList(IEnumerable<NestedStudentData> data);

        NestedStudentDataCollection GetNestedStudentDataCollection(List<StudentData> studentData, string filler2);
        IEnumerable<NestedStudentDataCollection> GetNestedStudentDataCollectionEnumerable(IEnumerable<NestedStudentDataCollection> data);
        IEnumerable<NestedStudentDataCollection> GetNestedStudentDataCollectionQueryable(IEnumerable<NestedStudentDataCollection> data);
        List<NestedStudentDataCollection> GetNestedStudentDataCollectionList(IEnumerable<NestedStudentDataCollection> data);

        StudentDataList GetStudentDataList(StudentDataList data);
    }

    public class TestService : ITestService
    {
        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public NoStudentData GetNoStudentData(string filler)
        {
            return new NoStudentData {Filler = filler};
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public IEnumerable<NoStudentData> GetNoStudentDataEnumerable(IEnumerable<NoStudentData> data)
        {
            return data;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public IEnumerable<NoStudentData> GetNoStudentDataQueryable(IEnumerable<NoStudentData> data)
        {
            return data.AsQueryable();
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public List<NoStudentData> GetNoStudentDataList(IEnumerable<NoStudentData> data)
        {
            return data.ToList();
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public StudentData GetStudentData(long studentUSI, string filler)
        {
            return new StudentData {StudentUSI = studentUSI, Filler = filler};
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public IEnumerable<StudentData> GetStudentDataEnumerable(IEnumerable<StudentData> data)
        {
            return data;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public IEnumerable<StudentData> GetStudentDataQueryable(IEnumerable<StudentData> data)
        {
            return data.AsQueryable();
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public List<StudentData> GetStudentDataList(IEnumerable<StudentData> data)
        {
            return data.ToList();
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public NestedStudentData GetNestedStudentData(long studentUSI, string filler, string filler2)
        {
            return new NestedStudentData {StudentData = new StudentData {StudentUSI = studentUSI, Filler = filler}, Filler = filler2};
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public IEnumerable<NestedStudentData> GetNestedStudentDataEnumerable(IEnumerable<NestedStudentData> data)
        {
            return data;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public IEnumerable<NestedStudentData> GetNestedStudentDataQueryable(IEnumerable<NestedStudentData> data)
        {
            return data.AsQueryable();
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public List<NestedStudentData> GetNestedStudentDataList(IEnumerable<NestedStudentData> data)
        {
            return data.ToList();
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public NestedStudentDataCollection GetNestedStudentDataCollection(List<StudentData> studentData, string filler2)
        {
            return new NestedStudentDataCollection {StudentData = studentData, Filler = filler2};
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public IEnumerable<NestedStudentDataCollection> GetNestedStudentDataCollectionEnumerable(IEnumerable<NestedStudentDataCollection> data)
        {
            return data;   
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public IEnumerable<NestedStudentDataCollection> GetNestedStudentDataCollectionQueryable(IEnumerable<NestedStudentDataCollection> data)
        {
            return data.AsQueryable();
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public List<NestedStudentDataCollection> GetNestedStudentDataCollectionList(IEnumerable<NestedStudentDataCollection> data)
        {
            return data.ToList();
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public StudentDataList GetStudentDataList(StudentDataList data)
        {
            return data;
        }
    }
    #endregion

    #region Test Models

    public class NoStudentData
    {
        public string Filler { get; set; }
    }

    public class StudentData : IStudent
    {
        public long StudentUSI { get; set; }
        public string Filler { get; set; }
    }

    public class NestedStudentData
    {
        public StudentData StudentData { get; set; }
        public string Filler { get; set; }
    }

    public class NestedStudentDataCollection
    {
        public List<StudentData> StudentData { get; set; }
        public string Filler { get; set; }
    }

    public class StudentDataList : List<StudentData>
    {
        public StudentDataList()
        {
        }

        public StudentDataList(StudentDataList studentDataList, IEnumerable<StudentData> collection) : base(collection)
        {
            Filler = studentDataList.Filler;
        }

        public string Filler { get; set; }
    }

    #endregion


}

// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Resources.Navigation.Mvc.Areas;

namespace EdFi.Dashboards.Resources.Navigation.Mvc
{
    //public interface IEdFiSiteAreasBase<TAdmin, TLocalEducationAgency, TSchool, TStudentSchool, TStaff, TCommon, TSearch> 
    //    where TAdmin                : Areas.Admin
    //    where TLocalEducationAgency : Areas.LocalEducationAgency, new()
    //    where TSchool               : Areas.School, new()
    //    where TStudentSchool        : Areas.StudentSchool, new()
    //    where TStaff                : Areas.Staff, new()
    //    where TCommon               : Areas.Common, new()
    //    where TSearch               : Areas.Search, new()
    //{
    //    TAdmin Admin { get; }
    //    TLocalEducationAgency LocalEducationAgency { get; }
    //    TStudentSchool StudentSchool { get; }
    //    TSchool School { get; }
    //    TStaff Staff { get; }
    //    TCommon Common { get; }
    //    TSearch Search { get; }
    //}

    /// <summary>
    /// Provides an extensibility point for providing strongly-typed classes for generating links for the various areas of the Ed-Fi dashboard application.
    /// </summary>
    /// <typeparam name="TAdmin">The concrete type of the class facilitating link generation for the Admin area of the application.</typeparam>
    /// <typeparam name="TApplication">The concrete type of the class facilitating link generation for the Application area of the application.</typeparam>
    /// <typeparam name="TLocalEducationAgency">The concrete type of the class facilitating link generation for the Local Education Agency area of the application.</typeparam>
    /// <typeparam name="TSchool">The concrete type of the class facilitating link generation for the School area of the application.</typeparam>
    /// <typeparam name="TStudentSchool">The concrete type of the class facilitating link generation for the Student area of the application.</typeparam>
    /// <typeparam name="TStaff">The concrete type of the class facilitating link generation for the Staff area of the application.</typeparam>
    /// <typeparam name="TCommon">The concrete type of the class facilitating link generation for the Common area of the application.</typeparam>
    /// <typeparam name="TSearch">The concrete type of the class facilitating link generation for the Search area of the application.</typeparam>
    /// <typeparam name="TError">The concrete type of the class facilitating link generation for the Error area of the application.</typeparam>
    public abstract class EdFiSiteAreasBase<TAdmin, TApplication, TLocalEducationAgency, TSchool, TStudentSchool, TStaff, TCommon, TSearch, TError> 
        //: IEdFiSiteAreasBase<TAdmin, TLocalEducationAgency, TSchool, TStudentSchool, TStaff, TCommon, TSearch> 
        where TAdmin                : IAdminAreaLinks
        where TApplication          : IApplicationAreaLinks
        where TLocalEducationAgency : ILocalEducationAgencyAreaLinks
        where TSchool               : ISchoolAreaLinks
        where TStudentSchool        : IStudentSchoolAreaLinks
        where TStaff                : IStaffAreaLinks
        where TCommon               : ICommonLinks
        where TSearch               : ISearchAreaLinks
        where TError                : IErrorAreaLinks
    {
        protected EdFiSiteAreasBase()
        {
            Admin = IoC.Resolve<TAdmin>();
            Application = IoC.Resolve<TApplication>();
            LocalEducationAgency = IoC.Resolve<TLocalEducationAgency>();
            School = IoC.Resolve<TSchool>();
            Common = IoC.Resolve<TCommon>();
            Search = IoC.Resolve<TSearch>();
            Staff = IoC.Resolve<TStaff>();
            StudentSchool = IoC.Resolve<TStudentSchool>();
            Error = IoC.Resolve<TError>();
        }

        public virtual TAdmin Admin { get; protected set; }
        public virtual TApplication Application { get; protected set; }
        public virtual TLocalEducationAgency LocalEducationAgency { get; protected set; }
        public virtual TStudentSchool StudentSchool { get; protected set; }
        public virtual TSchool School { get; protected set; }
        public virtual TStaff Staff { get; protected set; }
        public virtual TCommon Common { get; protected set; }
        public virtual TSearch Search { get; protected set; }
        public virtual TError Error { get; protected set; }
    }
}
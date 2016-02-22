/*/ *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Linq;
using Castle.DynamicProxy;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Security
{
	public class StudentIQueryableInterceptor : IInterceptor
	{
	    private const string noUserInformationErrorMessage = "No user information.";

        private readonly IAuthorizationInformationProvider authorizationInformationProvider;

        public StudentIQueryableInterceptor(IAuthorizationInformationProvider authorizationInformationProvider)
		{
            this.authorizationInformationProvider = authorizationInformationProvider;
		}

		public void Intercept(IInvocation invocation)
		{
			invocation.Proceed();
			/**************************************************************
			 * Spike Activity for Filtering IQueryables done on August 24, 2011                                                            
			 * Notes:
			 * 1) It didn't work 100% because of various factors:
			 *		a) One of the problems was that you cant go back from an IQueryable<IStudent> to the original IQueryable<originalModel> The cast blows up.
			 *		b) If we added restrictions to the extension method for T to be T:IStudent then subsonic would blow up saying that it doesn't know what to do with "StudetId"
			 *		c) If using Dynamic then we cant use LINQ
			 *		d) Also if we could get everything working there is still one problem. Subsonic does not support UNION. In the case of teacher the query to see available students is cohorts + sections.
			/************************************************************** /
			//invocation.ReturnValue = FilterResults(invocation.ReturnValue);
		}

		protected dynamic FilterResults(dynamic returnedModel)
		{
			if (returnedModel == null)
				return null;

			dynamic queryable = returnedModel as IQueryable<object>;

			if(queryable!=null)
			{
				var user = GetCurrentUserInfo();

				IQueryable<int> filteringQuery = null;

                if (user.HasLocalEducationAgencyClaim(EdFiDashboardContext.Current.LocalEducationAgencyId, EdFiClaimTypes.ViewAllStudents))
                {
                    // they can see all students
                    //filteringQuery = null;
                }
                else if (user.HasClaimOnAnyOrganization( EdFiClaimTypes.ViewAllStudents))
			    {
					// they can see all students for their school
			        filteringQuery = authorizationInformationProvider.GetIQueryableForPrincipalStudentUSIs(user.StaffUSI);
			    }
                else if (user.HasClaimOnAnyOrganization(EdFiClaimTypes.ViewMyStudents))
                {
                    // they can see all students in all of their cohorts
                    // they can see all students in all of their sections
                    //filteringQuery = authorizationInformationProvider.GetStaffStudentUSIs(user.StaffUSI, schoolYearProvider.CurrentSchoolYear); 
                }

				if(filteringQuery!=null)
				{
					dynamic queryableOfIStudent = returnedModel as IQueryable<IStudent>;
					if (queryableOfIStudent != null)
						return QueryableFilteringExtensions.FilterStudents(returnedModel, filteringQuery);
				}

			}


			return returnedModel;
		}

        private static UserInformation GetCurrentUserInfo()
		{
			var userInfo = UserInformation.Current;

			if (userInfo == null)
			    throw new SessionExpiredException(noUserInformationErrorMessage);

            return userInfo;
		}
	}

	public static class QueryableFilteringExtensions
	{
		public static IQueryable<T> FilterStudents<T>(this IQueryable<T> source, IQueryable<int> filteringQuery)//Tried restrictions but it breaks subsonic. It gives out something like cant do something with StudentUSI
		{
			Type enumerableType = typeof(T);
			if (enumerableType.GetInterfaces().Contains(typeof(IStudent)))
			{
				//var queryableOfIStudent = source as IQueryable<StudentSearchModel>;
				var queryableOfIStudent = source as IQueryable<IStudent>;//This breaks the code... cant cast it back...
					

					if(queryableOfIStudent!=null)
					{
						var x = (from s in queryableOfIStudent
						         join f in filteringQuery on s.StudentUSI equals f
						         select s);
						return (IQueryable<T>)x;
					}
			}

			return source;
		}

	}
}*/

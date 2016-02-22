// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
namespace EdFi.Dashboards.SecurityTokenService.Web.Mvp.BaseArchitecture
{
	public interface IViewBase<TModel>
	{
		TModel Model { get; set; }
		
		void PerformDataBinding();
	}
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Coypu;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Presentation.Architecture.Mvc.Controllers;
using EdFi.Dashboards.Presentation.Core.Tests.Routing.Support;
using EdFi.Dashboards.Presentation.Core.UITests.Attributes;
using EdFi.Dashboards.Presentation.Core.UITests.Support.Coypu;
using EdFi.Dashboards.Presentation.Core.UITests.Support.SpecFlow;
using EdFi.Dashboards.Resources.Models.School.Information;
using EdFi.Dashboards.Resources.Models.Staff;
using EdFi.Dashboards.Resources.Staff;
using NUnit.Framework;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace EdFi.Dashboards.Presentation.Core.UITests.Pages.Staff
{
    [AssociatedController(typeof (ServicePassthroughController<GeneralOverviewRequest, GeneralOverviewModel>))]
    public class GeneralOverviewPage : StaffBasedPageBase<GeneralOverviewPage, InformationModel>
    {
        //----------------------------------------------------------------------------
        //                          Css Selectors
        //----------------------------------------------------------------------------

        protected virtual string CustomizeViewButtonIdCss
        {
            get { return "#GeneralOverview-EdFiGrid-interaction-open-menu-button"; }
        }

        protected virtual string AddOrRemoveStudentsFromWatchListButtonIdCss
        {
            get { return "#GeneralOverview-CustomStudentList-StartLink"; }
        }

        protected virtual string AddSelectedStudentsToListButtonIdCss
        {
            get { return "#GeneralOverview-CustomStudentList-FinishLink"; }
        }

        protected virtual string AddSelectedStudentsToExistingListRaidioCss
        {
            get { return "#GeneralOverview-CustomStudentList-Create-UseOldListRadio"; }
        }

        protected virtual string CreateNewStudentListRadioCss
        {
            get { return "#GeneralOverview-CustomStudentList-Create-UseNewListRadio"; }
        }

        protected virtual string FirstStudentListItemCss
        {
            get { return "select#GeneralOverview-CustomStudentList-Create-ListSelectionListbox > option"; }
        }

        protected virtual string CreateNewStudentListTextboxIdCss
        {
            get { return "#GeneralOverview-CustomStudentList-Create-NewListName"; }
        }

        protected virtual string CustomStudentListCheckboxClassCss
        {
            get { return ".CustomStudentList-checkbox"; }
        }

        protected virtual string CustomStudentListStudentsTableIdCss
        {
            get { return "#GeneralOverview-EdFiGrid-fixed-data-table"; }
        }

        protected virtual string CustomStudentListNameSelectorCss
        {
            get { return "#GeneralOverview-EdFiGrid-fixed-data-table a:nth-child(2n)";  }
        }

        protected virtual string SelectedStudentsLocatorCss(int row)
        {
            int s = row;
            return string.Format("#GeneralOverview-EdFiGrid-fixed-data-table > tbody:nth-child(1) > tr:nth-child({0}) > td:nth-child(2) > div:nth-child(1) > div:nth-child(1) > span:nth-child(3) > a:nth-child(2)", s);
        }

        protected virtual string StudentListDropDownMenuCss
        {
            get { return "#sectionSelect"; }
        }

        protected virtual string StudenListSelectorCss
        {
            get { return "#sectionSelect option"; }
        }

        protected virtual string EditStudentWatchListCss
        {
            get { return "#GeneralOverview-CustomStudentList-StartLink"; }
        }

        protected virtual string DeleteSelectedStudentsFromWatchListCss
        {
            get { return "#GeneralOverview-CustomStudentList-FinishLink"; }
        }

        protected virtual string DeleteStudentWatchListCss
        {
            get { return "#GeneralOverview-CustomStudentList-DeleteLink"; }
        }

        protected virtual string RenameStudentWatchListCss
        {
            get { return "#GeneralOverview-CustomStudentList-RenameLink"; }
        }

        protected virtual string RenameStudentWatchListTextboxCss
        {
            get { return "#GeneralOverview-CustomStudentList-Rename-Input"; }
        }

        protected virtual string SeeMoreDataCss
        {
            get { return "#GeneralOverview-EdFiGrid-change-columns"; }
        }

        protected virtual string SaveColumnsCss
        {
            get { return "#GeneralOverview-EdFiGrid-save-columns"; }
        }
        
        protected virtual string ResetColumnsCss
        {
            get { return "#GeneralOverview-EdFiGrid-reset-columns"; }
        }

        /// <summary>
        /// Gets the format specifier for a CSS selector to locate a specific checkbox, where the 
        /// single format value refers a number associated with the checkbox, incremented by the
        /// value of the <see cref="RowHeaderColumnsToSkipInGrid"/> property prior to formatting.
        /// </summary>
        protected virtual string ColumnCheckboxCssFormat
        {
            get { return "#cb"; }
        }

        protected virtual string ColumnNamesCss
        {
            //Most rediculous, outragous CSS selector yet!
            get { return "#GeneralOverview-EdFiGrid-scroll-header-table > tbody:nth-child(1) > tr:nth-child(3) > td:not(.hiddenCol) > a"; }
            //                   ID of body ↑,              one tag down ↑,  third tr tag ↑,  not hidden ↑
            //#GeneralOverview-EdFiGrid-scroll-header-table > tbody:nth-child(1) > tr:nth-child(3) > td:nth-child(6) > a:nth-child(1)
        }

        protected virtual string ColumnHeaderPrefixCss
        {
            get { return ".scroll"; }
        }

        /// <summary>
        /// Indicates the number of base columns in grid to skip when determining the starting point for metric column selection.
        /// </summary>
        protected virtual int RowHeaderColumnsToSkipInGrid
        {
            get { return 5; }
        }

        //------------------------------------------------------

        public override void Visit(bool forceNavigation = false)
        {
            if (!IsCurrent() || forceNavigation)
            {
                // Navigate to the General Overview page
                var userProfile = ScenarioContext.Current.GetUserProfile();

                var url = Website.Staff.GeneralOverview(userProfile.GetSchoolId(), userProfile.StaffUSI, "staff-name");

                ScenarioContext.Current
                               .GetBrowser()
                               .Visit(url);
            }
        }

        //---------------------------------------------------------------
        //                  Clicking Various Things
        //--------------------------------------------------------------

        protected virtual void ClickCustomizeView()
        {
            Browser.FindCss(CustomizeViewButtonIdCss).Click();
        }

        protected virtual void ClickCreateOrAddToWatchList()
        {
            Browser.FindCss(AddOrRemoveStudentsFromWatchListButtonIdCss).Click();
        }

        protected virtual void ActivateStudentSelectionModeForCreateOrAddToWatchList()
        {
            ClickCustomizeView();
            ClickCreateOrAddToWatchList();
        }

        protected virtual void ClickAddSelectedStudents()
        {
            Browser.FindCss(AddSelectedStudentsToListButtonIdCss).Click();
        }

        protected virtual void ClickEditWatchList()
        {
            Browser.FindCss(EditStudentWatchListCss).Click();
        }

        protected virtual void ClickRenameWatchList()
        {
            Browser.FindCss(RenameStudentWatchListCss).Click();
        }

        protected virtual void DeleteSelectedStudentsFromWatchList()
        {
            Browser.FindCss(DeleteSelectedStudentsFromWatchListCss).Click();
            ClickOk();
        }

        protected virtual void ClickSeeMoreData()
        {
            Browser.FindCss(SeeMoreDataCss).Click();
        }
        //-----------------------------------------------------------------------------------

        public string DeleteWatchList()
        {
            Browser.FindCss(DeleteStudentWatchListCss).Click();
            var deletedList = Browser.FindCss(StudentListDropDownMenuCss).SelectedOption;
            ClickOk();
            return deletedList;
        }

        public void ResetColumns()
        {
            int loop = 0;
            while (!Browser.FindCss(ResetColumnsCss).Exists() & loop < 5)
            {
                loop++;
            }
            Browser.FindCss(ResetColumnsCss).Click();
        }

        public virtual void ClickOk()
        {
            Browser.ClickButton("OK");
        }


        public List<string> DeleteStudentsFromWatchList(int count)
        {
            ActivateStudentSelectionModeForEditing();
            var deletedStudents = SelectStudentsForCustomStudentListOperation(count);
            DeleteSelectedStudentsFromWatchList();

            return deletedStudents;
        }
        
        public List<string> AddStudentsToNewList(int count, string name)
        {

            ActivateStudentSelectionModeForCreateOrAddToWatchList();

            // Select "n" students from the list
            var studentNames = SelectStudentsForCustomStudentListOperation(count);
            ClickAddSelectedStudents();

            // Enter Name of new student list
            Browser.FindCss(CreateNewStudentListTextboxIdCss).FillInWith(name);
            
            ClickOk();

            return studentNames;

        }

        public CustomStudentListManipulationResult AddStudentsToExistingList(int count)
        {
            var studentListResult = new CustomStudentListManipulationResult();

            ActivateStudentSelectionModeForCreateOrAddToWatchList();

            // Select "n" students from the list
            studentListResult.StudentNames = SelectStudentsForCustomStudentListOperation(count, 2); 
            ClickAddSelectedStudents();
            // Indicate we want to add the selected students to an existing list
            Browser.FindCss(AddSelectedStudentsToExistingListRaidioCss).Click();

            // Select the first existing list
            studentListResult.ListName = SelectFirstExistingCustomStudentList();
            
            ClickOk();

            return studentListResult;
        }
        
        /// <summary>
        /// Selects the first existing custom student list found in the dialog presented for adding students to an existing list.
        /// </summary>
        /// <returns>The name of the existing custom student list to which students will be added.</returns>
        protected string SelectFirstExistingCustomStudentList()
        {
            Browser.FindCss(FirstStudentListItemCss).Click();

            return Browser.FindCss(FirstStudentListItemCss).Text;
        }

        /// <summary>
        /// Selects the specified number of students from the already activated list in selection mode.
        /// </summary>
        /// <param name="count">The number of students to select.</param>
        /// <param name="startIndex">The starting index (0-based) of the first student to select.</param>
        /// <returns>A list containing the selected students' last and first name (as displayed on the webpage).</returns>
        protected List<string> SelectStudentsForCustomStudentListOperation(int count, int startIndex = 0)
        {
            var checkboxes = Browser.FindAllCss(CustomStudentListCheckboxClassCss);
            startIndex = startIndex + 2;
           const int invisibleCheckboxesPresentOnPage = 0;

            // Select the students
            var checkBoxesToSelect =
                from c in checkboxes.Skip(invisibleCheckboxesPresentOnPage + startIndex).Take(count)
                select c;

            checkBoxesToSelect.Each(c => c.Click());

            // Gather the names of the selected students
            var studentNames = new List<string>();
            for (int i = 0; i < count; i++)
            {
                studentNames.Add(String.Format(Browser.FindCss(SelectedStudentsLocatorCss(i + 1)).Text));
            }
            

            // Verify that the correct number were found
            Assert.That(studentNames.Count(), Is.EqualTo(count));

            // Finish Creating the Student Watch list
            
           
            return studentNames;
        }

        public void RenameCurrentWatchList(string newName)
        {
            ActivateStudentSelectionModeForEditing();
            Browser.FindCss(RenameStudentWatchListCss).Click();
            Browser.FindCss(RenameStudentWatchListTextboxCss).FillInWith(newName);
            ClickOk();
        }

        public virtual void ActivateStudentSelectionModeForEditing()
        {
            ClickCustomizeView();
            ClickEditWatchList();
        }

        public void NavigateToColumnSelection()
        {
            ClickCustomizeView();
            ClickSeeMoreData();
        }

        public List<string> EditColumns(int count)
        {
            var clickedColumns = new List<string>();
            
            for (int i = 0; i < count; i++)
            {
                int addon = i + RowHeaderColumnsToSkipInGrid;
                Browser.FindCssPatiently(ColumnCheckboxCssFormat + addon).Click();
                clickedColumns.Add(Browser.FindCss(string.Format(".scroll{0} > label", addon)).Text);
                
            }
            
            Browser.FindCssPatiently(SaveColumnsCss).Click();

            return clickedColumns;

        }

        //-----------------------------------------------------
        //                      Checks
        //-----------------------------------------------------

        public string GetSelectedStudentList(Options options)
        {
            Exception lastException = new Exception("Could not find Selected Student List");
            
            bool exceptionThrown = false;
            ElementScope studentListDropdown = null;
            DateTime expireTime = DateTime.Now + options.Timeout;

            while (DateTime.Now < expireTime)
            {
                try
                {
                   studentListDropdown = Browser.FindCss(StudentListDropDownMenuCss, Make_It.Wait_10_Seconds);
                    exceptionThrown = false;
                    var testaccessing = studentListDropdown.SelectedOption; //Access studentListDropdown object. If it has changed, an exception will be thrown.
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    exceptionThrown = true;
                }

                if (exceptionThrown == false)
                    return studentListDropdown.SelectedOption;
                    
            }

            throw lastException;

        }

        public List<string> GetStudentsInList()
        {
            var studentsFoundInList = Browser.FindAllCssPatiently(CustomStudentListNameSelectorCss);
            Console.WriteLine("Students found in list: " + studentsFoundInList.Count());
            var students = studentsFoundInList.Select(name => name.Text).ToList();
            
            return students;
        }

        public List<string> GetStudentLists()
        {
            var listOptionsFromCss = Browser.FindAllCssPatiently(StudenListSelectorCss);
            var listOptions = new List<string>();

            foreach (var option in listOptionsFromCss)
            {
                if (!listOptions.Contains(option.Text))
                    listOptions.Add(option.Text);
            }

            return listOptions;
        }

        public List<string> GetColumns()
        {
            var columnsShowingUpList = new List<string>();
            bool breakallow = false;
            while(true)
            {
                try
                {
                    var columnsShowingUp = Browser.FindAllCssPatiently(ColumnNamesCss);
                    foreach (var column in columnsShowingUp)
                    {
                        if (!string.IsNullOrWhiteSpace(column.Text))
                            columnsShowingUpList.Add(column.Text.Trim());
                    }
                    if (columnsShowingUpList.Count > 0)
                        breakallow = true;
                }
                catch (StaleElementReferenceException)
                {
                    breakallow = false;
                    var x = new List<string>();
                    columnsShowingUpList = x;
                }
                if (breakallow)
                    break;
            }
            return columnsShowingUpList;
        }
    }

    public class CustomStudentListManipulationResult
    {
        public string ListName { get; set; }
        public List<string> StudentNames { get; set; }
    }
}
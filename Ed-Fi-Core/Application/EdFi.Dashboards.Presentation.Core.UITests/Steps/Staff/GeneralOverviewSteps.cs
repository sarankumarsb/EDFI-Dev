using System;
using System.Collections.Generic;
using System.Threading;
using EdFi.Dashboards.Presentation.Core.UITests.Pages.Staff;
using EdFi.Dashboards.Presentation.Core.UITests.Support.Coypu;
using NUnit.Framework;
using TechTalk.SpecFlow;
using EdFi.Dashboards.Presentation.Core.UITests.Support.SpecFlow;

namespace EdFi.Dashboards.Presentation.Core.UITests.Steps.Staff
{
    [Binding]
    public class GeneralOverviewSteps
    {
        private readonly GeneralOverviewPage generalOverviewPage;

        //-----------------------------------------------------
        //             Keys for ScenarioContext
        //-----------------------------------------------------

        protected string ListNameContextKey
        {
            get { return "NameOfList"; }
        }

        protected string NameOfDeletedListKey
        {
            get { return "DeletedList"; }
        }

        protected string NameOfDeletedStudentKey
        {
            get { return "DeletedStudentFromList"; }
        }

        protected string SelectedStudentNamesContextKey
        {
            get { return "selectedStudentNames"; }
        }

        protected string ClickColumnsKey
        {
            get { return "ClickedColumns"; }
        }

        private static string CreateListName()
        {
            var listName = "TEST - " + DateTime.Now.Ticks;
            return listName;
        }

        public GeneralOverviewSteps(GeneralOverviewPage generalOverviewPage)
        {
            this.generalOverviewPage = generalOverviewPage;
        }

        [Given(@"I am on the General Overview page")]
        public void GivenIAmOnTheGeneralOverviewPage()
        {
            generalOverviewPage.Visit();
        }

        [When(@"I add ([0-9]*) student[s]? to a new list")]
        public void WhenIAddXStudentsToANewList(int count)
        {
            string name = CreateListName();
            var students = generalOverviewPage.AddStudentsToNewList(count, name);
            
            ScenarioContext.Current.Set(students, SelectedStudentNamesContextKey);
            ScenarioContext.Current.Set(name, ListNameContextKey);
        }

        

        [When(@"I add ([0-9]*) student[s]? to the existing list")]
        public void WhenIAddXStudentsToTheExistingList(int count)
        {
            var result = generalOverviewPage.AddStudentsToExistingList(count);

            // Save the list name and the students that were selected for the operation for later assertions
            ScenarioContext.Current.Set(result.StudentNames, SelectedStudentNamesContextKey);
            ScenarioContext.Current.Set(result.ListName, ListNameContextKey); 
        }

        [When(@"I delete ([0-9]*) student[s]? from the custom list")]
        public void WhenIDeleteXStudentsFromTheCustomList(int count)
        {
            ScenarioContext.Current.Set(generalOverviewPage.DeleteStudentsFromWatchList(count), SelectedStudentNamesContextKey);
        }

        [When(@"I delete the student watch list")]
        public void WhenIDeleteTheStudentWatchList()
        {
            generalOverviewPage.ActivateStudentSelectionModeForEditing();
            ScenarioContext.Current.Set(generalOverviewPage.DeleteWatchList(), NameOfDeletedListKey);
        }

        [When(@"I rename the watch list")]
        public void WhenIRenameTheWatchList()
        {
            string newListName = CreateListName();
            
            generalOverviewPage.RenameCurrentWatchList(newListName);
            ScenarioContext.Current.Set(newListName, ListNameContextKey); 
        }

        [When(@"I [adremove]+ ([0-9]*) columns [tofrm]+ the list")]
        public void WhenIAddorRemoveXColumns(int count)
        {
            generalOverviewPage.NavigateToColumnSelection();
            ScenarioContext.Current.Set(generalOverviewPage.EditColumns(count), ClickColumnsKey);
        }

        [When(@"I reset the columns")]
        public void WhenIResetTheColumns()
        {
            generalOverviewPage.NavigateToColumnSelection();
            generalOverviewPage.ResetColumns();
        }

        [Then(@"the new student list should be selected")]
        public void ThenTheNewStudentListShouldBeSelected()
        {
            var nameOfCreatedList = ScenarioContext.Current.Get<string>(ListNameContextKey);
            string nameOfSelectedList = "";
            var finish = DateTime.Now.AddSeconds(15);
            while (DateTime.Now < finish)
            {
                nameOfSelectedList = generalOverviewPage.GetSelectedStudentList(Make_It.Wait_10_Seconds);
                if (nameOfCreatedList == nameOfSelectedList)
                {
                    break;
                }
                Thread.Sleep(250); //Giving the thread some room to breathe.
                //Sometimes the browser will grab the selected student list before the page reloads,
                //which can allow for the old student name list to be passed in, causing the test to fail.
            }
            Assert.That(nameOfCreatedList == nameOfSelectedList);
        }

        [Then(@"I should (see|not see) those students in the list")]
        public void ThenIShouldSeeOrNotSeeThoseStudentsInTheList(string seeOrNotSee)
        {
            var namesSavedEarlier = ScenarioContext.Current.Get<List<string>>(SelectedStudentNamesContextKey);
            var namesInList = generalOverviewPage.GetStudentsInList();
            bool seeOrNotSeeBool = (seeOrNotSee != "not see");
            // Check each name in the list
            foreach (var name in namesSavedEarlier)
            {
                Assert.That(namesInList.Contains(name) == seeOrNotSeeBool);
            }
        }

        [Then(@"I should just see the students in the list")]
        public void ThenIShouldJustSeeThoseSelectedStudentsInTheList()
        {
            var namesSavedEarlier = ScenarioContext.Current.Get<List<string>>(SelectedStudentNamesContextKey);
            var namesInList = generalOverviewPage.GetStudentsInList();
            var namesfound = new bool[namesSavedEarlier.Count];
            int i = 0;
            foreach (var name in namesSavedEarlier)
            {
                if (namesInList.Contains(name))
                {
                    namesfound[i] = true;
                    i++;
                }
                namesInList.Remove(name);
            }
            Assert.That(namesInList.Count == 0); //No Leftover names
        }

        [Then(@"the Watch List should not be in the drop down list")]
        public void ThenTheWatchListShouldNotBeInTheDropDownList()
        {
            var deletedList = ScenarioContext.Current.Get<string>(NameOfDeletedListKey);
            var studentLists = generalOverviewPage.GetStudentLists();
            Assert.That(!studentLists.Contains(deletedList));
        }

        [Then(@"I should (see|not see) those columns")]
        public void ThenIShouldSeeThoseColumns(string seeOrNotSeeText)
        {
            var clickedColumns = ScenarioContext.Current.Get<List<string>>(ClickColumnsKey);
            var columnsOnPage = generalOverviewPage.GetColumns();
            
            bool shouldSee = (seeOrNotSeeText == "see");

            // Check each name in the list
            foreach (string column in clickedColumns)
            {
                Assert.That(columnsOnPage.Contains(column), Is.EqualTo(shouldSee), string.Format("Unable to see column '{0}' in the grid.", column));
            }
        }

        [Then(@"I check the column reset")]
        public void ThenICheckColumnReset()
        {
            var columnsOnPage = generalOverviewPage.GetColumns();
            Assert.That(!columnsOnPage.Contains("Last Eight Weeks Attendance")); // I don't know how to check for reset...
        }
    }
}




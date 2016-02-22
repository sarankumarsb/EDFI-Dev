Feature: Attendance and Discipline
	In order to spot attendance and discipline issues
	As a dashboard user
	I want to see things

Scenario Outline: Look for drilldowns
	Given I am logged on as the <profile>
	And I am on the Attendance and Discipline page of the Local Education Agency Academic Dashboard
	Then I should <see-or-not-see> the "More" menu

	Examples: 
	| profile               | see-or-not-see |
	| Superintendent        | see            |
	| High School Principal | not see        |

Scenario: Expand school list drilldown
	Given I am logged on as the Superintendent
	And I am on the Attendance and Discipline page of the Local Education Agency Academic Dashboard
	When I show the School List for the Daily Attendance Rate --> year to date metric
	Then I should see the text "Middle School"

Feature: See More Data
	As a teacher
	I want to manage the visiblity of additional columns on the student lists
	So that I can see information more pertient to helping my students more effectively

Background:
    Given I am logged on as the High School Teacher

Scenario: 1 - Add Columns
	Given I am on the teacher's General Overview page
	When I add 2 columns to the list
	Then I should see those columns

Scenario: 2 - Remove Columns
	Given I am on the teacher's General Overview page
	When I remove 1 columns from the list
	Then I should not see those columns

Scenario: 3 - Reset Columns
	Given I am on the teacher's General Overview page
	When I reset the columns
	Then I check the column reset
	#Then I should see the default set of columns

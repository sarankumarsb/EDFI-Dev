Feature: Manage System Message
	In order to provide users with important timely information
	As a System Administrator
	I want to be able to manage the system message to be displayed on all web pages

Scenario: 1 - Set the current system message
	Given I am logged on as the System Administrator
	And I enter a system message
	And I am logged on as the High School Teacher
	And I go to the teacher's General Overview page
	Then the previously set system message should be displayed

Scenario: 2 - Clear the current system message
	Given I am logged on as the System Administrator
	And I clear the system message
	And I am logged on as the High School Teacher
	And I go to the teacher's General Overview page
	Then the system message should not be displayed

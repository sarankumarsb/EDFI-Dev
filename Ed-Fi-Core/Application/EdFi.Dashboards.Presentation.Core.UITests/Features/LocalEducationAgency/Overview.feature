Feature: Local Education Agency Overview
	In order to see the state of my LEA
	As a district-level administrator
	I want to see a high level summary of district performance

Background:
	Given I am logged on as the Superintendent
	And I am on the Local Education Agency Overview page

# This scenario demonstrates the use of the underlying ViewModel 
# to verify that the data is displayed on the page correctly
Scenario: Reviewing Attendance and Discipline metrics
	Then the accountability rating label should be correct
	And the accountability rating should be correct

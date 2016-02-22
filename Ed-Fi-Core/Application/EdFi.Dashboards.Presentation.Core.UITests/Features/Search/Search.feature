Feature: Search
	In order to find students
	as a Education-related District Employee
	I want to be able to search for students

Scenario: Search for student
	Given I am logged on as the High School Teacher
	When I search for and select a student
	Then I should be on that student's page 

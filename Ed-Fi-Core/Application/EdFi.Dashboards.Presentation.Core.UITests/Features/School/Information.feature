Feature: School Information
	In order to know more about a school without having to visit multiple pages
	As a dashboard user
	I want to see all the basic information about a school in one place

Scenario: Confirm Student Demographics
	Given I am logged on as the Superintendent
	And I am on the High School Information page
	Then I should see the correct student gender demographics

Feature: Login
	In order to use the dashboards
	As a dashboards user
	I want to log in

Background: 
	Given I am at the login page

Scenario: Failed login
	Given I have entered an incorrect password
	When I click the Login button
	Then I should see an error message containing the text "username or password is incorrect"

Scenario: Successful login with the ENTER key
	Given I have entered the correct password for the Superintendent
	When I hit the ENTER key
	Then I should be logged in successfully

Scenario Outline: Successful login
	Given I have entered the correct password for the <profile>
	When I click the Login button
	Then I should be logged in successfully
	And I should be on the <dashboard_type> <page_name> page

	Examples:
	| profile               | dashboard_type         | page_name |
	| Superintendent        | Local Education Agency | Overview  |
	| High School Principal | School                 | Overview  |

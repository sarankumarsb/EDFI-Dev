Feature: Killswitch
	As a system Administrator
	I want to be able to shut off access to the dashboards
	to prevent dissemination of inaccurate data

Background: 
	# Get the High School Teacher logged in and on the General Overview page before activating the killswitch
	Given I am logged on as the High School Teacher
	And I go to the teacher's General Overview page

Scenario: 1 - Killswitch prevents further access to the site for users already logged in
	Given I am logged on as the System Administrator
	And I have activated the kill switch
	And I am logged on as the High School Teacher
	And I refresh the current web page
	Then access to the website data should be prevented

Scenario: 2 - Killswitch denys login
	Given I attempt to log on as the Superintendent
	Then access to the website data should be prevented

Scenario: 3 - Deactivate kill switch
	Given I am logged on as the System Administrator
	And I have deactivated the kill switch
	And I am logged on as the High School Teacher
	And I go to the teacher's General Overview page
	Then access to the website data should be allowed

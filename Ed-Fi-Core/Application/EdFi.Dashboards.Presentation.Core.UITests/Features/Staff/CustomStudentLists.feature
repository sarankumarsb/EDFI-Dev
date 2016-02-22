Feature: Custom Student Lists
    As a teacher
    I want to manage custom student lists
    So that I can track the progress of groups of students more effectively

Background:
    Given I am logged on as the High School Teacher

Scenario: 1 - Create a new student list
    Given I am on the teacher's General Overview page
    When I add 2 students to a new list
    Then the new student list should be selected
    And I should just see the students in the list

Scenario: 2 - Add Student to the list
    Given I am on the teacher's General Overview page
    When I add 1 student to the existing list
    Then the new student list should be selected
    And I should see those students in the list

Scenario: 3 - Delete Student from the list
    When I delete 1 student from the custom list
    Then I should not see those students in the list

Scenario: 4 - Rename Student Watch List
    When I rename the watch list
    Then the new student list should be selected

Scenario: 5 - Delete Student Watch List
    When I delete the student watch list
    Then the Watch List should not be in the drop down list


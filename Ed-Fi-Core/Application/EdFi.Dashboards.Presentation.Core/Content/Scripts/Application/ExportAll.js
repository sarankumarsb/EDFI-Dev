/// <reference path="../External/jquery.js" />
/// <reference path="analytics.js"/>

function ExportLocalEducationAgencyCustomStudentList(listName, exportPath) {
    analyticsManager.trackExport('LocalEducationAgencyCustomStudentList', listName);
    window.location = exportPath;
}
function ExportLocalEducationAgencySchoolMetricList(metricVariantId, exportPath) {
    analyticsManager.trackExport('LocalEducationAgencySchoolMetricTable', metricVariantId);
    window.location = exportPath;
}

function ExportLocalEducationAgencyStudentDemographicList(demographic, exportPath) {
    analyticsManager.trackExport('LocalEducationAgencyDemographicList', demographic);
    window.location = exportPath;
}

function ExportLocalEducationAgencyStudentSchoolCategory(schoolCategory, exportPath) {
    analyticsManager.trackExport('LocalEducationAgencyStudentSchoolCategory', schoolCategory);
    window.location = exportPath;
}

function ExportSchoolStudentList(metricVariantId, exportPath) {
    analyticsManager.trackExport('SchoolStudentList', metricVariantId);
    window.location = exportPath;
}

function ExportSchoolStudentDemographicList(demographic, exportPath) {
    analyticsManager.trackExport('SchoolStudentDemographicList', demographic);
    window.location = exportPath;
}

function ExportSchoolStudentGradeList(grade, exportPath) {
    analyticsManager.trackExport('SchoolStudentGradeList', grade);
    window.location = exportPath;
}
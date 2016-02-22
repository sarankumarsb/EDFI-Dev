// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Resources.Models.Student;

namespace EdFi.Dashboards.Resources.StudentSchool
{
    public interface IAccommodationProvider
    {
        List<Accommodations> GetAccommodations(long studentUSI, int schoolId);
        List<Accommodation> GetAccommodations(long[] studentUSIs, int schoolId);
    }

    public class AccommodationProvider : IAccommodationProvider
    {
        protected const string NullAccommodationExceptionMessage = "Student {0} has an accommodation with a null name.";
        protected const string AccommodationNotImplementedExceptionMessage = "Accommodation not implemented. ({0})";
        protected const string GiftedTalentedAccommodation = "Gifted/Talented";
        protected const string SpecialEducationAccommodation = "Special Education";
        protected const string PrimaryInstructionalSettingAccommodation = "Primary Instructional Setting";
        protected const string SpecialEducationServicesAccommodation = "Special Education Services";
        protected const string EnglishAsSecondLanguageAccommodation = "English as Second Language";
        protected const string LimitedEnglishProficiencyAccommodation = "Limited English Proficiency";
        protected const string LimitedEnglishProficiencyMonitoredFirstAccommodation = "Limited English Proficiency Monitored 1";
        protected const string LimitedEnglishProficiencyMonitoredSecondAccommodation = "Limited English Proficiency Monitored 2";
        protected const string BilingualProgramAccommodation = "Bilingual Program";
        protected const string RepeaterAccommodation = "Repeater";
        protected const string OverAgeAccommodation = "Over Age";
        protected const string TestAccommodation = "Test Accommodation";
        protected const string LateEnrollment = "Late Enrollment";
        protected const string PartialTranscript = "Partial Transcript";
        protected const string DesignationAccommodation = "504 Designation";
        protected const string Yes = "YES";

        private readonly IRepository<StudentIndicator> _studentIndicatorRepository;
        private readonly IRepository<StudentSchoolInformation> _studentSchoolInformationRepository;
        private readonly School.IIdNameService _idNameService;

        public AccommodationProvider(IRepository<StudentIndicator> studentIndicatorRepository, 
            IRepository<StudentSchoolInformation> studentSchoolInformationRepository, 
            School.IIdNameService idNameService)
        {
            _studentSchoolInformationRepository = studentSchoolInformationRepository;
            _idNameService = idNameService;
            _studentIndicatorRepository = studentIndicatorRepository;
        }

        public List<Accommodations> GetAccommodations(long studentUSI, int schoolId)
        {
            var results = GetAccommodations(new [] {studentUSI}, schoolId);
            if (results == null || results.Count == 0)
                return new List<Accommodations>();
            return results[0].AccommodationsList;
        }

        public List<Accommodation> GetAccommodations(long[] studentUSIs, int schoolId)
        {
            var res = new List<Accommodation>();
            var accommodations = GetStudentAccommodations(schoolId, studentUSIs).ToList();

            var students = from a in accommodations
                           group a by a.StudentUSI
                               into g
                               select new { StudentUSI=g.Key, Accommodations=g };

            foreach (var s in students)
            {
                var model = new Accommodation(s.StudentUSI);

                foreach (var a in s.Accommodations)
                {
                    if (string.IsNullOrEmpty(a.Name))
                        throw new InvalidOperationException(String.Format(NullAccommodationExceptionMessage, s.StudentUSI));

                    var translatedAcc = GetTranslatedStringAccommodationToEnum(a.Name);
                    if (!model.AccommodationsList.Contains(translatedAcc))
                        model.AccommodationsList.Add(translatedAcc);
                }

                if(model.AccommodationsList.Count > 0)
                    res.Add(model);
            }

            return res;
        }

        private IEnumerable<StudentIndicator> GetStudentAccommodations(int schoolId, IEnumerable<long> studentUSIs)
        {
            // Resolve the School Id to the LEA Id, as that is what the Student Indicators are associated with
            var idNameModel = _idNameService.Get(School.IdNameRequest.Create(schoolId));
            int localEducationAgencyId = idNameModel.LocalEducationAgencyId;

            var indicatorsToLookup = GetListOfAccommodationToPullFromStudentIndicatorRepository().ToArray();

            //Don't filter by school because it is at the ISD level so we have a EducationOrganizationId
            var res = (from p in _studentIndicatorRepository.GetAll()
                       where (studentUSIs).Contains(p.StudentUSI) 
                       && p.EducationOrganizationId == localEducationAgencyId
                       && p.Status == true
                       && indicatorsToLookup.Contains(p.Name)
                       select p).ToList();

            // Late Enrollment and Partial Transcript
            var enrollmentData = (from data in _studentSchoolInformationRepository.GetAll()
                                      where (studentUSIs).Contains(data.StudentUSI) && data.SchoolId == schoolId && (data.LateEnrollment == Yes || data.IncompleteTranscript == Yes)
                                      select new
                                                {
                                                    data.StudentUSI,
                                                    data.LateEnrollment,
                                                    data.IncompleteTranscript
                                                }).ToList();

            res.AddRange(enrollmentData.Where(x=>x.LateEnrollment == Yes).Select(l => new StudentIndicator
                                                                                        {
                                                                                            EducationOrganizationId = localEducationAgencyId, 
                                                                                            StudentUSI = l.StudentUSI, 
                                                                                            Name = LateEnrollment, 
                                                                                            Status = true,
                                                                                        }));

            res.AddRange(enrollmentData.Where(x => x.IncompleteTranscript == Yes).Select(l => new StudentIndicator
                                                                                        {
                                                                                            EducationOrganizationId = localEducationAgencyId,
                                                                                            StudentUSI = l.StudentUSI,
                                                                                            Name = PartialTranscript,
                                                                                            Status = true,
                                                                                        }));

            return res;
        }

        protected virtual IEnumerable<string> GetListOfAccommodationToPullFromStudentIndicatorRepository()
        {
            yield return GiftedTalentedAccommodation;
            yield return SpecialEducationAccommodation;
            yield return PrimaryInstructionalSettingAccommodation;
            yield return SpecialEducationServicesAccommodation;
            yield return EnglishAsSecondLanguageAccommodation;
            yield return LimitedEnglishProficiencyAccommodation;
            yield return BilingualProgramAccommodation;
            yield return RepeaterAccommodation;
            yield return OverAgeAccommodation;
            yield return TestAccommodation;
            yield return DesignationAccommodation;
            yield return LimitedEnglishProficiencyMonitoredFirstAccommodation;
            yield return LimitedEnglishProficiencyMonitoredSecondAccommodation;
        }

        protected virtual Accommodations GetTranslatedStringAccommodationToEnum(string accommodationName)
        { 
            switch (accommodationName)
                {
                    case GiftedTalentedAccommodation:
                        return Accommodations.GiftedAndTalented;
                    //For Language any of these apply
                    case LimitedEnglishProficiencyAccommodation:
                    case EnglishAsSecondLanguageAccommodation:
                    case BilingualProgramAccommodation:                
                        return Accommodations.ESLAndLEP;
                    //For Special Services any of these apply
                    case SpecialEducationAccommodation:
                    case PrimaryInstructionalSettingAccommodation:
                    case SpecialEducationServicesAccommodation:
                        return Accommodations.SpecialEducation;
                    case OverAgeAccommodation:
                        return Accommodations.Overage;
                    case RepeaterAccommodation:
                        return Accommodations.Repeater;
                    case LateEnrollment:
                        return Accommodations.LateEnrollment;
                    case PartialTranscript:
                        return Accommodations.PartialTranscript;
                    case TestAccommodation:
                        return Accommodations.TestAccommodation;
                    case DesignationAccommodation:
                        return Accommodations.Designation;
                    case LimitedEnglishProficiencyMonitoredFirstAccommodation:
                        return Accommodations.LEPMonitoredFirst;
                    case LimitedEnglishProficiencyMonitoredSecondAccommodation:
                        return Accommodations.LEPMonitoredSecond;
                    default:
                        throw new ArgumentException(String.Format(AccommodationNotImplementedExceptionMessage, accommodationName), "accommodationName");
                }
        }
    }
}

// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources.Models.Student;

namespace EdFi.Dashboards.Resources.Models.Search
{
    [Serializable]
    public class QuickSearchModel
    {
        public QuickSearchModel()
        {
            Students = new List<StudentSearchItem>();
            Schools = new List<SearchItem>();
            Teachers = new List<SearchItem>();
        }

        public List<StudentSearchItem> Students { get; set; }
        public List<SearchItem> Schools { get; set; }
        public List<SearchItem> Teachers { get; set; }

        [Serializable]
        public class SearchItem
        {
            public long Id { get; set; }
            public string Text { get; set; }
            public Link Link { get; set; }
        }

        [Serializable]
        public class StudentSearchItem : SearchItem, IStudent
        {
            public StudentSearchItem() { }
			
            public StudentSearchItem(long studentUSI)
            {
                StudentUSI = studentUSI;
            }

            public long StudentUSI { get; set; }

            public string IdentificationCode { get; set; }
        }

        [IgnoreDataMember]
        public IQueryable<SearchModel.StudentSearchData> StudentQuery { get; set; }
    }
}

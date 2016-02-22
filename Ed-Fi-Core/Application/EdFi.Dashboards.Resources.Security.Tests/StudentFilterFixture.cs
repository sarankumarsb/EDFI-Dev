using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using EdFi.Dashboards.Resources.Models.Student;
using NUnit.Framework;

namespace EdFi.Dashboards.Resources.Security.Tests
{
    [TestFixture]
    public class StudentFilterFixture
    {
        private static readonly AccessibleStudents accessibleStudents = new AccessibleStudents()
        {
            CanAccessAllStudents = false,
            StudentUSIs = new HashSet<long>()
                    {
                        1,
                        2,
                        3,
                        4
                    }
        };

        [Test]
        public void Should_filter_students_as_values_in_a_given_dictionary()
        {
            var studentDictionary = new Dictionary<int, Student>()
                {
                    { 1, new Student{StudentUSI = 1} },
                    { 2, new Student{StudentUSI = 2} },
                    { 3, new Student{StudentUSI = 5} },
                };

            var actual = new Security.StudentFilter(accessibleStudents).ExecuteFilter(studentDictionary);

            CollectionAssert.AreEquivalent(new int[] { 1, 2 }, ((Dictionary<int, Student>)actual).Values.Select(x => x.StudentUSI).ToArray());
        }

        [Test]
        public void Should_filter_students_as_keys_in_a_given_dictionary()
        {
            var studentDictionary = new Dictionary<Student, int>()
                {
                    { new Student{StudentUSI = 1}, 1 },
                    { new Student{StudentUSI = 2}, 2 },
                    { new Student{StudentUSI = 5}, 5},
                };

            var actual = new Security.StudentFilter(accessibleStudents).ExecuteFilter(studentDictionary);

            CollectionAssert.AreEquivalent(new int[] { 1, 2 }, ((Dictionary<Student, int>)actual).Keys.Select(x => x.StudentUSI).ToArray());
        }

        [Test]
        public void Should_filter_students_given_generic_list()
        {
            var list = new List<Person>
                {
                    new Student{StudentUSI = 1} ,
                    new Student{StudentUSI = 2} ,
                    new Student{StudentUSI = 5} ,
                    new Teacher(),
                };

            var actual = (List<Person>)new Security.StudentFilter(accessibleStudents).ExecuteFilter(list);

            Assert.AreEqual(1, actual.OfType<Teacher>().Count());

            CollectionAssert.AreEquivalent(new int[] { 1, 2 }, actual.OfType<Student>().Select(x => x.StudentUSI).ToArray());
        }

        [Test]
        public void Should_filter_students_given_a_hashSet()
        {
            var list = new HashSet<Person>
                {
                    new Student{StudentUSI = 1} ,
                    new Student{StudentUSI = 2} ,
                    new Student{StudentUSI = 5} ,
                    new Teacher(),
                };

            var actual = (HashSet<Person>)new Security.StudentFilter(accessibleStudents).ExecuteFilter(list);

            Assert.AreEqual(1, actual.OfType<Teacher>().Count());

            CollectionAssert.AreEquivalent(new int[] { 1, 2 }, actual.OfType<Student>().Select(x => x.StudentUSI).ToArray());
        }

        [Test]
        public void Type_of_data_should_not_be_altered_after_filter()
        {
            var model = new Model()
            {
                OtherStudents = new Student[]
                        {
                            new Student{StudentUSI = 1} ,
                            new Student{StudentUSI = 2} ,
                            new Student{StudentUSI = 3} ,
                        }
            };
            var actual = (Model)new Security.StudentFilter(accessibleStudents).ExecuteFilter(model);

            Assert.AreEqual(model.OtherStudents.GetType(), actual.OtherStudents.GetType());
        }

     
        [Test]
        public void Should_filter_student_if_not_accessible()
        {
            var student = new Student() { StudentUSI = 100 };

            var filter = new Security.StudentFilter(accessibleStudents);
            var actual = filter.ExecuteFilter(student);

            Assert.IsNull(actual);
        }

        [Test]
        public void Should_not_filter_student_if_accessible()
        {
            var student = new Student() { StudentUSI = 1 };

            var filter = new Security.StudentFilter(accessibleStudents);
            var actual = filter.ExecuteFilter(student);

            Assert.IsNotNull(actual);
        }

        [Test]
        public void Should_filter_object_with_array()
        {
            var model = new Model()
            {
                OtherStudents = new[]
                        {
                            new Student{StudentUSI = 1} ,
                            new Student{StudentUSI = 2} ,
                            new Student{StudentUSI = 5} ,
                        }
            };


            var filter = new Security.StudentFilter(accessibleStudents);
            var actual = (Model)filter.ExecuteFilter(model);
            CollectionAssert.AreEquivalent(new[] { 1, 2 }, actual.OtherStudents.Select(x => x.StudentUSI).ToArray());
        }

        [Test]
        public void Should_filter_object_with_base_type()
        {
            var model = new ExtendedModel
            {
                Students = new List<Student>
                {
                    new Student {StudentUSI = 1},
                    new Student {StudentUSI = 2},
                    new Student {StudentUSI = 5},
                },
                ExtendedStudents = new List<Student>
                {
                    new Student {StudentUSI = 3},
                    new Student {StudentUSI = 6},
                }
            };

            var filter = new EdFi.Dashboards.Resources.Security.StudentFilter(accessibleStudents);
            var actual = (ExtendedModel)filter.ExecuteFilter(model);
            CollectionAssert.AreEquivalent(new[] { 1, 2 }, actual.Students.Select(x => x.StudentUSI).ToArray());
            CollectionAssert.AreEquivalent(new[] { 3 }, actual.ExtendedStudents.Select(x => x.StudentUSI).ToArray());
        }

        [Test]
        public void Should_filter_object_containing_list()
        {
            var model = new Model()
            {
                Students = new List<Student>
                        {
                            new Student{StudentUSI = 5} ,
                            new Student{StudentUSI = 2} ,
                            new Student{StudentUSI = 6} ,
                        }
            };

            var filter = new Security.StudentFilter(accessibleStudents);

            var actual = (Model)filter.ExecuteFilter(model);
            CollectionAssert.AreEquivalent(new int[] { 2 }, actual.Students.Select(x => x.StudentUSI).ToArray());
        }

        [Test]
        public void Should_filter_ienumerable()
        {
            var model = new ModelWithIEnumerable()
            {
                Students = new List<Student>
                        {
                            new Student{StudentUSI = 5} ,
                            new Student{StudentUSI = 2} ,
                            new Student{StudentUSI = 6} ,
                        }.Select(s => s)
            };

            var filter = new Security.StudentFilter(accessibleStudents);

            var actual = (ModelWithIEnumerable)filter.ExecuteFilter(model);
            CollectionAssert.AreEquivalent(new int[] { 2 }, actual.Students.Select(x => x.StudentUSI).ToArray());
        }

        [Test]
        public void Should_filter_iqueryable()
        {
            var model = new ModelWithIQueryable()
            {
                Students = new List<Student>
                        {
                            new Student{StudentUSI = 5} ,
                            new Student{StudentUSI = 2} ,
                            new Student{StudentUSI = 6} ,
                        }
                        .AsQueryable()
            };

            var filter = new Security.StudentFilter(accessibleStudents);

            var actual = (ModelWithIQueryable)filter.ExecuteFilter(model);
            CollectionAssert.AreEquivalent(new int[] { 2 }, actual.Students.Select(x => x.StudentUSI).ToArray());
        }

        [Test]
        public void Should_filter_iqueryable_backed_by_custom_queryable_implementation()
        {
            var model = new ModelWithIQueryable()
            {
                Students = new SomethingQueryable<Student>(
                        new List<Student>
                        {
                            new Student{StudentUSI = 5} ,
                            new Student{StudentUSI = 2} ,
                            new Student{StudentUSI = 6} ,
                        })
            };

            var filter = new Security.StudentFilter(accessibleStudents);

            var actual = (ModelWithIQueryable)filter.ExecuteFilter(model);
            CollectionAssert.AreEquivalent(new int[] { 2 }, actual.Students.Select(x => x.StudentUSI).ToArray());
        }
    }


    public class Person { }

    public class Teacher : Person
    {
    }

    public class Student : Person, IStudent
    {
        public long StudentUSI { get; set; }
    }

    public class Model : ParameterInfo
    {
        public List<Student> Students { get; set; }
        public Student[] OtherStudents { get; set; }
    }

    public class ExtendedModel : Model
    {
        public List<Student> ExtendedStudents { get; set; }
    }

    public class ModelWithIEnumerable
    {
        public IEnumerable<Student> Students { get; set; }
    }

    public class ModelWithIQueryable
    {
        public IQueryable<Student> Students { get; set; }
    }

    public class SomethingQueryable<T> : IQueryable<T>
    {
        private IQueryable<T> underlyingList;

        public SomethingQueryable(List<T> underlyingList)
        {
            this.underlyingList = underlyingList.AsQueryable();

            Expression = this.underlyingList.Expression;
            ElementType = this.underlyingList.ElementType;
            Provider = this.underlyingList.Provider;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return underlyingList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Expression Expression { get; private set; }
        public Type ElementType { get; private set; }
        public IQueryProvider Provider { get; private set; }
    }
}

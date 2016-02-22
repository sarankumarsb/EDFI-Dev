using System;
using EdFi.Dashboards.Infrastructure.Implementations;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using ServiceStack.Text;

namespace EdFi.Dashboards.Infrastructure.Tests.Implementations.Serialization
{
    public class When_serializing_and_deserializing_a_derived_class_where_the_base_class_type_is_provided_for_deserialization : TestFixtureBase
    {
        private Student originalStudent;

        private Person deserializedStudent;
        private Person genericallyDeserializedStudent;
        
        private Student clonedStudent;

        protected override void ExecuteTest()
        {
            originalStudent = new Student { FirstName = "Bob", BirthDate = new DateTime(2005, 6, 15), Grade = 2};

            var serializer = new ServiceStackTypeSerializer();
            var studentBytes = serializer.Serialize(originalStudent);

            // The serializer is told to use the base class (i.e. Person), but the deserialized versions should be instances of Student
            deserializedStudent = (Person) serializer.Deserialize(studentBytes, typeof(Person));
            genericallyDeserializedStudent = serializer.Deserialize<Person>(studentBytes);

            clonedStudent = serializer.DeepClone(originalStudent);
        }

        [Test]
        public virtual void Should_deserialize_derived_types_correctly_using_the_non_generic_method()
        {
            Assert.That(((Student)deserializedStudent).Dump(), Is.EqualTo(originalStudent.Dump()));
        }

        [Test]
        public virtual void Should_deserialize_derived_types_correctly_using_the_generic_method()
        {
            Assert.That(((Student)genericallyDeserializedStudent).Dump(), Is.EqualTo(originalStudent.Dump()));
        }

        [Test]
        public virtual void Should_clone_objects_correctly()
        {
            Assert.That(clonedStudent.Dump(), Is.EqualTo(originalStudent.Dump()));
        }
    }

    public class Person
    {
        public string FirstName { get; set; }
        public DateTime BirthDate { get; set; }
    }

    public class Student : Person
    {
        public int Grade { get; set; }
    }
}

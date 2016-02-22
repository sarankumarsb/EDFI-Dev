using System.Collections.Specialized;
using System.Runtime.InteropServices;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdFi.Dashboards.Resources.Security.Tests
{
    public abstract class SignInRequestAdornUtilityExtensionsFixture : TestFixtureBase
    {
        public static readonly string domain = "http://www.google.com";

        public static readonly int leaNumber = 1;
        public static readonly string leaNaN = "a";
        public static readonly string leaIdPairNumber = string.Concat(SignInRequestAdornUtility.Lea_Id, "=", leaNumber);
        public static readonly string leaIdPairNaN = string.Concat(SignInRequestAdornUtility.Lea_Id, "=", leaNaN);

        public static readonly string leaCode = "code";
        public static readonly string leaCodePair = string.Concat(SignInRequestAdornUtility.Lea_Code, "=", leaCode);

        public static readonly string leaName = "name";
        public static readonly string leaNamePair = string.Concat(SignInRequestAdornUtility.Lea_Name, "=", leaName);

        public static readonly string leaWimp = "234";
        public static readonly string leaWimpPair = string.Concat(SignInRequestAdornUtility.Wimp, "=", leaWimp);

        public static readonly string smallQuery1 = string.Concat(leaIdPairNumber, "&", leaNamePair);
        public static readonly string smallQuery2 = string.Concat(leaCodePair, "&", leaWimpPair);

        public static readonly string bigQuery = string.Concat(leaIdPairNumber, "&", leaCodePair, "&", leaNamePair, "&",
            leaWimpPair);

        public static readonly string Domain_QuestionMark_SmallQuery1 = string.Concat(domain, "?", smallQuery1);
        public static readonly string Domain_QuestionMark_SmallQuery2 = string.Concat(domain, "?", smallQuery2);
        public static readonly string Domain_QuestionMark_BigQuery = string.Concat(domain, "?", bigQuery);
        public static readonly string Domain_Questionmark_NoQuery = domain + "?";

        public static readonly string NoDomain_QuestionMark_SmallQuery1 = string.Concat("?", smallQuery1);
        public static readonly string NoDomain_QuestionMark_SmallQuery2 = string.Concat("?", smallQuery2);
        public static readonly string NoDomain_QuestionMark_BigQuery = string.Concat("?", bigQuery);

        public static readonly string NoDomain_QuestionMark_NoQuery = "?";
        public static readonly string NoDomain_NoQuestionMark_NoQuery = "";

        protected ISignInRequestAdornModel actualModel;

        protected override void ExecuteTest()
        {
            actualModel = SignInRequestAdornUtility.FromUrlQuery(GetUrl());
        }

        public void AssertValuesAreEqual(ISignInRequestAdornModel model, int id, string code, string name, string wimp)
        {
            Assert.That(model.LocalEducationAgencyCode, Is.EqualTo(code));
            Assert.That(model.LocalEducationAgencyId, Is.EqualTo(id));
            Assert.That(model.LocalEducationAgencyName, Is.EqualTo(name));
            Assert.That(model.Wimp, Is.EqualTo(wimp));
        }

        public abstract string GetUrl();
    }

    public class When_Calling_SignInRequestAdornUtilityExtensions_LeaID_NaN_With_Domain :
        SignInRequestAdornUtilityExtensionsFixture
    {
        public override string GetUrl()
        {
            return domain + "?" + leaIdPairNaN;
        }

        [Test]
        public void Should_Return_Empty()
        {
            Assert.That(actualModel, Is.EqualTo(SignInRequestAdornModel.Empty));
        }
    }


    public class When_Calling_SignInRequestAdornUtilityExtensions_LeaID_NaN_No_Domain :
        SignInRequestAdornUtilityExtensionsFixture
    {
        public override string GetUrl()
        {
            return "?" + leaIdPairNaN;
        }

        [Test]
        public void Should_Return_Empty()
        {
            Assert.That(actualModel, Is.EqualTo(SignInRequestAdornModel.Empty));
        }
    }

    public class When_Calling_SignInRequestAdornUtilityExtensions_LeaID_NaN_Only :
        SignInRequestAdornUtilityExtensionsFixture
    {
        public override string GetUrl()
        {
            return leaIdPairNaN;
        }

        [Test]
        public void Should_Return_Empty()
        {
            Assert.That(actualModel, Is.EqualTo(SignInRequestAdornModel.Empty));
        }
    }

    public class When_Calling_SignInRequestAdornUtilityExtensions_NoDomain_QuestionMark_NoQuery :
        SignInRequestAdornUtilityExtensionsFixture
    {
        public override string GetUrl()
        {
            return NoDomain_QuestionMark_NoQuery;
        }

        [Test]
        public void Should_Return_Empty()
        {
            Assert.That(actualModel, Is.EqualTo(SignInRequestAdornModel.Empty));
        }
    }

    public class When_Calling_SignInRequestAdornUtilityExtensions_NoDomain_NoQuestionMark_NoQuery :
        SignInRequestAdornUtilityExtensionsFixture
    {
        public override string GetUrl()
        {
            return NoDomain_NoQuestionMark_NoQuery;
        }

        [Test]
        public void Should_Return_Empty()
        {
            Assert.That(actualModel, Is.EqualTo(SignInRequestAdornModel.Empty));
        }
    }

    public class When_Calling_SignInRequestAdornUtilityExtensions_Domain_QuestionMark_SmallQuery1 :
        SignInRequestAdornUtilityExtensionsFixture
    {
        public override string GetUrl()
        {
            return Domain_QuestionMark_SmallQuery1;
        }

        [Test]
        public void Should_Return_Model()
        {
            AssertValuesAreEqual(actualModel, leaNumber, null, leaName, null);
        }
    }

    public class When_Calling_SignInRequestAdornUtilityExtensions_Domain_QuestionMark_SmallQuery2 :
        SignInRequestAdornUtilityExtensionsFixture
    {
        public override string GetUrl()
        {
            return Domain_QuestionMark_SmallQuery2;
        }

        [Test]
        public void Should_Return_Model()
        {
            AssertValuesAreEqual(actualModel, 0, leaCode, null, leaWimp);
        }
    }

    public class When_Calling_SignInRequestAdornUtilityExtensions_Domain_QuestionMark_BigQuery :
        SignInRequestAdornUtilityExtensionsFixture
    {
        public override string GetUrl()
        {
            return Domain_QuestionMark_BigQuery;
        }

        [Test]
        public void Should_Return_Model()
        {
            AssertValuesAreEqual(actualModel, leaNumber, leaCode, leaName, leaWimp);
        }
    }

    public class When_Calling_SignInRequestAdornUtilityExtensions_NoDomain_QuestionMark_SmallQuery1 :
        SignInRequestAdornUtilityExtensionsFixture
    {
        public override string GetUrl()
        {
            return NoDomain_QuestionMark_SmallQuery1;
        }

        [Test]
        public void Should_Return_Model()
        {
            AssertValuesAreEqual(actualModel, leaNumber, null, leaName, null);
        }
    }

    public class When_Calling_SignInRequestAdornUtilityExtensions_NoDomain_QuestionMark_SmallQuery2 :
        SignInRequestAdornUtilityExtensionsFixture
    {
        public override string GetUrl()
        {
            return NoDomain_QuestionMark_SmallQuery2;
        }

        [Test]
        public void Should_Return_Model()
        {
            AssertValuesAreEqual(actualModel, 0, leaCode, null, leaWimp);
        }
    }

    public class When_Calling_SignInRequestAdornUtilityExtensions_NoDomain_QuestionMark_BigQuery :
        SignInRequestAdornUtilityExtensionsFixture
    {
        public override string GetUrl()
        {
            return NoDomain_QuestionMark_BigQuery;
        }

        [Test]
        public void Should_Return_Model()
        {
            AssertValuesAreEqual(actualModel, leaNumber, leaCode, leaName, leaWimp);
        }
    }
}

﻿using Miru.Core;
using Miru.Makers;
using Miru.Testing;
using NUnit.Framework;

namespace Miru.Tests.Makers
{
    public class QueryMakerTest
    {
        private MiruPath _solutionDir;

        [SetUp]
        [TearDown]
        public void Setup()
        {
            _solutionDir = A.TempPath("Miru", "Shopifu");
            
            Directories.DeleteIfExists(_solutionDir);
        }
        
        [Test]
        public void Make_a_query()
        {
            // arrange
            var m = new Maker(new MiruSolution(_solutionDir));
            
            // act
            m.Query("Users", "User", "Show");
            
            // assert
            (m.Solution.FeaturesDir / "Users" / "UserShow.cs")
                .ShouldExistAndContains(
                    "namespace Shopifu.Features.Users",
                    "public class UserShow");
            
            (m.Solution.FeaturesDir / "Users" / "Show.cshtml")
                .ShouldExistAndContains(
                    "@model UserShow.Result");
            
            (m.Solution.AppTestsDir / "Features" / "Users" / "UserShowTest.cs")
                .ShouldExistAndContains(
                    "namespace Shopifu.Tests.Features.Users",
                    "public class UserShowTest : FeatureTest",
                    "public async Task Can_show_users()");
            
            (m.Solution.AppPageTestsDir / "Pages" / "Users" / "UserShowPageTest.cs")
                .ShouldExistAndContains(
                    "namespace Shopifu.PageTests.Pages.Users",
                    "public class UserShowPageTest : PageTest",
                    "public void Can_show_users()");
        }
        
        [Test]
        public void Make_a_query_in_sub_folders()
        {
            // arrange
            var m = new Maker(new MiruSolution(_solutionDir));
            
            // act
            m.Query("Admin/Report/Sales", "Sale", "Overview");
            
            // assert
            (m.Solution.FeaturesDir / "Admin" / "Report" / "Sales" / "SaleOverview.cs")
                .ShouldExistAndContains(
                    "namespace Shopifu.Features.Admin.Report.Sale",
                    "public class SaleOverview");
        }
    }
}
using System;
using System.IO;
using Miru.Core;
using Miru.Core.Makers;
using NUnit.Framework;
using Shouldly;

namespace Miru.Tests.Makers
{
    public class NewMakerTest
    {
        private MiruPath _tempDir;

        [SetUp]
        // [TearDown]
        public void Setup()
        {
            _tempDir = A.TempPath("Miru");
            
            Directories.DeleteIfExists(_tempDir);
        }

        [Test]
        public void Make_new_solution()
        {
            var m = Maker.For(_tempDir, "StackOverflow");

            m.New("StackOverflow");

            // check some main files
            File.Exists(_tempDir / "StackOverflow" / ".gitignore").ShouldBeTrue();
            
            File.ReadAllText(_tempDir / "StackOverflow" / "config" / "Config.Development.yml").ShouldContain("{{ db_dir }}StackOverflow_dev");
        }
        
        [Test]
        public void Cant_create_if_directory_already_exist()
        {
            Directories.CreateIfNotExists(_tempDir / "StackOverflow");
            
            var m = Maker.For(_tempDir);

            Should.Throw<InvalidOperationException>(() => m.New("StackOverflow"));
        }
    }
}
using Miru.Testing;

namespace Skeleton.Tests
{
    public class Program
    {
        public static int Main(string[] args) => new TestRunner().RunAssemblyOfType<Program>(args);
    }
}
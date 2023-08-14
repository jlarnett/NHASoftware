using System.Security.Claims;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHASoftware.Services.AccessWarden;

namespace NHA.Testing.AccessWardenTests
{
    [TestClass]
    public class AccessWardenTests
    {
        [DataTestMethod]
        [DataRow("test.png", true)]
        [DataRow("test.jpeg", true)]
        [DataRow("test.bmp", true)]
        [DataRow("test.pdf", false)]
        public void CheckUserIsAdmin(ClaimsPrincipal user)
        {
            IWarden accessWarden = new NHASoftware.Services.AccessWarden.AccessWarden();
            //Assert.AreEqual(expectedResult, result);
        }
    }
}

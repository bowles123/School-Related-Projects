using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CommSub;

namespace CommSubTesting
{
    [TestClass]
    public class ErrorListTester
    {
        [TestMethod]
        public void ErrorList_TestBasicFunctionality()
        {
            ErrorList errorList = new ErrorList();
            Assert.AreEqual(0, errorList.Count);
            Assert.AreEqual(100, errorList.Max);

            errorList.Max = 10;
            Assert.AreEqual(10, errorList.Max);

            errorList.Max = 0;
            Assert.AreEqual(100, errorList.Max);

            errorList.Max = -1;
            Assert.AreEqual(100, errorList.Max);

            errorList.Max = 6;
            Assert.AreEqual(6, errorList.Max);

            errorList.Add(Error.Get(Error.StandardErrorNumbers.AttackingProcessIsNotAPlayer));
            errorList.Add(Error.Get(Error.StandardErrorNumbers.CannotValidateProcess));
            errorList.Add(Error.Get(Error.StandardErrorNumbers.EndPointCannotBeNull));
            errorList.Add(Error.Get(Error.StandardErrorNumbers.GameAlreadyRegistered));
            errorList.Add(Error.Get(Error.StandardErrorNumbers.InvalidGameId));

            Assert.AreEqual(5, errorList.Count);
            Error[] errs = errorList.All;
            Assert.IsNotNull(errs);
            Assert.AreEqual(5, errs.Length);
            Assert.AreEqual(Error.StandardErrorNumbers.AttackingProcessIsNotAPlayer, errs[0].Number);
            Assert.AreEqual(Error.StandardErrorNumbers.CannotValidateProcess, errs[1].Number);
            Assert.AreEqual(Error.StandardErrorNumbers.EndPointCannotBeNull, errs[2].Number);
            Assert.AreEqual(Error.StandardErrorNumbers.GameAlreadyRegistered, errs[3].Number);
            Assert.AreEqual(Error.StandardErrorNumbers.InvalidGameId, errs[4].Number);

            errorList.Add(Error.Get(Error.StandardErrorNumbers.InvalidProcessId));
            errs = errorList.All;
            Assert.IsNotNull(errs);
            Assert.AreEqual(6, errs.Length);
            Assert.AreEqual(Error.StandardErrorNumbers.AttackingProcessIsNotAPlayer, errs[0].Number);
            Assert.AreEqual(Error.StandardErrorNumbers.CannotValidateProcess, errs[1].Number);
            Assert.AreEqual(Error.StandardErrorNumbers.EndPointCannotBeNull, errs[2].Number);
            Assert.AreEqual(Error.StandardErrorNumbers.GameAlreadyRegistered, errs[3].Number);
            Assert.AreEqual(Error.StandardErrorNumbers.InvalidGameId, errs[4].Number);
            Assert.AreEqual(Error.StandardErrorNumbers.InvalidProcessId, errs[5].Number);

            errorList.Add(Error.Get(Error.StandardErrorNumbers.InvalidResource));
            errs = errorList.All;
            Assert.IsNotNull(errs);
            Assert.AreEqual(6, errs.Length);
            Assert.AreEqual(Error.StandardErrorNumbers.CannotValidateProcess, errs[0].Number);
            Assert.AreEqual(Error.StandardErrorNumbers.EndPointCannotBeNull, errs[1].Number);
            Assert.AreEqual(Error.StandardErrorNumbers.GameAlreadyRegistered, errs[2].Number);
            Assert.AreEqual(Error.StandardErrorNumbers.InvalidGameId, errs[3].Number);
            Assert.AreEqual(Error.StandardErrorNumbers.InvalidProcessId, errs[4].Number);
            Assert.AreEqual(Error.StandardErrorNumbers.InvalidResource, errs[5].Number);


            errorList.Add(Error.Get(Error.StandardErrorNumbers.InvalidProcessId));
            errs = errorList.All;
            Assert.IsNotNull(errs);
            Assert.AreEqual(6, errs.Length);
            Assert.AreEqual(Error.StandardErrorNumbers.EndPointCannotBeNull, errs[0].Number);
            Assert.AreEqual(Error.StandardErrorNumbers.GameAlreadyRegistered, errs[1].Number);
            Assert.AreEqual(Error.StandardErrorNumbers.InvalidGameId, errs[2].Number);
            Assert.AreEqual(Error.StandardErrorNumbers.InvalidProcessId, errs[3].Number);
            Assert.AreEqual(Error.StandardErrorNumbers.InvalidResource, errs[4].Number);
            Assert.AreEqual(Error.StandardErrorNumbers.InvalidProcessId, errs[5].Number);

            errorList.Clear();
            Assert.AreEqual(0, errorList.Count);
        }
    }
}

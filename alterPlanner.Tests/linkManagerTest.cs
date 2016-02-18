using alter.iface;
// <copyright file="linkManagerTest.cs">Copyright ©  2016</copyright>

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using alter.Link.classes;

namespace alter.Link.classes.Tests
{
    [TestClass]
    [PexClass(typeof(linkManager))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class linkManagerTest
    {

        [PexMethod]
        public linkManager Constructor(IId owner)
        {
            linkManager target = new linkManager(owner);
            return target;
            // TODO: добавление проверочных утверждений в метод linkManagerTest.Constructor(IId)
        }
    }
}

using alter.Service.Exceptions;
using alter.iface;
// <copyright file="linkManagerTest.cs">Copyright ©  2016</copyright>

using System;
using alter.classes;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using alter.Link.classes;
using alter.types;

namespace alter.Link.classes.Tests
{
    [TestClass]
    [PexClass(typeof(linkManager))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(NullReferenceException))]
    public partial class linkManagerTest
    {
        [PexMethod]
        [PexArguments(e_Entity.Task, e_Entity.Group, e_Entity.Link)]
        [PexAllowedException(typeof(wrongObjectEntityException))]
        public linkManager Constructor(e_Entity entity)
        {
            IId owner = new Identity(entity);
            linkManager target = new linkManager(owner);
            return target;
            // TODO: добавление проверочных утверждений в метод linkManagerTest.Constructor(IId)
        }
    }
}

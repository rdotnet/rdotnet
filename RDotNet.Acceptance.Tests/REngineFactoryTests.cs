using System;
using System.Collections.Generic;
using System.ServiceModel;
using RDotNet.Client;
using RDotNet.Server;
using Moq;
using NUnit.Framework;

namespace RDotNet.Acceptance.Tests
{
    [TestFixture]
    public class REngineFactoryTests
    {
        [Test]
        public void ProperlyConfiguresLocalInstance()
        {
            var mli = new Mock<ILocalInstance>();
            var mhc = new Mock<IHostConfiguration>();
            var uri = new Uri("net.pipe://localhost");
            mhc.Setup(s => s.GetProcessType()).Returns("InProcess");
            mhc.Setup(g => g.CreateUriFromConfiguration()).Returns(uri);
            var maf = new Mock<IAPIFactory>();
            maf.Setup(s => s.CreateRManagementAPI(It.IsAny<int>())).Returns(new Mock<IRManagementAPI>().Object);
            maf.Setup(s => s.CreateRObjectAPI(It.IsAny<int>())).Returns(new Mock<IRObjectAPI>().Object);
            maf.Setup(s => s.CreateROutputAPI(It.IsAny<int>())).Returns(new Mock<IROutputAPI>().Object);
            mhc.Setup(g => g.CreateBindingFromConfiguration()).Returns(new NetNamedPipeBinding());
            var factory = new REngineFactory(mli.Object, new Mock<ProcessFactory>().Object, mhc.Object, maf.Object);

            var engine = factory.CreateInstance(0);

            Assert.That(engine.ID, Is.Not.EqualTo(0));
            mli.Verify(v => v.Create(It.Is<Uri>(u => u.Equals(uri)), It.IsAny<int>()), Times.AtLeastOnce());
        }

        [Test]
        public void DoesntCreateMultipleLocalInstances()
        {
            var mli = new Mock<ILocalInstance>();
            var mhc = new Mock<IHostConfiguration>();
            var uri = new Uri("net.pipe://localhost");
            mhc.Setup(s => s.GetProcessType()).Returns("InProcess");
            mhc.Setup(g => g.CreateUriFromConfiguration()).Returns(uri);
            mhc.Setup(g => g.CreateBindingFromConfiguration()).Returns(new NetNamedPipeBinding());
            var maf = new Mock<IAPIFactory>();
            maf.Setup(s => s.CreateRManagementAPI(It.IsAny<int>())).Returns(new Mock<IRManagementAPI>().Object);
            maf.Setup(s => s.CreateRObjectAPI(It.IsAny<int>())).Returns(new Mock<IRObjectAPI>().Object);
            maf.Setup(s => s.CreateROutputAPI(It.IsAny<int>())).Returns(new Mock<IROutputAPI>().Object);
            var factory = new REngineFactory(mli.Object, new Mock<ProcessFactory>().Object, mhc.Object, maf.Object);

            var engine = factory.CreateInstance(0);
            var engine2 = factory.CreateInstance(0);

            Assert.That(engine.ID, Is.EqualTo(engine2.ID));
            mli.Verify(v => v.Create(It.Is<Uri>(u => u == uri), It.IsAny<int>()), Times.Once());
        }

        [Test]
        public void OnlyInitializesTheServerOnceForInProcess()
        {
            var mli = new Mock<ILocalInstance>();
            var mhc = new Mock<IHostConfiguration>();
            var uri = new Uri("net.pipe://localhost");
            mhc.Setup(s => s.GetProcessType()).Returns("InProcess");
            mhc.Setup(g => g.CreateUriFromConfiguration()).Returns(uri);
            mhc.Setup(g => g.CreateBindingFromConfiguration()).Returns(new NetNamedPipeBinding());
            var mapi = new Mock<IRManagementAPI>();
            var maf = new Mock<IAPIFactory>();
            maf.Setup(s => s.CreateRManagementAPI(It.IsAny<int>())).Returns(mapi.Object);
            maf.Setup(s => s.CreateRObjectAPI(It.IsAny<int>())).Returns(new Mock<IRObjectAPI>().Object);
            maf.Setup(s => s.CreateROutputAPI(It.IsAny<int>())).Returns(new Mock<IROutputAPI>().Object);
            var factory = new REngineFactory(mli.Object, new Mock<ProcessFactory>().Object, mhc.Object, maf.Object);

            factory.CreateInstance(0);
            factory.CreateInstance(0);

            mapi.Verify(v => v.Start(It.IsAny<StartupParameter>()), Times.Once());
        }

        [Test]
        public void ProperlyConfiguresOutOfProcessInstance()
        {
            var process = new Mock<IProcess>();
            process.SetupGet(s => s.HasExited).Returns(false);
            process.SetupGet(s => s.MainWindowTitle).Returns("State: Ready Instance: 4321 RootURI: net.pipe://localhost/");
            var mpf = new Mock<IProcessFactory>();
            mpf.Setup(s => s.GetProcessesByName(It.IsAny<string>())).Returns(new List<IProcess>());
            mpf.Setup(s => s.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IDictionary<string, string>>())).Returns(process.Object);
            var mhc = new Mock<IHostConfiguration>();
            mhc.Setup(s => s.GetProcessType()).Returns("OutOfProcess");
            mhc.Setup(g => g.CreateUriFromConfiguration()).Returns(new Uri("net.pipe://localhost/"));
            mhc.Setup(g => g.CreateBindingFromConfiguration()).Returns(new NetNamedPipeBinding());
            var maf = new Mock<IAPIFactory>();
            maf.Setup(s => s.CreateRManagementAPI(It.IsAny<int>())).Returns(new Mock<IRManagementAPI>().Object);
            maf.Setup(s => s.CreateRObjectAPI(It.IsAny<int>())).Returns(new Mock<IRObjectAPI>().Object);
            maf.Setup(s => s.CreateROutputAPI(It.IsAny<int>())).Returns(new Mock<IROutputAPI>().Object);
            var factory = new REngineFactory(new Mock<ILocalInstance>().Object, mpf.Object, mhc.Object, maf.Object);

            var engine = factory.CreateInstance(0);

            Assert.That(engine.ID, Is.Not.EqualTo(0));
        }

        [Test]
        public void DoesntKillUnresponsiveProcess()
        {
            var process = new Mock<IProcess>();
            process.SetupGet(s => s.HasExited).Returns(false);
            process.SetupGet(s => s.MainWindowTitle).Returns("State: Ready Instance: 4321 RootURI: net.pipe://localhost/");
            var mpf = new Mock<IProcessFactory>();
            mpf.Setup(s => s.GetProcessesByName(It.IsAny<string>())).Returns(new List<IProcess> { process.Object });
            mpf.Setup(s => s.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IDictionary<string, string>>())).Returns(process.Object);
            var mhc = new Mock<IHostConfiguration>();
            mhc.Setup(s => s.GetProcessType()).Returns("OutOfProcess");
            mhc.Setup(g => g.CreateUriFromConfiguration()).Returns(new Uri("net.pipe://localhost"));
            mhc.Setup(g => g.CreateBindingFromConfiguration()).Returns(new NetNamedPipeBinding());
            var mapi = new Mock<IRManagementAPI>();
            mapi.Setup(s => s.IsAlive()).Returns(false);
            var maf = new Mock<IAPIFactory>();
            maf.Setup(s => s.CreateRManagementAPI(It.IsAny<int>())).Returns(mapi.Object);
            maf.Setup(s => s.CreateRObjectAPI(It.IsAny<int>())).Returns(new Mock<IRObjectAPI>().Object);
            maf.Setup(s => s.CreateROutputAPI(It.IsAny<int>())).Returns(new Mock<IROutputAPI>().Object);
            var factory = new REngineFactory(new Mock<ILocalInstance>().Object, mpf.Object, mhc.Object, maf.Object);

            factory.CreateInstance(4321);

            process.Verify(v => v.Kill());
        }


        [Test]
        public void DoesntKillProcessWithMissingInstanceToken()
        {
            var process = new Mock<IProcess>();
            process.SetupGet(s => s.HasExited).Returns(false);
            process.SetupGet(s => s.MainWindowTitle).Returns("State: Ready RootURI: net.pipe://localhost/");
            var mpf = new Mock<IProcessFactory>();
            mpf.Setup(s => s.GetProcessesByName(It.IsAny<string>())).Returns(new List<IProcess> { process.Object });
            mpf.Setup(s => s.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IDictionary<string, string>>())).Returns(process.Object);
            var mhc = new Mock<IHostConfiguration>();
            mhc.Setup(s => s.GetProcessType()).Returns("OutOfProcess");
            mhc.Setup(g => g.CreateUriFromConfiguration()).Returns(new Uri("net.pipe://localhost"));
            mhc.Setup(g => g.CreateBindingFromConfiguration()).Returns(new NetNamedPipeBinding());
            var mapi = new Mock<IRManagementAPI>();
            mapi.Setup(s => s.IsAlive()).Returns(true);
            var maf = new Mock<IAPIFactory>();
            maf.Setup(s => s.CreateRManagementAPI(It.IsAny<int>())).Returns(mapi.Object);
            maf.Setup(s => s.CreateRObjectAPI(It.IsAny<int>())).Returns(new Mock<IRObjectAPI>().Object);
            maf.Setup(s => s.CreateROutputAPI(It.IsAny<int>())).Returns(new Mock<IROutputAPI>().Object);
            var factory = new REngineFactory(new Mock<ILocalInstance>().Object, mpf.Object, mhc.Object, maf.Object);

            factory.CreateInstance(4321);

            process.Verify(v => v.Kill());
        }

        [Test]
        public void DoesntKillProcessWithMissingInstanceNumber()
        {
            var process = new Mock<IProcess>();
            process.SetupGet(s => s.HasExited).Returns(false);
            process.SetupGet(s => s.MainWindowTitle).Returns("State: Ready Instance: RootURI: net.pipe://localhost/");
            var mpf = new Mock<IProcessFactory>();
            mpf.Setup(s => s.GetProcessesByName(It.IsAny<string>())).Returns(new List<IProcess> { process.Object });
            mpf.Setup(s => s.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IDictionary<string, string>>())).Returns(process.Object);
            var mhc = new Mock<IHostConfiguration>();
            mhc.Setup(s => s.GetProcessType()).Returns("OutOfProcess");
            mhc.Setup(g => g.CreateUriFromConfiguration()).Returns(new Uri("net.pipe://localhost"));
            mhc.Setup(g => g.CreateBindingFromConfiguration()).Returns(new NetNamedPipeBinding());
            var mapi = new Mock<IRManagementAPI>();
            mapi.Setup(s => s.IsAlive()).Returns(true);
            var maf = new Mock<IAPIFactory>();
            maf.Setup(s => s.CreateRManagementAPI(It.IsAny<int>())).Returns(mapi.Object);
            maf.Setup(s => s.CreateRObjectAPI(It.IsAny<int>())).Returns(new Mock<IRObjectAPI>().Object);
            maf.Setup(s => s.CreateROutputAPI(It.IsAny<int>())).Returns(new Mock<IROutputAPI>().Object);
            var factory = new REngineFactory(new Mock<ILocalInstance>().Object, mpf.Object, mhc.Object, maf.Object);

            factory.CreateInstance(4321);

            process.Verify(v => v.Kill());
        }

        [Test]
        public void ExceptionWhenProcessABEND()
        {
            var process = new Mock<IProcess>();
            process.SetupGet(s => s.HasExited).Returns(true);
            process.SetupGet(s => s.MainWindowTitle).Returns("State: Ready Instance: RootURI: net.pipe://localhost/");
            var mpf = new Mock<IProcessFactory>();
            mpf.Setup(s => s.GetProcessesByName(It.IsAny<string>())).Returns(new List<IProcess> { process.Object });
            mpf.Setup(s => s.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IDictionary<string, string>>())).Returns(process.Object);
            var mhc = new Mock<IHostConfiguration>();
            mhc.Setup(s => s.GetProcessType()).Returns("OutOfProcess");
            mhc.Setup(g => g.CreateUriFromConfiguration()).Returns(new Uri("net.pipe://localhost"));
            mhc.Setup(g => g.CreateBindingFromConfiguration()).Returns(new NetNamedPipeBinding());
            var mapi = new Mock<IRManagementAPI>();
            mapi.Setup(s => s.IsAlive()).Returns(true);
            var maf = new Mock<IAPIFactory>();
            maf.Setup(s => s.CreateRManagementAPI(It.IsAny<int>())).Returns(mapi.Object);
            maf.Setup(s => s.CreateRObjectAPI(It.IsAny<int>())).Returns(new Mock<IRObjectAPI>().Object);
            maf.Setup(s => s.CreateROutputAPI(It.IsAny<int>())).Returns(new Mock<IROutputAPI>().Object);
            var factory = new REngineFactory(new Mock<ILocalInstance>().Object, mpf.Object, mhc.Object, maf.Object);

            Assert.Throws<InvalidOperationException>(() => factory.CreateInstance(4321));
        }

        [Test]
        public void DoesntKillWrongProcess()
        {
            var process = new Mock<IProcess>();
            process.SetupGet(s => s.HasExited).Returns(false);
            const int localId = 4321;
            process.SetupGet(s => s.MainWindowTitle).Returns(() => string.Format("State: Ready Instance: {0} RootURI: net.pipe://localhost/", localId));
            var mpf = new Mock<IProcessFactory>();
            mpf.Setup(s => s.GetProcessesByName(It.IsAny<string>())).Returns(new List<IProcess> { process.Object });
            mpf.Setup(s => s.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IDictionary<string, string>>())).Returns(process.Object);
            var mhc = new Mock<IHostConfiguration>();
            mhc.Setup(s => s.GetProcessType()).Returns("OutOfProcess");
            mhc.Setup(g => g.CreateUriFromConfiguration()).Returns(new Uri("net.pipe://localhost"));
            mhc.Setup(g => g.CreateBindingFromConfiguration()).Returns(new NetNamedPipeBinding());
            var mapi = new Mock<IRManagementAPI>();
            mapi.Setup(s => s.IsAlive()).Returns(true);
            var maf = new Mock<IAPIFactory>();
            maf.Setup(s => s.CreateRManagementAPI(It.IsAny<int>())).Returns(mapi.Object);
            maf.Setup(s => s.CreateRObjectAPI(It.IsAny<int>())).Returns(new Mock<IRObjectAPI>().Object);
            maf.Setup(s => s.CreateROutputAPI(It.IsAny<int>())).Returns(new Mock<IROutputAPI>().Object);
            var factory = new REngineFactory(new Mock<ILocalInstance>().Object, mpf.Object, mhc.Object, maf.Object);

            factory.CreateInstance(9876);

            process.Verify(v => v.Kill(), Times.Never());
        }

        [Test]
        public void ExceptionWhenProcessIsNull()
        {
            var process = new Mock<IProcess>();
            process.SetupGet(s => s.HasExited).Returns(true);
            process.SetupGet(s => s.MainWindowTitle).Returns("State: Ready Instance: RootURI: net.pipe://localhost/");
            var mpf = new Mock<IProcessFactory>();
            mpf.Setup(s => s.GetProcessesByName(It.IsAny<string>())).Returns(new List<IProcess> { process.Object });
            mpf.Setup(s => s.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IDictionary<string, string>>())).Returns((IProcess)null);
            var mhc = new Mock<IHostConfiguration>();
            mhc.Setup(s => s.GetProcessType()).Returns("OutOfProcess");
            mhc.Setup(g => g.CreateUriFromConfiguration()).Returns(new Uri("net.pipe://localhost"));
            mhc.Setup(g => g.CreateBindingFromConfiguration()).Returns(new NetNamedPipeBinding());
            var mapi = new Mock<IRManagementAPI>();
            mapi.Setup(s => s.IsAlive()).Returns(true);
            var maf = new Mock<IAPIFactory>();
            maf.Setup(s => s.CreateRManagementAPI(It.IsAny<int>())).Returns(mapi.Object);
            maf.Setup(s => s.CreateRObjectAPI(It.IsAny<int>())).Returns(new Mock<IRObjectAPI>().Object);
            maf.Setup(s => s.CreateROutputAPI(It.IsAny<int>())).Returns(new Mock<IROutputAPI>().Object);
            var factory = new REngineFactory(new Mock<ILocalInstance>().Object, mpf.Object, mhc.Object, maf.Object);

            Assert.Throws<InvalidOperationException>(() => factory.CreateInstance(4321));
        }

        [Test]
        public void OnlyInitializesTheServerOnceForOutOfProcess()
        {
            var process = new Mock<IProcess>();
            process.SetupGet(s => s.HasExited).Returns(false);
            process.SetupGet(s => s.MainWindowTitle).Returns("State: Ready Instance: 4321 RootURI: net.pipe://localhost/");
            var mpf = new Mock<IProcessFactory>();
            mpf.Setup(s => s.GetProcessesByName(It.IsAny<string>())).Returns(new List<IProcess> { process.Object });
            mpf.Setup(s => s.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IDictionary<string, string>>())).Returns(process.Object);
            var mhc = new Mock<IHostConfiguration>();
            mhc.Setup(s => s.GetProcessType()).Returns("OutOfProcess");
            mhc.Setup(g => g.CreateUriFromConfiguration()).Returns(new Uri("net.pipe://localhost"));
            mhc.Setup(g => g.CreateBindingFromConfiguration()).Returns(new NetNamedPipeBinding());
            var mapi = new Mock<IRManagementAPI>();
            mapi.Setup(s => s.IsAlive()).Returns(true);
            var maf = new Mock<IAPIFactory>();
            maf.Setup(s => s.CreateRManagementAPI(It.IsAny<int>())).Returns(mapi.Object);
            maf.Setup(s => s.CreateRObjectAPI(It.IsAny<int>())).Returns(new Mock<IRObjectAPI>().Object);
            maf.Setup(s => s.CreateROutputAPI(It.IsAny<int>())).Returns(new Mock<IROutputAPI>().Object);
            var factory = new REngineFactory(new Mock<ILocalInstance>().Object, mpf.Object, mhc.Object, maf.Object);

            factory.CreateInstance(4321);

            mapi.Verify(v => v.Start(It.IsAny<StartupParameter>()), Times.Never());
        }
    }
}

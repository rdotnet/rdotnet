using System;
using System.ServiceModel;
using RDotNet.Client.RLanguageService;
using RDotNet.Client.RManagementService;
using RDotNet.Client.ROutputService;
using RDotNet.R.Adapter;
using RDotNet.Server;
using FluentAssertions;
using NUnit.Framework;

namespace RDotNet.Acceptance.Tests
{
    [Category("Integration")]
    [Description("Integration tests to verify the local configuration of the WCF services.")]
    public class WCFServiceTests
    {
        private RLanguageServiceClient _ls;
        private ROutputServiceClient _os;

        public void SetFixture(REngineFixture data)
        {
            _ls = data.LanguageService;
            _os = data.OutputService;
        }

        [Test]
        public void ExecuteGoodRCode()
        {
            var ge = _ls.GetGlobalEnvironment();

            const string msg = "sessionInfo()\n";
            var s = _ls.MakeString(msg);
            _ls.Protect(s);

            ParseStatus status;
            var vector = _ls.ParseVector(s, -1, out status);
            status.Should().Be(ParseStatus.Ok);

            _ls.Protect(vector);

            var expression = _ls.GetExpressionVectorValueAt(vector, 0);

            bool errorOccurred;
            var evaluated = _ls.TryEvaluate(expression, ge, out errorOccurred);
            errorOccurred.Should().BeFalse();

            _ls.Protect(evaluated);

            var messages = _ls.CoerceVector(evaluated, SymbolicExpressionType.CharacterVector);
            _ls.Protect(messages);

            var message = _ls.GetCharacterVectorValueAt(messages, 0);

            _ls.Unprotect(4);

            message.Should().Contain("R version 3.1.0");
        }

        [Test]
        public void ExecutesBadCodeAndGetsErrorMessage()
        {

            const string msg = "a -< c(1,2,3,4,5)\n";
            var s = _ls.MakeString(msg);
            _ls.Protect(s);

            ParseStatus status;
            _ls.ParseVector(s, -1, out status);
            status.Should().Be(ParseStatus.Error);

            var message = _ls.GetParseError();
            message.Should().Contain("unexpected '<'");

            _ls.Unprotect(1);
        }

        public class REngineFixture : IDisposable
        {
            private readonly ServiceHost _lHost;
            private readonly ServiceHost _mHost;
            private readonly ServiceHost _oHost;

            public RManagementServiceClient ManagementService { get; private set; }
            public RLanguageServiceClient LanguageService { get; private set; }
            public ROutputServiceClient OutputService { get; private set; }

            public REngineFixture()
            {
                _lHost = new ServiceHost(typeof(Server.Services.RManagementService), new Uri("net.pipe://localhost/RManagementService"));
                _mHost = new ServiceHost(typeof(Server.Services.RLanguageService), new Uri("net.pipe://localhost/RLanguageService"));
                _oHost = new ServiceHost(typeof(Server.Services.ROutputService), new Uri("net.pipe://localhost/ROutputService"));

                _mHost.Open();
                _lHost.Open();
                _oHost.Open();

                ManagementService = new RManagementServiceClient(new NetNamedPipeBinding(), new EndpointAddress("net.pipe://localhost/RManagementService"));
                LanguageService = new RLanguageServiceClient(new NetNamedPipeBinding(), new EndpointAddress("net.pipe://localhost/RLanguageService"));
                OutputService = new ROutputServiceClient(new NetNamedPipeBinding(), new EndpointAddress("net.pipe://localhost/ROutputService"));

                ManagementService.Start(StartupParameter.Default);

                var alive = ManagementService.IsAlive();
                alive.Should().BeTrue();
            }

            public void Dispose()
            {
                ManagementService.ForceGarbageCollection();
                ManagementService.Abort();

                _mHost.Close();
                _lHost.Close();
                _oHost.Close();
            }
        }
    }
}

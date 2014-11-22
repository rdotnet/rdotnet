using System;
using System.Collections.Generic;
using System.Linq;
using RDotNet.R.Adapter;
using RDotNet.Server.CharacterDevices;
using RDotNet.Server.GraphicsDevices;

namespace RDotNet.Server.Services
{
    public class ROutputService : IROutputService
    {
        private readonly RInstance _rInstance;

        public ROutputService()
            : this(new RInstanceManager())
        { }

        public ROutputService(IRInstanceManager instanceManager)
        {
            if (instanceManager == null) throw new ArgumentNullException("instanceManager");
            _rInstance = instanceManager.GetInstance();

            _rInstance.AddGraphicsDevice(new ServerSvgGraphicsDevice());
        }

        public void ClearAll()
        {
            ClearText();
            ClearPlots();
        }

        public void ClearPlots()
        {
            _rInstance.GraphicsDevices.ForEach(d => d.Clear());
        }

        public void ClearText()
        {
            _rInstance.CharacterDevice.Clear();
        }

        public int GetPendingPlotCount()
        {
            var plots = GetAllPlots();
            return plots.Count();
        }

        public IEnumerable<string> GetAllPlots()
        {
            var result = _rInstance.GraphicsDevices.SelectMany(d => d.GetPlots());
            return result;
        }

        public IEnumerable<string> GetText()
        {
            var buffer = _rInstance.CharacterDevice as ICachedOutput;
            if (buffer != null) return buffer.GetOutput();

            return Enumerable.Empty<string>();
        }
    }
}
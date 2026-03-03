using System;
using System.Linq;
using TMPro;
using UniRx;
using UniRx.Diagnostics;

namespace Assets.Features.Core
{
    public class LogHandler : IDisposable
    {
        private Logger _logger;
        private TMP_Text _logTextComponent;
        private CompositeDisposable _disposed = new CompositeDisposable();
        public LogHandler(Logger logger, UIMainWindow uIMainWindow)
        { 
            _logTextComponent = uIMainWindow.LogTextComponent;
            _logger = logger;

            ObservableLogger.Listener.LogToUnityDebug().AddTo(_disposed);

            ObservableLogger.Listener
                .Where(x => x.LogType == UnityEngine.LogType.Log)
                .Subscribe(x => 
                {
                    _logTextComponent.text = x.Message;
                })
                .AddTo(_disposed);
        }
        public void Dispose()
        {
            _logger = null;
            _logTextComponent = null;            

            _disposed?.Dispose();
            _disposed = null;
        }
    }
}

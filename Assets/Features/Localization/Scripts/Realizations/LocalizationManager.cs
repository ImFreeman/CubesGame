using Assets.Features.Localization.Scripts.Interfaces;
using System;
using System.Collections.Generic;

namespace Assets.Features.Localization.Scripts.Realizations
{
    public class LocalizationManager : IDisposable, ILocalizationManager
    {
        private const string ErrorMessage = "No text with such key";
        private IDictionary<string, string> _localizeData = new Dictionary<string, string>();
        public LocalizationManager(LocalizationConfig config)
        {
            //TODO: adressables
            foreach (var item in config.Data)
            {
                _localizeData.TryAdd(item.Key, item.Text);
            }
        }

        public string Localize(string key)
        {
            try
            {
                if(_localizeData.TryGetValue(key, out var value))
                {
                    return value;
                }

                throw new Exception(ErrorMessage);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public void Dispose()
        {
            
        }        
    }
}

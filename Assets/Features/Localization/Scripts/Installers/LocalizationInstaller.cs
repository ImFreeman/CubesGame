using Assets.Features.Localization.Scripts.Interfaces;
using Assets.Features.Localization.Scripts.Realizations;
using UnityEngine;
using Zenject;

namespace Assets.Features.Localization.Scripts.Installers
{
    public class LocalizationInstaller : MonoInstaller
    {
        [SerializeField] private LocalizationConfig _language;
        public override void InstallBindings()
        {
            Container
                .Bind<LocalizationConfig>()
                .FromScriptableObject(_language)
                .AsSingle();

            Container
                .Bind<ILocalizationManager>()
                .To<LocalizationManager>()
                .AsSingle();
        }
    }
}
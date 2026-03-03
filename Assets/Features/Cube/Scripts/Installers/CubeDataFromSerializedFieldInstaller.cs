using UnityEngine;
using Zenject;

namespace Assets.Features.Cube.Scripts.Installers
{
    public class CubeDataFromSerializedFieldInstaller : MonoInstaller
    {
        [SerializeField] private CubeConfig _config;
        [SerializeField] private CubeView _view;
        [SerializeField] private Transform _poolParent;

        public override void InstallBindings()
        {
            Container
                .Bind<CubeConfig>()
                .FromScriptableObject(_config)
                .AsSingle();

            CubeInstaller.Install(Container, _view, _poolParent);
        }
    }
}
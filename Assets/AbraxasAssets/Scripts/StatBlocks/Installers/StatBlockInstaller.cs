
﻿using Abraxas.StackBlocks.Views;

using Abraxas.StatBlocks.Controllers;
using Abraxas.StatBlocks.Data;
using Abraxas.StatBlocks.Factories;
using Abraxas.StatBlocks.Models;
using Abraxas.StatBlocks.Views;


using Zenject;

namespace Abraxas.StatBlocks.Installers
{

    /// <summary>
    /// StatBlockInstaller is a Unity-Specific Zenject installer for the stat block system.
    /// </summary>

    class StatBlockInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<StatBlockView>().AsTransient();
            Container.BindInterfacesAndSelfTo<StatBlockController>().AsTransient();
            Container.BindInterfacesAndSelfTo<StatBlockModel>().AsTransient();

            Container.BindFactory<StatBlockData, IStatBlockView, IStatBlockController, StatBlockController.Factory>().FromFactory<StatBlockFactory>();
        }
    }
}

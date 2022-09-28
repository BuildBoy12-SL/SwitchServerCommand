// -----------------------------------------------------------------------
// <copyright file="Plugin.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SwitchServerCommand
{
    using System;
    using CommandSystem;
    using Exiled.API.Features;
    using RemoteAdmin;
    using SwitchServerCommand.Commands;

    /// <inheritdoc />
    public class Plugin : Plugin<Config>
    {
        /// <inheritdoc/>
        public override string Author => "Build";

        /// <inheritdoc/>
        public override string Name => "SwitchServerCommand";

        /// <inheritdoc/>
        public override string Prefix => "SwitchServerCommand";

        /// <inheritdoc/>
        public override Version Version { get; } = new(1, 0, 0);

        /// <inheritdoc/>
        public override Version RequiredExiledVersion { get; } = new(5, 3, 0);

        /// <inheritdoc />
        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Server.ReloadedConfigs += OnReloadedConfigs;
            base.OnEnabled();
        }

        /// <inheritdoc />
        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Server.ReloadedConfigs -= OnReloadedConfigs;
            base.OnDisabled();
        }

        /// <inheritdoc />
        public override void OnRegisteringCommands()
        {
            QueryProcessor.DotCommandHandler.RegisterCommand(Config.ServerCommand);
            Commands[typeof(ClientCommandHandler)][typeof(ServerCommand)] = Config.ServerCommand;
            base.OnRegisteringCommands();
        }

        private void OnReloadedConfigs()
        {
            OnUnregisteringCommands();
            OnRegisteringCommands();
        }
    }
}
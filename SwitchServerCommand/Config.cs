// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SwitchServerCommand
{
    using Exiled.API.Interfaces;
    using SwitchServerCommand.Commands;

    /// <inheritdoc />
    public class Config : IConfig
    {
        /// <inheritdoc/>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a configurable instance of the <see cref="Commands.ServerCommand"/> class.
        /// </summary>
        public ServerCommand ServerCommand { get; set; } = new();
    }
}
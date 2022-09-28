// -----------------------------------------------------------------------
// <copyright file="ServerCommand.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SwitchServerCommand.Commands
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using NorthwoodLib.Pools;
    using RoundRestarting;

    /// <inheritdoc />
    public class ServerCommand : ICommand
    {
        /// <inheritdoc />
        public string Command { get; set; } = "server";

        /// <inheritdoc />
        public string[] Aliases { get; set; } = Array.Empty<string>();

        /// <inheritdoc />
        public string Description { get; set; } = "Switches your current server.";

        /// <summary>
        /// Gets or sets the permission required to use this command.
        /// </summary>
        /// <remarks>If <see langword="null"/> or <see cref="string.Empty"/> then no permission will be required.</remarks>
        public string RequiredPermission { get; set; } = "sc.switch";

        /// <summary>
        /// Gets or sets the response to provide to the user when they lack the <see cref="RequiredPermission"/>.
        /// </summary>
        [Description("The response to provide to the user when they lack the required permission.")]
        public string InsufficientPermissionResponse { get; set; } = "You do not have permission to use this command";

        /// <summary>
        /// Gets or sets the identifiers paired with the port of the server they represent.
        /// </summary>
        [Description("The identifiers paired with the port of the server they represent.")]
        public Dictionary<string, ushort> ServerIdentifiers { get; set; } = new()
        {
            { "1", 7777 },
            { "2", 7778 },
        };

        /// <summary>
        /// Gets or sets the format used to separate the server names from the <see cref="ServerIdentifiers"/> config.
        /// </summary>
        [Description("The format used to separate the server names from the server identifiers config. Placeholders: {0}=Server, {1}=Server Index, {2}=Server Port")]
        public string IdentifierFormat { get; set; } = "\n- {0}";

        /// <summary>
        /// Gets or sets the response to provide to the user when they fail to provide a server to switch to.
        /// </summary>
        [Description("The response to provide to the user when they fail to provide a server to switch to. Placeholders: {0}=Server List")]
        public string SpecifyServerResponse { get; set; } = "You must specify a server to switch to. Available:{0}";

        /// <summary>
        /// Gets or sets the response to provide to the user when the server they provide is not specified in the <see cref="ServerIdentifiers"/> config.
        /// </summary>
        [Description("The response to provide to the user when the server they provide is not specified in the server identifiers config. Placeholders: {0}=Server List")]
        public string InvalidServerResponse { get; set; } = "Invalid server. Available:{0}";

        /// <summary>
        /// Gets or sets the response to provide to the user when the server they specify is the server they are currently connected to.
        /// </summary>
        [Description("The response to provide to the user when the server they specify is the server they are currently connected to.")]
        public string AlreadyConnectedResponse { get; set; } = "You are already connected to this server.";

        /// <inheritdoc />
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);
            if (player is null || player == Server.Host)
            {
                response = "This command can only be used by players.";
                return false;
            }

            if (!string.IsNullOrEmpty(RequiredPermission) && !player.CheckPermission(RequiredPermission))
            {
                response = InsufficientPermissionResponse;
                return false;
            }

            string formattedServerList = GenerateFormattedServerList();
            if (arguments.Count == 0)
            {
                response = string.Format(SpecifyServerResponse, formattedServerList);
                return false;
            }

            string argument = string.Join(" ", arguments);
            if (!ServerIdentifiers.TryGetValue(argument, out ushort port))
            {
                response = string.Format(InvalidServerResponse, formattedServerList);
                return false;
            }

            if (Server.Port == port)
            {
                response = AlreadyConnectedResponse;
                return false;
            }

            player.Connection.Send(new RoundRestartMessage(RoundRestartType.RedirectRestart, 0.1f, port, true, false));
            response = string.Empty;
            return true;
        }

        private string GenerateFormattedServerList()
        {
            StringBuilder responseBuilder = StringBuilderPool.Shared.Rent();

            int i = 1;
            foreach (KeyValuePair<string, ushort> pair in ServerIdentifiers)
            {
                responseBuilder.Append(string.Format(IdentifierFormat, pair.Key, i, pair.Value));
                i++;
            }

            return StringBuilderPool.Shared.ToStringReturn(responseBuilder);
        }
    }
}
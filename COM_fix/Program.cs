/*
 * COM_fix - Universal COM Port Rescue Tool
 * Copyright (c) 2026 d7main
 *
 * SPDX-License-Identifier: MIT
 *
 * This file is part of COM_fix.
 * Program.cs: Application entry point with single-instance enforcement.
 */

namespace COM_fix
{
    /// <summary>
    /// Application entry point. Enforces single-instance execution using a named mutex
    /// and provides a helper to check for Administrator privileges.
    /// </summary>
    internal static class Program
    {
        /// <summary>Named mutex to prevent multiple instances of the application.</summary>
        private static readonly System.Threading.Mutex _mutex =
            new System.Threading.Mutex(true, "{8F6F0AC4-B9A1-45fd-A8CF-72F04E6BDE8F}");

        /// <summary>
        /// The main entry point for the application.
        /// Acquires a named mutex to enforce single-instance execution,
        /// initializes the application configuration, and starts the main form.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!_mutex.WaitOne(TimeSpan.Zero, true))
            {
                if (!_mutex.WaitOne(3000, true)) return;
            }

            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }

        /// <summary>
        /// Checks whether the current process is running with Administrator privileges.
        /// </summary>
        /// <returns><c>true</c> if the current user has the Administrator role; otherwise <c>false</c>.</returns>
        public static bool IsAdministrator()
        {
            using (var identity = System.Security.Principal.WindowsIdentity.GetCurrent())
            {
                var principal = new System.Security.Principal.WindowsPrincipal(identity);
                return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
            }
        }
    }
}
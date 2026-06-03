/*
 * COM_fix - Universal COM Port Rescue Tool
 * Copyright (c) 2026 d7main
 *
 * SPDX-License-Identifier: MIT
 *
 * This file is part of COM_fix.
 * DriverService.cs: System-level driver manipulation using pnputil and registry commands.
 */

using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace COM_fix.SystemServices
{
    /// <summary>
    /// Provides asynchronous methods for driver-level device operations,
    /// including forced device restarts and COM port database resets.
    /// Requires elevated (Administrator) privileges for most operations.
    /// </summary>
    public class DriverService : IDisposable
    {
        private bool _disposed;

        /// <summary>
        /// Forces a restart of the specified PnP device using <c>pnputil /restart-device</c>.
        /// This triggers a full driver stack re-enumeration, which can resolve COM port conflicts.
        /// </summary>
        /// <param name="instanceId">The PNPDeviceID instance path (e.g., "USB\VID_0483&amp;PID_5740\...").</param>
        /// <returns>A status message indicating success or the nature of the failure.</returns>
        public async Task<string> ForceRestartDeviceAsync(string instanceId)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (string.IsNullOrEmpty(instanceId) || instanceId == "UNKNOWN")
                        return "Error: Invalid device instance ID.";

                    ProcessStartInfo psiRestart = new ProcessStartInfo
                    {
                        FileName = "pnputil",
                        Arguments = $"/restart-device \"{instanceId}\"",
                        Verb = "runas",
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true
                    };
                    Process.Start(psiRestart)?.WaitForExit();
                    return "Success: Device restarted successfully via pnputil.";
                }
                catch (UnauthorizedAccessException)
                {
                    return "Error: Access Denied. Elevated privileges required.";
                }
                catch (Exception ex)
                {
                    return $"Error: {ex.Message}";
                }
            });
        }

        /// <summary>
        /// Resets the Windows COM Name Arbiter database by deleting the ComDB registry value.
        /// This clears all cached COM port assignments, forcing Windows to re-allocate ports.
        /// A system reboot may be required for changes to take full effect.
        /// </summary>
        /// <returns>A status message indicating success or the nature of the failure.</returns>
        public async Task<string> ResetComInventoryAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = "/c reg delete \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\COM Name Arbiter\" /v ComDB /f",
                        Verb = "runas",
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true
                    };
                    Process.Start(psi)?.WaitForExit();
                    return "Success: COM map successfully purged via registry key reset.";
                }
                catch (UnauthorizedAccessException)
                {
                    return "Error: Access Denied. Elevated privileges required.";
                }
                catch (Exception ex)
                {
                    return $"Error: {ex.Message}";
                }
            });
        }

        /// <summary>
        /// Releases all resources used by the <see cref="DriverService"/>.
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                GC.SuppressFinalize(this);
            }
        }
    }
}

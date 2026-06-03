/*
 * COM_fix - Universal COM Port Rescue Tool
 * Copyright (c) 2026 d7main
 *
 * SPDX-License-Identifier: MIT
 *
 * This file is part of COM_fix.
 * ProcessService.cs: Identifies and terminates processes that lock COM ports.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace COM_fix.SystemServices
{
    /// <summary>
    /// Provides methods to detect and terminate processes known to aggressively
    /// lock COM ports, preventing other applications from accessing serial devices.
    /// Targets flight controller configurators, 3D printer slicers, terminal emulators,
    /// IDEs, and Arduino tools.
    /// </summary>
    public class ProcessService
    {
        /// <summary>
        /// List of process names known to hold COM port handles open.
        /// </summary>
        private static readonly string[] TargetProcesses =
        {
            "betaflight-configurator", "cleanflight-configurator", "groundcontrol", "missionplanner",
            "cura", "crealityprint", "bambulab", "prusaslicer",
            "putty", "teraterm", "minicom",
            "vscodium", "vscode", "code",
            "arduino", "arduino-ide"
        };

        /// <summary>
        /// Asynchronously scans for and terminates all running instances of known
        /// COM port-locking processes.
        /// </summary>
        /// <returns>A list of process names that were successfully terminated.</returns>
        public async Task<List<string>> KillConflictProcessesAsync()
        {
            return await Task.Run(() =>
            {
                List<string> closedProcesses = new List<string>();

                foreach (string pName in TargetProcesses)
                {
                    try
                    {
                        Process[] processes = Process.GetProcessesByName(pName);
                        foreach (Process proc in processes)
                        {
                            proc.Kill();
                            closedProcesses.Add(proc.ProcessName);
                        }
                    }
                    catch
                    {
                        // Ignore access denied exceptions for system-protected processes
                    }
                }
                return closedProcesses;
            });
        }
    }
}

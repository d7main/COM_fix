/*
 * COM_fix - Universal COM Port Rescue Tool
 * Copyright (c) 2026 d7main
 *
 * SPDX-License-Identifier: MIT
 *
 * This file is part of COM_fix.
 * SerialDeviceInfo.cs: Data model representing a discovered serial device.
 */

namespace COM_fix.Models
{
    /// <summary>
    /// Represents a discovered serial device with its port name, description,
    /// hardware identifier, and availability status.
    /// </summary>
    public class SerialDeviceInfo
    {
        /// <summary>The system COM port name (e.g., "COM3") or "DFU" for bootloader devices.</summary>
        public string PortName { get; set; }

        /// <summary>Human-readable device description from WMI (e.g., "STM32 Virtual ComPort").</summary>
        public string Description { get; set; }

        /// <summary>PnP Hardware ID from WMI (e.g., "USB\VID_0483&amp;PID_5740\...").</summary>
        public string HardwareID { get; set; }

        /// <summary>Indicates whether the port is currently locked by another process.</summary>
        public bool IsBusy { get; set; }

        /// <summary>
        /// Returns a formatted string combining port name and description.
        /// </summary>
        /// <returns>A string in the format "COMx - Description".</returns>
        public override string ToString()
        {
            return $"{PortName} - {Description}";
        }
    }
}

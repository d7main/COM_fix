/*
 * COM_fix - Universal COM Port Rescue Tool
 * Copyright (c) 2026 d7main
 *
 * SPDX-License-Identifier: MIT
 *
 * This file is part of COM_fix.
 * DeviceScanner.cs: WMI-based hardware discovery for COM ports and USB devices.
 */

using System;
using System.Collections.Generic;
using System.Management;
using COM_fix.Models;

namespace COM_fix.SystemServices
{
    /// <summary>
    /// Scans the system for serial COM ports and USB devices using WMI.
    /// Implements a multi-layer detection strategy: WMI PnP enumeration,
    /// USB-specific class GUIDs, and <c>SerialPort.GetPortNames()</c> fallback.
    /// </summary>
    public class DeviceScanner
    {
        /// <summary>
        /// Performs a comprehensive hardware scan for all COM port devices.
        /// Uses a triple-layer detection strategy to maximize device discovery.
        /// </summary>
        /// <returns>A list of <see cref="SerialDeviceInfo"/> representing all discovered serial devices.</returns>
        public List<SerialDeviceInfo> ScanPorts()
        {
            var devices = new List<SerialDeviceInfo>();
            var seenPorts = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            try
            {
                string query = "SELECT Caption, PNPDeviceID, Description FROM Win32_PnPEntity " +
                               "WHERE PNPClass='Ports' OR PNPClass='USBDevice' " +
                               "OR ClassGuid='{36FC9E60-C465-11CF-8056-444553540000}' " +
                               "OR ClassGuid='{4d36e978-e325-11ce-bfc1-08002be10318}' OR Service='usbser'";

                using (var searcher = new ManagementObjectSearcher(query))
                {
                    foreach (ManagementBaseObject obj in searcher.Get())
                    {
                        ProcessWmiDevice(obj, devices, seenPorts);
                    }
                }

                // Fallback: pick up any ports missed by WMI
                string[] fallbackPorts = System.IO.Ports.SerialPort.GetPortNames();
                foreach (string port in fallbackPorts)
                {
                    if (!seenPorts.Contains(port))
                    {
                        devices.Add(new SerialDeviceInfo
                        {
                            PortName = port,
                            Description = "Generic Fallback Serial Port",
                            HardwareID = "UNKNOWN",
                            IsBusy = CheckIfPortIsBusy(port)
                        });
                        seenPorts.Add(port);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DeviceScanner] WMI Error: {ex.Message}");
            }

            return devices;
        }

        /// <summary>
        /// Processes a single WMI device object, extracting COM port information
        /// and adding it to the device list if it hasn't been seen before.
        /// </summary>
        /// <param name="obj">The WMI management object to process.</param>
        /// <param name="devices">The accumulating list of discovered devices.</param>
        /// <param name="seenPorts">Set of already-seen port names to prevent duplicates.</param>

        /**
         * The method attempts to extract the COM port name from the device's caption using a regex pattern.
         * If a COM port is found and hasn't been seen before, it adds a new SerialDeviceInfo to the list.
         * It also handles special cases like STM32 DFU bootloader devices that may not have a COM port assigned.
         */

        /// <summary> OLD VERSION: The method attempts to extract the COM port name from the device's caption using a regex pattern.
        /*
        private void ProcessWmiDevice(ManagementBaseObject obj, List<SerialDeviceInfo> devices, HashSet<string> seenPorts)
        {
            string caption = obj["Caption"]?.ToString() ?? "";
            string deviceId = obj["PNPDeviceID"]?.ToString() ?? "";
            string description = obj["Description"]?.ToString() ?? "";

            if (string.IsNullOrEmpty(caption) && string.IsNullOrEmpty(description)) return;

            string hwIdUpper = deviceId.ToUpper();

            // Attempt to extract COMx port name from the caption (e.g., "USB Serial Device (COM3)")
            System.Text.RegularExpressions.Match match =
                System.Text.RegularExpressions.Regex.Match(caption, @"\((COM\d+)\)");

            if (match.Success)
            {
                string portName = match.Groups[1].Value.ToUpper();
                if (!seenPorts.Contains(portName))
                {
                    devices.Add(new SerialDeviceInfo
                    {
                        PortName = portName,
                        Description = description == "" ? "Generic Device" : description,
                        HardwareID = deviceId,
                        IsBusy = CheckIfPortIsBusy(portName)
                    });
                    seenPorts.Add(portName);
                }
            }
            // Catch STM32 DFU bootloader devices (no COM port assigned)
            else if (hwIdUpper.Contains(HardwareConstants.VID_STM32_DFU))
            {
                if (!seenPorts.Contains(hwIdUpper))
                {
                    devices.Add(new SerialDeviceInfo
                    {
                        PortName = "DFU",
                        Description = description == "" ? "STM32 BOOTLOADER" : description,
                        HardwareID = deviceId,
                        IsBusy = false
                    });
                    seenPorts.Add(hwIdUpper);
                }
            }
        }
        */

        private void ProcessWmiDevice(ManagementBaseObject obj, List<SerialDeviceInfo> devices, HashSet<string> seenPorts)
        {
            string caption = obj["Caption"]?.ToString() ?? "";
            string deviceId = obj["PNPDeviceID"]?.ToString() ?? "";
            string description = obj["Description"]?.ToString() ?? "";

            if (string.IsNullOrEmpty(caption) && string.IsNullOrEmpty(description)) return;

            string hwIdUpper = deviceId.ToUpper();

            // Identify the chipset using centralized hardware constants
            string chipType = HardwareConstants.IdentifyChip(deviceId);

            // Attempt to extract COMx port name from the caption (e.g., "USB Serial Device (COM3)")
            System.Text.RegularExpressions.Match match =
                System.Text.RegularExpressions.Regex.Match(caption, @"\((COM\d+)\)");

            if (match.Success)
            {
                string portName = match.Groups[1].Value.ToUpper();
                if (!seenPorts.Contains(portName))
                {
                    devices.Add(new SerialDeviceInfo
                    {
                        PortName = portName,
                        Description = string.IsNullOrEmpty(description) ? chipType : description,
                        HardwareID = deviceId,
                        IsBusy = CheckIfPortIsBusy(portName)
                    });
                    seenPorts.Add(portName);

                    // Prevent redundant multi-interface enumeration for the same Espressif composite device
                    if (hwIdUpper.Contains(HardwareConstants.VID_ESP_PREFIX))
                    {
                        seenPorts.Add("ESP_ALREADY_FOUND_AS_COM");
                    }
                }
            }
            // Catch STM32 DFU bootloader mode (no native virtual COM port exposed)
            else if (hwIdUpper.Contains(HardwareConstants.VID_STM32_DFU))
            {
                if (!seenPorts.Contains("STM32_DFU"))
                {
                    devices.Add(new SerialDeviceInfo
                    {
                        PortName = "DFU",
                        Description = "STM32 BOOTLOADER",
                        HardwareID = deviceId,
                        IsBusy = false
                    });
                    seenPorts.Add("STM32_DFU");
                }
            }
            // Catch Espressif devices without an assigned COM port (e.g., raw JTAG interface or missing drivers)
            else if (hwIdUpper.Contains(HardwareConstants.VID_ESP_PREFIX))
            {
                // Skip parent composite device nodes, only capture unmapped fallback endpoints
                if (!seenPorts.Contains("ESP_ALREADY_FOUND_AS_COM") &&
                    !seenPorts.Contains("ESP_BOOT") &&
                    !caption.Contains("Složené") &&
                    !caption.Contains("Composite"))
                {
                    devices.Add(new SerialDeviceInfo
                    {
                        PortName = "ESP_BOOT",
                        Description = "ESP32 Native USB (No COM Port / Driver Issue)",
                        HardwareID = deviceId,
                        IsBusy = false
                    });
                    seenPorts.Add("ESP_BOOT");
                }
            }
        }

        /// <summary>
        /// Checks whether a serial port is currently locked by another process
        /// by attempting a brief open/close cycle.
        /// </summary>
        /// <param name="portName">The port name to test (e.g., "COM3").</param>
        /// <returns><c>true</c> if the port is busy or inaccessible; otherwise <c>false</c>.</returns>
        private bool CheckIfPortIsBusy(string portName)
        {
            if (portName == "DFU" || portName == "ESP_BOOT") return false;

            try
            {
                using (var tempPort = new System.IO.Ports.SerialPort(portName))
                {
                    tempPort.Open();
                    tempPort.Close();
                    return false;
                }
            }
            catch
            {
                return true;
            }
        }
    }
}

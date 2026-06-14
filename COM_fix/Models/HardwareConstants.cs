/*
 * COM_fix - Universal COM Port Rescue Tool
 * Copyright (c) 2026 d7main
 *
 * SPDX-License-Identifier: MIT
 *
 * This file is part of COM_fix.
 * HardwareConstants.cs: Centralized hardware identifiers and chip identification logic.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace COM_fix
{
    /// <summary>
    /// Data model representing a hardware device definition for JSON serialization.
    /// Kept within this file to maintain a flat architecture without forcing extra folders.
    /// </summary>
    public class HardwareDefinition
    {
        public string HardwareIDIdentifier { get; set; }
        public string ChipFamily { get; set; }
    }

    /// <summary>
    /// Provides centralized constants for USB Vendor/Product IDs and chip identification.
    /// Supports dynamic expansion via an external 'devices.json' file with a hardcoded safety backup.
    /// </summary>
    public static class HardwareConstants
    {
        // ── STM32 Family ────────────────────────────────────────────────
        /// <summary>STM32 Device Firmware Upgrade (DFU) bootloader mode.</summary>
        public const string VID_STM32_DFU = "VID_0483&PID_DF11";

        /// <summary>STM32 Virtual COM Port (VCP) — standard flight controller mode.</summary>
        public const string VID_STM32_VCP = "VID_0483&PID_5740";

        /// <summary>STMicroelectronics vendor prefix.</summary>
        public const string VID_STM32_PREFIX = "VID_0483";

        // ── Espressif Family ────────────────────────────────────────────
        /// <summary>Silicon Labs CP2102 — common ESP32 / NodeMCU USB-UART bridge.</summary>
        public const string VID_ESP32_CP2102 = "VID_10C4&PID_EA60";

        /// <summary>Espressif ESP32-C3 Super Mini native USB CDC/JTAG stub.</summary>
        public const string VID_ESP32C3_SUPER_MINI = "VID_303A&PID_1001";

        /// <summary>Espressif Systems vendor prefix (native USB on ESP32-S2/S3/C3/C6 series).</summary>
        public const string VID_ESP_PREFIX = "VID_303A";

        // ── Common USB-to-Serial Chips ──────────────────────────────────
        /// <summary>WCH CH340 — ubiquitous on Arduino clones and ESP8266 boards.</summary>
        public const string VID_CH340 = "VID_1A86&PID_7523";

        /// <summary>FTDI FT232R — professional-grade USB-UART converter.</summary>
        public const string VID_FTDI = "VID_0403&PID_6001";

        /// <summary>Arduino official boards (Uno, Mega, etc.).</summary>
        public const string VID_ARDUINO = "VID_2341";

        /// <summary>Raspberry Pi Pico (RP2040 native USB).</summary>
        public const string VID_RPI_PICO = "VID_2E8A&PID_0005";

        // ── Open-Source Dynamic Engine ──────────────────────────────────
        private static List<HardwareDefinition> _dynamicDefinitions = new List<HardwareDefinition>();
        private static readonly string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "devices.json");

        /// <summary>
        /// Shared static constructor. Automatically triggers hardware database initialization on first reference.
        /// </summary>
        static HardwareConstants()
        {
            LoadDefinitions();
        }

        /// <summary>
        /// Reads the external 'devices.json' configuration file. If the file is missing or corrupted,
        /// it populates the engine with standard hardcoded defaults and automatically regenerates the file.
        /// </summary>
        public static void LoadDefinitions()
        {
            try
            {
                if (File.Exists(ConfigPath))
                {
                    string jsonContent = File.ReadAllText(ConfigPath);
                    _dynamicDefinitions = JsonSerializer.Deserialize<List<HardwareDefinition>>(jsonContent)
                                           ?? new List<HardwareDefinition>();
                }
                else
                {
                    // Configuration file missing: fall back to default database and deploy the file
                    PopulateDefaults();
                    SaveDefinitions();
                }
            }
            catch
            {
                // Fault-protection: if a user breaks the JSON syntax manually, fall back to embedded defaults
                PopulateDefaults();
            }
        }

        /// <summary>
        /// Serializes and saves the current hardware database back to the external JSON configuration file.
        /// </summary>
        public static void SaveDefinitions()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(_dynamicDefinitions, options);
                File.WriteAllText(ConfigPath, json);
            }
            catch { /* Fail-silent: Ignore filesystem access errors (e.g., running without write permissions) */ }
        }

        /// <summary>
        /// Identifies the chipset/manufacturer of a USB device based on its PnP Device ID.
        /// Returns a human-readable description of the chip for enhanced logging.
        /// </summary>
        /// <param name="pnpId">The PNPDeviceID string from WMI (e.g., "USB\VID_0483&amp;PID_5740\...").</param>
        /// <returns>A human-readable chip identification string.</returns>
        public static string IdentifyChip(string pnpId)
        {
            if (string.IsNullOrEmpty(pnpId)) return "Unknown Device";

            string id = pnpId.ToUpper();

            // 1. Dynamic Match Pool (Scan against extensible database loaded from the JSON config)
            foreach (var definition in _dynamicDefinitions)
            {
                if (id.Contains(definition.HardwareIDIdentifier.ToUpper()))
                {
                    return definition.ChipFamily;
                }
            }

            // 2. Fallback Vendor Prefix Matching (Broad vendor sweeps if no specific definition matches)
            if (id.Contains(VID_STM32_PREFIX)) return "Generic STM32/ARM Device";
            if (id.Contains(VID_ESP_PREFIX)) return "Espressif Native USB Device";

            return "Generic USB/Serial Device";
        }

        /// <summary>
        /// Populates the internal list with standard hardware defaults when no external configuration is present.
        /// </summary>
        private static void PopulateDefaults()
        {
            _dynamicDefinitions = new List<HardwareDefinition>
            {
                new HardwareDefinition { HardwareIDIdentifier = VID_CH340, ChipFamily = "CH340 (Arduino Clone / ESP8266)" },
                new HardwareDefinition { HardwareIDIdentifier = VID_ESP32_CP2102, ChipFamily = "CP2102 (ESP32 / NodeMCU)" },
                new HardwareDefinition { HardwareIDIdentifier = VID_FTDI, ChipFamily = "FTDI FT232R" },
                new HardwareDefinition { HardwareIDIdentifier = VID_ARDUINO, ChipFamily = "Original Arduino (Uno/Mega)" },
                new HardwareDefinition { HardwareIDIdentifier = VID_STM32_VCP, ChipFamily = "STM32 VCP (Flight Controller)" },
                new HardwareDefinition { HardwareIDIdentifier = VID_STM32_DFU, ChipFamily = "STM32 DFU Bootloader" },
                new HardwareDefinition { HardwareIDIdentifier = VID_RPI_PICO, ChipFamily = "Raspberry Pi Pico (RP2040)" },
                new HardwareDefinition { HardwareIDIdentifier = VID_ESP32C3_SUPER_MINI, ChipFamily = "ESP32-C3 Super Mini" }
            };
        }
    }
}
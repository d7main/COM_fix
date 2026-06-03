/*
 * COM_fix - Universal COM Port Rescue Tool
 * Copyright (c) 2026 d7main
 *
 * SPDX-License-Identifier: MIT
 *
 * This file is part of COM_fix.
 * HardwareConstants.cs: Centralized hardware identifiers and chip identification logic.
 */

namespace COM_fix.Models
{
    /// <summary>
    /// Provides centralized constants for USB Vendor/Product IDs and chip identification.
    /// All hardcoded VID/PID strings used throughout the application are defined here
    /// to ensure consistency and easy maintenance.
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

        // ── ESP32 / Espressif Family ────────────────────────────────────
        /// <summary>Silicon Labs CP2102 — common ESP32 / NodeMCU USB-UART bridge.</summary>
        public const string VID_ESP32_CP2102 = "VID_10C4&PID_EA60";

        /// <summary>Espressif vendor prefix (native USB on ESP32-S2/S3).</summary>
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

            if (id.Contains(VID_CH340))       return "CH340 (Arduino Clone / ESP8266)";
            if (id.Contains(VID_ESP32_CP2102)) return "CP2102 (ESP32 / NodeMCU)";
            if (id.Contains(VID_FTDI))        return "FTDI FT232R";
            if (id.Contains(VID_ARDUINO))     return "Original Arduino (Uno/Mega)";
            if (id.Contains(VID_STM32_VCP))   return "STM32 VCP (Flight Controller)";
            if (id.Contains(VID_STM32_DFU))   return "STM32 DFU Bootloader";
            if (id.Contains(VID_RPI_PICO))    return "Raspberry Pi Pico (RP2040)";
            if (id.Contains(VID_ESP_PREFIX))   return "Espressif Native USB (ESP32-S2/S3)";

            return "Generic USB/Serial Device";
        }
    }
}

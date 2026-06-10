/*
 * COM_fix - Universal COM Port Rescue Tool
 * Copyright (c) 2026 d7main
 *
 * SPDX-License-Identifier: MIT
 *
 * This file is part of COM_fix.
 * Form1.cs: Main application interface — orchestrates device scanning,
 * driver repair, process management, and serial communication.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using COM_fix.Models;
using COM_fix.SystemServices;

namespace COM_fix
{
    /// <summary>
    /// Main application form for the COM_fix Universal Rescue Tool.
    /// Provides UI controls for hardware scanning, driver repair, process termination,
    /// COM database reset, and serial terminal connectivity.
    /// All hardware interactions are delegated to service classes in <see cref="COM_fix.SystemServices"/>.
    /// </summary>
    public partial class Form1 : Form
    {
        private readonly DeviceScanner _deviceScanner;
        private readonly ProcessService _processService;
        private readonly DriverService _driverService;
        private readonly SerialService _serialService;

        private List<SerialDeviceInfo> _lastDiscoveredDevices = new List<SerialDeviceInfo>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Form1"/> class.
        /// Creates all service instances and wires up event handlers.
        /// </summary>
        public Form1()
        {
            InitializeComponent();

            // ── Initialize Services ─────────────────────────────────────
            _deviceScanner = new DeviceScanner();
            _processService = new ProcessService();
            _driverService = new DriverService();
            _serialService = new SerialService();

            // Wire serial service events to the log
            _serialService.OnDataReceived += (s, data) => AddToLog($"> {data}");
            _serialService.OnError += (s, error) => AddToLog($"[WARN] {error}");

            // ── UI Setup ────────────────────────────────────────────────
            SetStatus("System Ready", false);

            listLog.BackColor = Color.White;
            listLog.ForeColor = Color.Black;
            listLog.Font = new Font("Consolas", 9f);

            this.Load += Form1_Load;
            RefreshPortList();
        }

        /// <summary>
        /// Handles the form load event. Logs the application startup message.
        /// </summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            AddToLog("[INFO] COM_fix Universal Rescue Tool v1.0 — Clean Architecture Deployed.");
        }

        #region Event Handlers

        /// <summary>
        /// Scans for all connected COM port devices using WMI and fallback detection.
        /// Updates the port combo box and highlights STM32 DFU targets.
        /// </summary>
        private async void btnScan_Click(object sender, EventArgs e)
        {
            SetControlState(false);
            SetStatus("Scanning for COM devices...", true);
            listLog.Clear();
            AddToLog("[INFO] Starting hardware discovery...");

            try
            {
                var foundDevices = await Task.Run(() => _deviceScanner.ScanPorts());
                _lastDiscoveredDevices = foundDevices;

                if (foundDevices.Count > 0)
                {
                    foreach (var device in foundDevices)
                    {
                        string busyStatus = device.IsBusy ? "[WARN] BUSY" : "[SUCCESS] FREE";
                        string chipInfo = HardwareConstants.IdentifyChip(device.HardwareID);
                        AddToLog($"{busyStatus}: {device.Description} ({device.PortName}) [{chipInfo}]");

                        if (device.HardwareID.Contains(HardwareConstants.VID_STM32_PREFIX))
                            AddToLog("   --> [INFO] STM32/ARM Device detected");
                    }
                    SetStatus($"Status: Found {foundDevices.Count} device(s).", false);
                }
                else
                {
                    AddToLog("[WARN] No COM devices found. Check physical connections.");
                    SetStatus("Scan complete. Nothing found.", false);
                }

                UpdatePortComboBox(foundDevices);
            }
            catch (Exception ex)
            {
                AddToLog($"[CRITICAL] Discovery failed: {ex.Message}");
                SetStatus("Scan failed.", false);
            }
            finally
            {
                System.Media.SystemSounds.Beep.Play();
                SetControlState(true);
            }
        }

        /// <summary>
        /// Attempts to repair the selected device by forcing a driver stack restart via pnputil.
        /// Requires a valid Hardware ID from a previous scan result.
        /// </summary>
        private async void btnFix_Click(object sender, EventArgs e)
        {
            SetControlState(false);
            System.Media.SystemSounds.Exclamation.Play();
            SetStatus("Repairing system...", true);
            AddToLog("[INFO] === STARTING DEEP SYSTEM REPAIR ===");

            try
            {
                string selectedPort = cmbPorts.SelectedItem?.ToString();
                var device = _lastDiscoveredDevices.Find(d => d.PortName == selectedPort);

                if (device != null && !string.IsNullOrEmpty(device.HardwareID) && device.HardwareID != "UNKNOWN")
                {
                    AddToLog($"[DEBUG] Targeting instance: {device.HardwareID}");

                    string result = await _driverService.ForceRestartDeviceAsync(device.HardwareID);

                    if (result.StartsWith("Success"))
                    {
                        AddToLog("[SUCCESS] Driver re-enumerated via restart. Target application should see the port now!");
                        SetStatus("Success: Device Ready!", false);
                        System.Media.SystemSounds.Asterisk.Play();
                    }
                    else
                    {
                        AddToLog($"[CRITICAL] Repair failed: {result}");
                        SetStatus("Repair failed.", false);
                    }
                }
                else
                {
                    AddToLog("[WARN] Cannot run repair: Device HardwareID is unknown. Please rescan.");
                    SetStatus("Repair failed.", false);
                }
            }
            catch (Exception ex)
            {
                AddToLog($"[CRITICAL] Repair sequence aborted: {ex.Message}");
                SetStatus("Repair failed.", false);
            }
            finally
            {
                SetControlState(true);
            }
        }

        /// <summary>
        /// Connects to or disconnects from the selected serial port.
        /// Uses <see cref="SerialService"/> for managed connection lifecycle.
        /// </summary>
        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (_serialService.IsOpen)
            {
                _serialService.Disconnect();
                btnConnect.Text = "Connect";
                SetStatus("Disconnected", false);
                AddToLog("[INFO] --- PORT CLOSED ---");
                return;
            }

            if (cmbPorts.SelectedItem == null || string.IsNullOrWhiteSpace(cmbBaudRate.Text))
            {
                MessageBox.Show("Please select both a COM port and a Baud Rate.",
                    "Configuration Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedPort = cmbPorts.SelectedItem.ToString();
            if (!int.TryParse(cmbBaudRate.Text, out int baudRate))
            {
                AddToLog("[WARN] Invalid Baud Rate format.");
                return;
            }

            _serialService.Connect(selectedPort, baudRate);

            if (_serialService.IsOpen)
            {
                btnConnect.Text = "Disconnect";
                SetStatus($"Listening to {selectedPort} at {baudRate} baud...", true);
                AddToLog($"[SUCCESS] --- CONNECTED TO {selectedPort} ---");
            }
        }

        /// <summary>
        /// Terminates all known COM port-locking processes (Betaflight, Arduino, PuTTY, etc.).
        /// </summary>
        private async void btnForceKill_Click(object sender, EventArgs e)
        {
            SetControlState(false);
            SetStatus("Killing processes...", true);
            AddToLog("[INFO] Executing: KillConflictProcesses()");
            try
            {
                var killed = await _processService.KillConflictProcessesAsync();
                if (killed.Count > 0)
                    killed.ForEach(p => AddToLog($"[SUCCESS] Terminated: {p}"));
                else
                    AddToLog("[INFO] No conflicting processes detected.");
            }
            catch (Exception ex)
            {
                AddToLog($"[CRITICAL] {ex.Message}");
            }
            finally
            {
                SetStatus("Ready", false);
                SetControlState(true);
            }
        }

        /// <summary>
        /// Forces a restart of the selected USB device via pnputil.
        /// </summary>
        private async void btnRestartDevice_Click(object sender, EventArgs e)
        {
            SetControlState(false);
            SetStatus("Restarting device...", true);

            string selectedPort = cmbPorts.SelectedItem?.ToString();
            var device = _lastDiscoveredDevices.Find(d => d.PortName == selectedPort);

            if (device == null || string.IsNullOrEmpty(device.HardwareID) || device.HardwareID == "UNKNOWN")
            {
                AddToLog("[WARN] Cannot restart: Hardware ID unknown.");
                SetStatus("Ready", false);
                SetControlState(true);
                return;
            }

            AddToLog($"[INFO] Restarting device: {selectedPort}...");
            try
            {
                string result = await _driverService.ForceRestartDeviceAsync(device.HardwareID);
                AddToLog($"[INFO] {result}");
            }
            catch (Exception ex)
            {
                AddToLog($"[CRITICAL] Restart failed: {ex.Message}");
            }
            finally
            {
                SetStatus("Ready", false);
                SetControlState(true);
            }
        }

        /// <summary>
        /// Resets the Windows COM Name Arbiter database to clear stale port assignments.
        /// </summary>
        private async void btnResetCom_Click(object sender, EventArgs e)
        {
            SetControlState(false);
            SetStatus("Resetting COM Database...", true);
            AddToLog("[INFO] Cleaning COM Name Arbiter database...");
            try
            {
                string result = await _driverService.ResetComInventoryAsync();
                AddToLog($"[INFO] {result}");
                AddToLog("[INFO] COM database reset. Reboot might be required for changes to take effect.");
            }
            catch (Exception ex)
            {
                AddToLog($"[CRITICAL] Reset failed: {ex.Message}");
            }
            finally
            {
                SetStatus("Ready", false);
                SetControlState(true);
            }
        }

        /// <summary>
        /// Opens Windows Device Manager (devmgmt.msc) for manual inspection.
        /// </summary>
        private void btnOpenDevMgr_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo { FileName = "devmgmt.msc", UseShellExecute = true });
                AddToLog("[INFO] Device Manager launched.");
            }
            catch (Exception ex)
            {
                AddToLog($"[WARN] Could not open Device Manager: {ex.Message}");
            }
        }

        /// <summary>
        /// Restarts the application with elevated (Administrator) privileges.
        /// </summary>
        private void btnRunAsAdmin_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = Application.ExecutablePath,
                    UseShellExecute = true,
                    Verb = "runas"
                });
                Application.Exit();
            }
            catch (Exception ex)
            {
                AddToLog($"[WARN] Failed to elevate: {ex.Message}");
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Refreshes the port combo box with system-reported COM port names.
        /// Called once during initialization before a full scan is performed.
        /// </summary>
        private void RefreshPortList()
        {
            cmbPorts.Items.Clear();
            string[] availablePorts = System.IO.Ports.SerialPort.GetPortNames();
            if (availablePorts.Length > 0)
            {
                cmbPorts.Items.AddRange(availablePorts);
                cmbPorts.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Updates the status bar text and controls the progress bar visibility.
        /// </summary>
        /// <param name="message">The status message to display.</param>
        /// <param name="isWorking">If <c>true</c>, shows the marquee progress bar.</param>
        private void SetStatus(string message, bool isWorking)
        {
            toolStripStatusLabel1.Text = $"Status: {message}";
            toolStripProgressBar1.Visible = isWorking;
            statusStrip1.Refresh();
        }

        /// <summary>
        /// Globally enables or disables all action buttons during long-running async tasks.
        /// Prevents the user from triggering concurrent hardware operations.
        /// </summary>
        /// <param name="enabled">If <c>true</c>, enables all action buttons; otherwise disables them.</param>
        private void SetControlState(bool enabled)
        {
            btnScan.Enabled = enabled;
            btnFix.Enabled = enabled;
            btnForceKill.Enabled = enabled;
            btnRestartDevice.Enabled = enabled;
            btnCleanComMap.Enabled = enabled;
        }

        /// <summary>
        /// Appends a color-coded, timestamped message to the log RichTextBox.
        /// Thread-safe — automatically marshals to the UI thread via <see cref="Control.Invoke"/>.
        /// </summary>
        /// <remarks>
        /// Log prefix color mapping:
        /// <list type="bullet">
        ///   <item><c>[SUCCESS]</c> — DarkGreen</item>
        ///   <item><c>[INFO]</c> — RoyalBlue</item>
        ///   <item><c>[WARN]</c> — Orange</item>
        ///   <item><c>[CRITICAL]</c> — DarkRed</item>
        ///   <item><c>[DEBUG]</c> — Gray</item>
        /// </list>
        /// </remarks>
        /// <param name="message">The log message to append, including prefix tag.</param>
        private void AddToLog(string message)
        {
            if (listLog.InvokeRequired)
            {
                listLog.Invoke(new Action(() => AddToLog(message)));
                return;
            }

            string fullMessage = $"[{DateTime.Now:HH:mm:ss}] {message}\n";
            Color textColor = Color.Black;

            if (message.Contains("[SUCCESS]"))       textColor = Color.DarkGreen;
            else if (message.Contains("[INFO]"))     textColor = Color.RoyalBlue;
            else if (message.Contains("[WARN]"))     textColor = Color.Orange;
            else if (message.Contains("[CRITICAL]")) textColor = Color.DarkRed;
            else if (message.Contains("[DEBUG]"))    textColor = Color.Gray;

            listLog.SelectionStart = listLog.TextLength;
            listLog.SelectionLength = 0;
            listLog.SelectionColor = textColor;
            listLog.AppendText(fullMessage);
            listLog.SelectionColor = listLog.ForeColor;
            listLog.ScrollToCaret();
        }

        /// <summary>
        /// Updates the port ComboBox with scanned device results.
        /// Auto-selects STM32 DFU or ESP targets if detected.
        /// </summary>
        /// <param name="devices">The list of discovered serial devices.</param>
        /// <summary>
        /// Updates the port ComboBox with scanned device results.
        /// Auto-selects STM32 DFU or ESP targets if detected.
        /// </summary>
        /// <param name="devices">The list of discovered serial devices.</param>
        private void UpdatePortComboBox(List<SerialDeviceInfo> devices)
        {
            cmbPorts.Items.Clear();
            int targetIndex = -1;
            bool isStm32Dfu = false;
            bool isEspDevice = false;

            foreach (var device in devices)
            {
                int idx = cmbPorts.Items.Add(device.PortName);

                if (device.HardwareID.Contains(HardwareConstants.VID_STM32_DFU))
                {
                    targetIndex = idx;
                    isStm32Dfu = true;
                }
                else if (device.HardwareID.Contains(HardwareConstants.VID_ESP_PREFIX))
                {
                    targetIndex = idx;
                    isEspDevice = true;
                }
            }

            if (cmbPorts.Items.Count > 0) cmbPorts.SelectedIndex = targetIndex != -1 ? targetIndex : 0;

            if (isStm32Dfu)
            {
                btnFix.BackColor = Color.Orange;
                AddToLog("[WARN] STM32 DFU FOUND! Click FIX to repair drivers.");
            }
            else if (isEspDevice)
            {
                btnFix.BackColor = SystemColors.Control;
                AddToLog("[INFO] Espressif Native USB device selected and ready.");
            }
            else
            {
                btnFix.BackColor = SystemColors.Control;
                AddToLog("[INFO] Waiting for device...");
            }
        }

        /// <summary>
        /// Handles form closing. Disposes all active services and serial connections.
        /// </summary>
        /// <param name="e">Event arguments for the closing event.</param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _serialService?.Dispose();
            _driverService?.Dispose();
            base.OnFormClosing(e);
        }

        #endregion
    }
}

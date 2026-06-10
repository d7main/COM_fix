/*
 * COM_fix - Universal COM Port Rescue Tool
 * Copyright (c) 2026 d7main
 *
 * SPDX-License-Identifier: MIT
 *
 * This file is part of COM_fix.
 * SerialService.cs: Manages serial port connections and bidirectional data communication.
 */

using System;
using System.IO.Ports;

namespace COM_fix.SystemServices
{
    /// <summary>
    /// Provides an event-driven serial port communication service.
    /// Wraps <see cref="SerialPort"/> with proper lifecycle management,
    /// thread-safe data reception events, and <see cref="IDisposable"/> support.
    /// </summary>
    public class SerialService : IDisposable
    {
        private SerialPort _serialPort;
        private bool _disposed;

        /// <summary>Raised when data is received from the connected serial device and parsed as a string (Legacy support).</summary>
        public event EventHandler<string> OnDataReceived;

        /// <summary>
        /// Raised when raw binary data is received from the serial interface.
        /// Provides structural flexibility for custom telemetry parsers and HEX visualization without data corruption.
        /// </summary>
        public event EventHandler<byte[]> OnRawDataReceived;

        /// <summary>Raised when a connection or communication error occurs.</summary>
        public event EventHandler<string> OnError;

        /// <summary>Gets a value indicating whether the serial port is currently open.</summary>
        public bool IsOpen => _serialPort != null && _serialPort.IsOpen;

        /// <summary>
        /// Gets the name of the currently connected port, or <c>null</c> if disconnected.
        /// </summary>
        public string CurrentPort => _serialPort?.PortName;

        /// <summary>
        /// Opens a connection to the specified serial port at the given baud rate.
        /// Any previously open connection is closed before opening the new one.
        /// </summary>
        /// <param name="portName">The COM port name to connect to (e.g., "COM3").</param>
        /// <param name="baudRate">The baud rate for communication (e.g., 115200).</param>
        public void Connect(string portName, int baudRate)
        {
            try
            {
                if (IsOpen) Disconnect();

                _serialPort = new SerialPort(portName, baudRate);

                // Wire up the internal data receipt handler
                _serialPort.DataReceived += (sender, e) =>
                {
                    try
                    {
                        int bytesToRead = _serialPort.BytesToRead;
                        if (bytesToRead > 0)
                        {
                            byte[] buffer = new byte[bytesToRead];
                            int readBytes = _serialPort.Read(buffer, 0, bytesToRead);

                            if (readBytes > 0)
                            {
                                // Array truncation if readBytes differs from buffer capacity
                                if (readBytes < buffer.Length)
                                {
                                    Array.Resize(ref buffer, readBytes);
                                }

                                // 1. Trigger the new raw bytes pipeline
                                OnRawDataReceived?.Invoke(this, buffer);

                                // 2. Trigger the legacy string pipeline for backward compatibility
                                string textData = System.Text.Encoding.UTF8.GetString(buffer).TrimEnd();
                                if (!string.IsNullOrWhiteSpace(textData))
                                {
                                    OnDataReceived?.Invoke(this, textData);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        OnError?.Invoke(this, $"Read fault: {ex.Message}");
                    }
                };

                _serialPort.Open();
            }
            catch (UnauthorizedAccessException)
            {
                OnError?.Invoke(this, "Port is busy or locked by another program. Try running 'Fix' first.");
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, $"Connection failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Closes the current serial port connection and releases resources.
        /// Safe to call even if no connection is active.
        /// </summary>
        public void Disconnect()
        {
            if (_serialPort != null)
            {
                if (_serialPort.IsOpen)
                {
                    _serialPort.Close();
                }
                _serialPort.Dispose();
                _serialPort = null;
            }
        }

        /// <summary>
        /// Sends a text command to the connected serial device, appending CR+LF.
        /// </summary>
        /// <param name="command">The command string to send.</param>
        public void SendCommand(string command)
        {
            if (IsOpen)
            {
                _serialPort.Write(command + "\r\n");
            }
        }

        /// <summary>
        /// Releases all resources used by the <see cref="SerialService"/>,
        /// including closing the serial port if open.
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                Disconnect();
                _disposed = true;
                GC.SuppressFinalize(this);
            }
        }
    }
}
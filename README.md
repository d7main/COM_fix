# COM_fix Universal Rescue Tool (Beta Release)

Professional driver and port recovery utility designed for hardware developers, embedded systems engineers, and IoT developers working with STM32, ESP32, and other MCU platforms. 

This tool automates the process of diagnosing, unlocking, and repairing frozen or misbehaving virtual COM ports without requiring physical cable disconnection or system reboots.

> **⚠️ PRODUCTION DISCLAIMER (BETA RELEASE):** > This software is currently in its **Beta** development phase. It performs low-level operations within the Windows hardware configuration, process stack, and registry database. While designed with safety boundaries, unexpected behavior may occur depending on specific vendor driver implementations and system privileges. Use with caution.

<img width="1095" height="673" alt="Snímek obrazovky 2026-06-03 130033" src="https://github.com/user-attachments/assets/df2fd1e2-0850-429f-a9c3-7f1faec05910" />


## Architecture & Core Modules

The application completely separates UI synchronization from low-level infrastructure interaction. It isolates hardware queries into decoupled sub-services to guarantee thread safety and prevent UI blockages.

### 1. Hardware Discovery Engine (`DeviceScanner.cs`)
* **WMI Interrogation:** Utilizes Windows Management Instrumentation (WMI) via the `System.Management` namespace to query the `Win32_PnPEntity` class. This allows the tool to extract deep hardware properties instead of relying purely on surface-level registry strings.
* **Metadata Parsing:** Automatically extracts structural Hardware IDs (`HardwareID`), Vendor IDs (`VID`), Product IDs (`PID`), and operational Device Instance Paths (`DeviceID`).
* **Target Detection:** Implements precise signature verification for common embedded platforms, immediately identifying specific states such as the STM32 Bootloader configuration (`VID_0483&PID_DF11`) or typical ESP32/ESP8266 silicon bridges.

### 2. Automated Driver Repair Engine (`DriverService.cs`)
* **Targeted Recovery:** Activated automatically via the `FIX` sequence when device infrastructure collisions or DFU states are discovered.
* **Safe Re-enumeration:** Triggers hardware re-enumeration routines via the Windows native management APIs. It safely simulates physical cable disconnection and reconnection programmatically to force the Windows PnP (Plug and Play) manager to re-initialize the software communications stack.

### 3. Process Management Subsystem (`ProcessService.cs`)
* **Handle Conflict Resolution:** Prevents `UnauthorizedAccessException` faults caused when an active serial terminal or IDE locks a target COM port.
* **Targeted Termination:** Scans running system processes and cross-references them against an active dictionary of common embedded tools (e.g., *Arduino IDE*, *Betaflight Configurator*, *PuTTY*, *VS Code*, *Cura*). If a collision is confirmed, it forces process termination to instantly free the active serial handle.

### 4. USB & Node Infrastructure Reset
* **Hardware Tree Manipulation:** Programmatically restarts hardware branches directly via specific device instance paths. 
* **Native Tool Integration:** Interfaces with native deployment execution sequences to safely restart target silicon layers without destabilizing adjacent system USB controller nodes.

### 5. Registry Arbiter Cleanup
* **Database Maintenance:** Directly addresses persistent resource exhaustion within the Windows *COM Name Arbiter* database located inside the system registry:
  `HKLM\SYSTEM\CurrentControlSet\Control\COM Name Arbiter`
* **Collision Resolution:** Safely clears corrupted or stale bitmap allocations (`ComDB`) that cause Windows to continuously increment COM port numbers or refuse to reuse unassigned low-index identifiers.

### 6. Asynchronous Verification Terminal (`SerialService.cs`)
* **Data Link Verification:** Contains an isolated, asynchronous serial routing engine built upon `System.IO.Ports`.
* **Runtime Validation:** Allows engineers to immediately initiate a physical communication link, change baud rates, and verify line stability within a single dashboard directly following a driver recovery process.

## Technical Specifications & Requirements

* **Framework:** .NET 8.0 Windows Forms Architecture.
* **Operating System:** Windows 10 / Windows 11.
* **Required Dependencies:** `System.Management`, `System.IO.Ports`.
* **Privileged Access:** **Administrator privileges are strictly required.** Access validation is enforced at startup via `WindowsPrincipal` role checks to permit modification of registry trees, systemic process signals, and active hardware states.

## Installation & Deployment

### Run Standalone Binary
1. Open the **Releases** section of this repository.
2. Download the packaged ZIP archive containing the compiled Beta binaries.
3. Extract all content, right-click `COM_fix.exe`, and select **Run as Administrator**. If executed under restricted tokens, use the internal **Run as Admin** control layout to request elevation.

### Build From Source
An installed .NET 8.0 SDK environment is required for local compilation.

```bash
# Clone the remote source tree
git clone [https://github.com/d7main/COMfix.git](https://github.com/d7main/COMfix.git)

# Enter the root directory
cd COMfix

# Restore dependencies and compile highly optimized Release binaries
dotnet build --configuration Release

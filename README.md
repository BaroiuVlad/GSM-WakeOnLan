# GSM Wake-on-LAN Windows Service

A .NET Windows Background Service designed for the remote execution of Wake-on-LAN commands within a corporate local area network. This system provides a parallel "Out-of-Band" management route, triggering the boot sequence of critical workstations via phone calls or SMS messages received by a physical GSM modem, completely eliminating the dependency on an active internet connection or VPN.

## 🚀 Key Features

* **Automated Hardware Detection:** Dynamic COM port scanning algorithm that automatically identifies and initializes the connected GSM modem upon system boot.
* **Asynchronous Processing:** Non-blocking, continuous polling (2-second interval) of the serial buffer to capture hardware state indicators (`RING`, `+CMTI`).
* **Security & Validation:** Logical firewall that filters incoming caller IDs against an in-memory Whitelist, ensuring highly performant $O(1)$ constant time lookups.
* **Layer 2 Transmission:** Standard "Magic Packet" construction and broadcast transmission over the network using the UDP transport protocol.
* **Automated Deployment:** Professional `.msi` installation package configured via WixSharp for rapid and unified deployment in production environments.

## 📂 Project Structure

The application is structured following Object-Oriented Programming (OOP) principles and Separation of Concerns:

* `📂 Core` -> Handles the main business logic and security validation (`AuthorizationService.cs`).
* `📂 Data` -> Manages data persistence and XML configuration parsing (`XmlConfigManager.cs`).
* `📂 Hardware` -> Encapsulates serial port communication and AT command execution (`GsmModemController.cs`, `ModemDetector.cs`).
* `📂 Models` -> Defines Data Transfer Objects (DTOs) for clean information flow (`AuthorizedUser.cs`, `ServiceConfig.cs`).
* `📂 Network` -> Implements Layer 2 network logic and Magic Packet generation (`WakeOnLanClient.cs`).
* `📄 Program.cs` -> The application entry point, configuring the Dependency Injection container.
* `📄 Worker.cs` -> The core background service engine that orchestrates the data flow.
* `📄 users.xml` -> The local database storing authorized phone numbers and their associated MAC addresses.

## 🛠️ Setup & Administration

1. **Service Installation:** Execute the `GSM_WOL_Installer.msi` package on the host machine. The components will be extracted, and the service will be configured to start automatically in the background (`StartType: Automatic`).
2. **Access Management:** To add or remove authorized users, edit the `users.xml` file located in the installation directory (`C:\Program Files (x86)\GsmWolApp`) with Administrator privileges.
3. **Applying Changes:** For the security policy modifications to take effect, open the OS Services Manager (`services.msc`), locate `GSM_WOL_Service`, and execute the **Restart** command.
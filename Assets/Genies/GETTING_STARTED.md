# Genies SDK

Welcome to the Genies SDK! This package enables you to integrate Genies into your Unity project.

## Getting Started

The quickest way to set up the SDK is to use the **Genies SDK Bootstrap Wizard**:

**Tools > Genies > SDK Bootstrap Wizard**

The wizard will guide you through all necessary configuration steps and can automatically fix most required project settings.

## For detailed documentation

Visit the [Genies Avatar SDK Documentation](https://docs.genies.com/docs/sdk-avatar).

## Quick Setup Summary

1. **Configure Prerequisites**
    - Set IL2CPP as the scripting backend
    - Set .NET Framework to 4.8
    - Configure Vulkan graphics API (Windows/Android)
    - Set ARM64 architecture (Android)
    - Set minimum Android API level to 31 (Android 12.0)
    - Import TextMesh Pro Essential Resources
    - Enable the new Input System (recommended)

    > **Note:** A restart of Unity is required if certain project settings have changed. It is recommended to change all required settings before restarting.

2. **Create Account & App**
    - Sign up or log in at the [Developer Portal](https://hub.genies.com)
    - Create a new app to obtain your Client ID and Client Secret

3. **Configure API Credentials**
    - Enter your Client ID and Client Secret in the wizard
    - Or configure via **Project Settings > Genies > Auth Settings**

4. **Import Samples**
    - Open the Unity Package Manager and import the sample scenes from the SDK package. Use the SDK Bootstrap Wizard for additional guidance.

## Resources

- [Technical Documentation](https://docs.genies.com)
- [Developer Portal](https://hub.genies.com)
- [Genies Support](https://support.genies.com)

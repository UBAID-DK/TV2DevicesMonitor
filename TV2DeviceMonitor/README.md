# Blackmagic Webpresenter HD Monitor

A monitoring and configuration tool for Blackmagic Webpresenter HD devices.

## Features

- Real-time monitoring of multiple Webpresenter HD devices
- Metrics collection and export to Prometheus
- Configuration management
- Alerting via Slack/Teams
- Secure credential storage
- Resilient operation with retry policies

## Requirements

- .NET 8 SDK
- Docker (for containerized deployment)
- Prometheus & Grafana (for monitoring visualization)

## Configuration

1. Copy `appsettings.example.json` to `appsettings.json`
2. Update with your device IPs and credentials
3. For production, use environment variables or secret manager for credentials

## Running the Application

### Local Development

```bash
dotnet run
```

# Bold-ERP Integration Project template

## Overview

This project is a .NET Core application designed to serve as an illustrative example or template on how to integrate Bold with any **ERP**

The integration involves synchronizing multiple collections of entities between the two systems. The integration is
divided into two main workflows:

1. Fetching data from the ERP and applying changes to Bold.
2. Using webhooks to handle updates from Bold and apply them to the ERP.

## Integration Workflows

### 1. Data Fetching from ERP to Bold

The integration periodically fetches data from the **ERP** using a `BackgroundService` and processes the following
collections:

- **SKUs**

For each collection, the last processed change is stored in the database, ensuring that only recent changes are
retrieved and applied to Bold.

### 2. Webhook Integration from Bold to ERP

This workflow leverages **webhooks** from Bold to notify our integration of changes. Upon receiving a webhook, the
integration triggers API calls to create, update, or delete the corresponding entity in the ERP.

## API Clients

### Bold API Client

The client for the **Bold API** is generated using **NSwag**, based on the OpenAPI documentation provided by Bold.

You need to install the NSwag CLI by running:

```bash
dotnet tool install --global NSwag.ConsoleCore
```

Then you need to generate the client by running:

```bash
nswag run nswag.json
```

## Error Handling

In case of an error during data synchronization or webhook processing:

- An email is sent to both the **client** and **support@bold-factory.com** to notify them of the issue.
- If the error is related to a specific entity, it is logged and stored in the **database** for future analysis and
  processing.

The email is hooked into the ILogger, when the log is Error or Critical.

For storing entity specific data, there's a `ErrorLoggerService` that can be used.

This mechanism ensures that errors can be tracked, investigated, and resolved efficiently.
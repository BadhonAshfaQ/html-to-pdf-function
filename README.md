# HtmlToPdfFunctionApp

**HtmlToPdfFunctionApp** is an Azure Function-based microservice designed to generate PDF files from HTML input. This service is optimized for serverless deployments and integrates easily into distributed .NET systems.

## 🛠️ Tech Stack

- **C# (.NET Core)** — Main development language
- **Azure Functions** — Serverless runtime for event-driven execution
- **HTML to PDF Library** — (e.g., DinkToPdf or similar; customizable)
- **Azure App Services** — Compatible hosting platform
- **JSON Configuration** — Environment & app settings
- **REST API** — Trigger endpoints

## 📁 Project Structure

```
/Core       → Core PDF rendering logic  
/Model      → Request/response models  
/Utility    → Helper functions (e.g., sanitization, logging)  
/Vault      → (Optional) secrets or credentials store
Function.cs → Main Azure Function entry point  
Program.cs  → Host setup
```

## 🚀 Usage

Deploy the function to Azure using Azure CLI or GitHub Actions. The function expects an HTTP POST request with HTML content, and returns a PDF file as a stream.

## 🧪 Local Development

1. Clone the repo
2. Restore NuGet packages
3. Run with Azure Functions Core Tools:
   ```
   func start
   ```
4. Test via Postman or cURL with a local endpoint:
   ```
   POST http://localhost:7071/api/render-pdf
   ```

## ⚙️ Configuration

Update the `local.settings.json` for local testing:
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet"
  }
}
```

## 📦 Deployment

- Azure CLI
- Visual Studio Publish
- GitHub Actions

## 📝 License

MIT (or choose your preferred license)

---

Created by [Md. Ashfaquzzaman]

## 🚀 Project Setup Instructions

This project includes automation scripts to scaffold the entire Clean Architecture solution.

The scripts support two modes:

* **Default:** Uses the current folder name as the project name
* **Custom:** Accepts a name argument (e.g., `petstore` → `Petstore`)

---

## 🐧 Linux / macOS (Bash)

### 1. Make Script Executable

```bash
chmod +x setup.sh
```

### 2. Run Script

#### ▶️ Option A: Use Current Folder Name

```bash
./setup.sh
```

> Example:
> If your folder is named `finnotify`, the solution will be created as:
>
> ```
> Finnotify.sln
> ```

---

#### ▶️ Option B: Pass a Custom Project Name

```bash
./setup.sh petstore
```

> This will generate:
>
> ```
> Petstore.sln
> Petstore.Api
> Petstore.Application
> ...
> ```

---

## 🪟 Windows (PowerShell)

### 1. Run Script

#### ▶️ Option A: Use Current Folder Name

```powershell
.\setup.ps1
```

---

#### ▶️ Option B: Pass a Custom Project Name

```powershell
.\setup.ps1 petstore
```

---

## ⚠️ Notes

* The provided name (or folder name) is automatically converted to **PascalCase**

  * `petstore` → `Petstore`
  * `fin-notify` → `FinNotify`

* Ensure you have the following installed:

  * [.NET SDK (8 or later)](https://dotnet.microsoft.com/)
  * PowerShell (for Windows script)

* On Windows, if you encounter execution restrictions, run:

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
```

---

## ✅ What the Script Does

* Creates project structure (src, tests, infra, etc.)
* Generates:

  * API project
  * Domain, Application, Infrastructure layers
  * Test projects
* Sets up project references (Clean Architecture compliant)
* Builds the solution


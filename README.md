# 🔐 PasswordManager

A simple, terminal-based password manager built in **C#**.  
It securely stores your credentials in an encrypted local vault protected by a **master password**.

## ✨ Features
- AES encryption for stored passwords  
- Master password protection using PBKDF2 key derivation  
- JSON-based secure local storage (`vault.json`)  
- Command-line interface with menu navigation  
- Password strength checker and generator  

## 🚀 Usage
```bash
# build and run
dotnet build
dotnet run

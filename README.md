# 🔐 Multi-Layer File Obfuscator (C#)

A console utility demonstrating a **multi-pass substitution cipher** for text files. This program applies a simple, keyed character-shifting mechanism across multiple layers, requiring the successful reversal of each layer for decryption.

⚠️ **Disclaimer:** This tool is for **educational demonstration** of cipher chaining and key management concepts. It uses a basic Vigenère-like cipher and is **not suitable for protecting sensitive or mission-critical data.**

***

## ✨ Features & Mechanism

### Core Logic
The application uses the Unicode/ASCII value of each character in the file content and shifts it by the corresponding value of a character in the key (password), cycling through the key length.

The fundamental operation is a character code addition/subtraction, handled with modulo arithmetic to manage wrap-around within the 16-bit character space ($0x10000$):
$$
\text{Transformed Char} = \text{Char} \pm \text{KeyChar} \pmod{0x10000}
$$

### Multi-Layer Chaining
* **Encryption:** Passwords are applied sequentially (Layer 1 $\to$ Layer 2 $\to$ ...).
* **Decryption:** Passwords must be applied in **reverse order** (Layer $N \to$ Layer $N-1 \to$ ...), reversing the shift for each layer.

### Key Structure
The program enforces a single **Master Password** format, which contains all necessary keys and the layer count for successful decryption:
```
layers|password_1|password_2|...|password_N
```

***

## 🚀 Quick Start

### Prerequisites
* .NET SDK (any modern version, e.g., .NET 8).

### Usage

1.  **Compile:** Build the project using the .NET CLI.
    ```bash
    dotnet build
    ```
2.  **Run:** Execute the compiled application (path may vary based on your project structure).
    ```bash
    .\bin\Debug\net8.0\Program.exe
    ```

### Example Flow (Encryption)

1.  **Select Mode:** Choose **Encrypt** (`en`).
2.  **Configuration:** Specify the file path and the number of layers (1-10).
3.  **Key Generation:** Enter the **First Password** (or type `gen-pwd`). The application auto-generates keys for subsequent layers.
4.  **Master Password Output:** Upon completion, the console displays the crucial **Master Password**. **Securely save this string; it is the only way to restore your file.**

```
Encrypt (en) or Decrypt (de)? en
...
Layers (1-10): 3
...
🔑 MASTER PASSWORD (SAVE THIS!): 3|MyCustomKey|&d4fP!xZ9kL2|Jm-aH+*rQp(7
```

# NoVar - Visual Studio Extension
[![Build](https://github.com/prabhath6/NoVarExtension/actions/workflows/dotnet.yml/badge.svg?branch=main&event=push)](https://github.com/prabhath6/NoVarExtension/actions/workflows/dotnet.yml)

<img src="https://raw.githubusercontent.com/prabhath6/NoVarExtension/refs/heads/main/img/novar.webp" align="right" width="160px" height="120px" >

NoVar is a Visual Studio extension designed to improve code readability by replacing `var` declarations with explicit types. This extension provides two main features that allow users to replace `var` based on their specific needs.

## Features

1. **Replace All Vars**:
   - This feature replaces all instances of `var` in the current document with their corresponding explicit types.
   - Ideal for developers who prefer explicit type declarations throughout their codebase.

2. **Replace Vars Not Initialized with New**:
   - This feature replaces `var` declarations that are not initialized with the `new` keyword.
   - Useful for scenarios where you want to maintain `var` for object creation but prefer explicit types for other initializations.

## How to Use

1. **Install the Extension**:
   - Download and install the NoVar extension from the Visual Studio Marketplace.

2. **Access the Features**:
   - Open a C# file in Visual Studio.
   - Navigate to the `Edit` menu.
   - Select either `Replace All Vars` or `Replace Vars Not Initialized with New` based on your requirement.

## Installation

1. **From Visual Studio Marketplace**:
   - Go to the [Visual Studio Marketplace](https://marketplace.visualstudio.com/).
   - Search for "NoVar".
   - Click "Download" and follow the instructions to install the extension.

2. **From GitHub**:
   - Find the VSIX file the [repository](https://github.com/prabhath6/NoVarExtension/releases)
   - Download the file.
   - Install the extension.

## Workflow

### Replace All Vars
![Replace All Vars](https://github.com/prabhath6/NoVarExtension/blob/main/img/replace_all_vars.gif)

### Replace Vars Not Initialized with New
![Replace Vars Not Initialized with New](https://github.com/prabhath6/NoVarExtension/blob/main/img/replace_not_new_vars.gif)

## Contributing

Contributions are welcome! If you have any suggestions or improvements, please create an issue or submit a pull request.

## License

This project is licensed under the MIT License - see the [LICENSE](./license.txt) file for details.

## Note

- This extension only works for `.cs` files.

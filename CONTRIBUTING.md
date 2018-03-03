# Contributing to ArgsReading

## Prerequisites

* Install [Visual Studio 2017](https://www.visualstudio.com/downloads/) or [Visual Studio Code](https://code.visualstudio.com/) with the [editorconfig extension](https://github.com/editorconfig/editorconfig-vscode).

## Guidelines

* All new code **must** have complete unit tests.
* All public classes, methods, interfaces, enums, etc. **must** have correct XML documentation comments.
* Update [VersionHistory](VersionHistory.md) with a human-readable description of the change.

## How to Build

* Clone the repository: `git clone https://github.com/FacilityApi/Facility.git`
* `cd ArgsReading`
* `powershell .\build.ps1 --target=Test`
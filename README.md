XML DOC to Markdown
===================

> Convert XML documentation files from .NET assemblies into Markdown files.

Author: [Tobias Kiertscher](http://www.mastersign.de/)

License: [MIT](https://opensource.org/licenses/MIT)

[latest release]: https://github.com/mastersign/xmldoc2md/releases

## Usage

```batch
xmldoc2md -TargetPath 'D:\docs\mytool' -Assemblies '.\path\to\assemblies\*.dll', 'D:\another\program.exe'
```

The tool can be called as a PowerShell script `xmldoc2md.ps1` or via the 
`xmldoc2md.cmd` batch file wrapper, which is executable from any Windows command prompt.

## Prerequisites

* Windows 7 or higher
* Microsoft .NET Framework 3.5 or higher
* PowerShell 3 or higher

## Installation

Extract the ZIP file of the [latest release][] into an arbitrary directory.

If you want to have the `xmldoc2md` command always available, add the directory
to your `PATH` environment variable.

## Documentation

```
NAME
    xmldoc2md.ps1

SYNOPSIS
    Convert XML documentation files from .NET assemblies into Markdown files.


SYNTAX
    xmldoc2md.ps1 [-TargetPath] <String> [-Assemblies] <String[]>
    [-FileNameExtension <String>] [-UrlBase <String>] [-UrlFileNameExtension <String>]
    [<CommonParameters>]


DESCRIPTION
    xmldoc2md aims to be a light-weight converter from XML doc files (and their
    assemblies) to Markdown files.

    If you use the /doc switch with a .NET compiler, e.g. by activating the XML
    documentation generation in the project properties inside of Visual Studio,
    the compiler will generate an XML file along with your assembly.
    This XML doc file contains the content of your XML comments on various
    code blocks (classes, properties, methods, ...).

    But the XML doc file itself is not very helpful and needs further processing
    to provide an easy-to-browse documentation.
    Actually, to build meaningfulf documentation, the compiled assembly is needed
    as well, because a lot of information about the types and type members is not
    included in the XML doc file.

    To generate a documentation for a number of assemblies, you can call
    xmldoc2md, specifying the output directory for the Markdown files and the
    paths to the assemblies. xmldoc2md expects to find the XML doc files beside
    their corresponding assembly.

    xmldoc2md generates one Markdown file for every namespace and every public type
    in the given assemblies.


RELATED LINKS
    https://github.com/mastersign/xmldoc2md
    https://daringfireball.net/projects/markdown/

REMARKS
    To see the examples, type: "Get-Help xmldoc2md.ps1 -examples".
    For more information, type: "Get-Help xmldoc2md.ps1 -detailed".
    For information about parameters, type: "Get-Help xmldoc2md.ps1 -parameter *".
    For technical information, type: "Get-Help xmldoc2md.ps1 -full".
```

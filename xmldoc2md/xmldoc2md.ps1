<#
.SYNOPSIS

Convert .NET XML documentation into Markdown.

.PARAMETER Assemblies

Type: [array]

A list of absolute ar relative paths, pointing to the .NET assemblies
(*.dll, *.exe) which will be included in the documentation.

.PARAMETER TargetPath

Type: [string]

A path to the directory where the Markdown files will be stored.
If the specified directory does not exist, it is created.

.PARAMETER FileNameExtension

Type: [string] (optional)
Default Value: ".md"

This file name extension is used for the generated Markdown files.

.PARAMETER UrlBase

Type: [string] (optional)
Default Value: ""

The URL base is used as a prefix for all links in the generated Markdown files.

.PARAMETER UrlFileNameExtension

Type: [string] (optional)
Default Value: ".md"

This file name extension is used in the URLs of cross file links,
(e.g. if one type references another type via a see or seealso XML doc tag).

.DESCRIPTION

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

xmldoc2md generates one Markdown file for every namespace and every public type
in the given assemblies.

.EXAMPLE

To generate the Markdown formatted documentation for one Assembly

.NOTES

You can use tools like the Sandacstle Help File Builder or Doxygen to generate
a static HTML website or a compiled HTML (*.chm) file, but these tools are kind
of heavy and do not support output in Markdown format.

https://github.com/EWSoftware/SHFB
http://www.stack.nl/~dimitri/doxygen/

The Markdown format is a light-weight text markup syntax and is widely used
and supported in a lot of modern software development and collaboration tools.

https://daringfireball.net/projects/markdown/

xmldoc2md works with a combination of code in different languages:

* The overall transformation process is controlled by this PowerShell script.
* A C# code file defining a couple of classes with parsing algorithms
  for cref-style member references, and formatting functions.
* A number of XSLT files for rendering the XML doc files as Markdown.

The PowerShell script compiles the C# file at runtime by calling the Add-Type
commandlet. The XSLT files are used through the .NET class 
System.Xml.Xsl.XsltCompiledTransformation. The types of the runtime compiled
C# files are passed to the XsltCompiledTransformation via an XsltArgumentList
as extension objects. This way, the XSLT files have access to the C# defined
functions.
By using more than one call to the XSL transformations to generate one output
file, the PowerShell script has fine control over the content of the generated
Markdown files.
#>

param (
	[array]$Assemblies = $(throw "You need to specifiy at least one assembly (*.dll, *.exe) to document."),
	[string]$TargetPath = $(throw "You need to provide a target directory."),
	[string]$FileNameExtension = ".md",
	[string]$UrlBase = "",
	[string]$UrlFileNameExtension = ".md"
)

if (!(Test-Path $TargetPath)) { mkdir $TargetPath | Out-Null }
$TargetPath = Resolve-Path $TargetPath

$assemblyPaths = @()
foreach ($p in $Assemblies)
{
	$assemblyPaths += Resolve-Path $p
}
[array]$assemblyPaths = $assemblyPaths | sort

Write-Host "Target Path: $TargetPath"
Write-Host "Assemblies:"
foreach ($p in $assemblyPaths)
{
	$docFile = [IO.Path]::ChangeExtension($p, ".xml")
	if (Test-Path $docFile)
	{
		Write-Host "  - $p"
	}
	else
	{
		Write-Host "  - $p (NO XMLDOC FILE)"
	}
}

Set-Alias new New-Object
$myDir = [IO.Path]::GetDirectoryName($MyInvocation.MyCommand.Definition)

try
{
	Add-Type -Path "$myDir\xmldoc2md.cs" -ReferencedAssemblies "System.Xml.dll"
}
catch
{
	Write-Warning $_.Exception.Message
	return
}
$crefParsing = new Mastersign.XmlDoc.CRefParsing
$crefFormatting = new Mastersign.XmlDoc.CRefFormatting
$crefFormatting.FileNameExtension = $FileNameExtension
$crefFormatting.UrlBase = $UrlBase
$crefFormatting.UrlFileNameExtension = $UrlFileNameExtension

$typeStyleFile = Resolve-Path "$myDir\xmldoc2md_type.xslt"
$memberStyleFile = Resolve-Path "$myDir\xmldoc2md_member.xslt"

$typeStyle = new System.Xml.Xsl.XslCompiledTransform
$typeStyle.Load([string]$typeStyleFile)
$memberStyle = new System.Xml.Xsl.XslCompiledTransform
$memberStyle.Load([string]$memberStyleFile)

$assemblyRefs = @()
$xmlDocs = @()
foreach ($p in $assemblyPaths)
{
	try {
		$assembly = [Reflection.Assembly]::ReflectionOnlyLoadFrom($p)
		$xmlDocFile = [IO.Path]::ChangeExtension($p, ".xml")
		$xmlDoc = new System.Xml.XmlDocument
		if (Test-Path $xmlDocFile)
		{
			$xmlDoc.Load([string]$xmlDocFile)
		}
		$assemblyRefs += $assembly
		$xmlDocs += $xmlDoc
	}
	catch
	{
		Write-Warning "Loading XMLDOC for $p failed."
		Write-Warning $_.Exception.Message
	}
}
$crefFormatting.Assemblies = $assemblyRefs;
$crefFormatting.XmlDocs = $xmlDocs;

function parse-cref($cref)
{
    return [Mastersign.XmlDoc.CRefParsing]::Parse($cref)
}

function public-types()
{
	foreach ($a in $assemblyRefs)
	{
		[System.Reflection.Assembly]$a = $a
		$a.GetTypes() | ? { $_.IsPublic -or $_.IsNestedPublic }
	}
}

function all-member()
{
	foreach ($doc in $xmlDocs)
	{
		$doc.SelectNodes("/doc/members/member")
	}
}

function type-member($typeName) { all-member | ? { $_.name -eq "T:${typeName}" } }
function field-member($typeName) { all-member | ? { $_.name -like "F:${typeName}.*" } }
function event-member($typeName) { all-member | ? { $_.name -like "E:${typeName}.*" } }
function property-member($typeName) { all-member | ? { $_.name -like "P:${typeName}.*" } }
function ctor-member($typeName)
{
	all-member | ? {
		$_.name.StartsWith("M:${typeName}.#ctor") -or
		$_.name.Equals("M:${typeName}.#cctor")
	}
}
function method-member($typeName)
{
	all-member | ? {
		($_.name -match "M:${typeName}\.[^\.]+(\(.+\))?$") -and
		!($_.name.Contains("#ctor") -or $_.name.EndsWith("#cctor"))
	}
}

function transform($writer, $style, [System.Xml.XmlElement]$e)
{
	if (!$e) { return }
    $sr = new System.IO.StringReader ("<partial>" + [string]$e.OuterXml + "</partial>")
    $xr = [System.Xml.XmlReader]::Create($sr)
    $xmlArgs = new System.Xml.Xsl.XsltArgumentList
	$xmlArgs.AddExtensionObject("urn:CRefParsing", $crefParsing)
	$xmlArgs.AddExtensionObject("urn:CRefFormatting", $crefFormatting)
    try
    {
        $style.Transform($xr, $xmlArgs, $writer)
    }
    catch
    {
        Write-Warning $_.Exception
    }
    $xr.Close()
    $sr.Close()
}

function write-indexblock($writer, $nodes, $title)
{
	if (!$nodes) { return }
	$writer.WriteLine("**$title**")
	$writer.WriteLine()
	foreach ($n in $nodes)
	{
		$cref = $n.name
		$label = $crefFormatting.EscapeMarkdown($crefFormatting.Label($cref))
		$anchor = $crefFormatting.Anchor($cref)
		$writer.WriteLine("* [$label](#$anchor)")
	}
	$writer.WriteLine()
}

function write-memberblock($writer, $nodes, $title, $ref)
{
	if (!$nodes) { return }
	$writer.WriteLine("## $title {#$ref}")
	$writer.WriteLine()
    foreach ($n in $nodes)
    {
        transform $writer $memberStyle $n
    }
}

function type-variation([Type]$type)
{
	if ($type.IsInterface) { return "Interface"	}
	elseif ($type.IsEnum) { return "Enumeration" }
	elseif ([MulticastDelegate].IsAssignableFrom($type)) { return "Delegate" }
	elseif ($type.IsValueType) { return "Struct" }
	else { return "Class" }
}

[array]$types = public-types | sort { $_.FullName }

Write-Host "Types"
foreach ($t in $types)
{
	$tFile = $crefFormatting.FileName($t)
	$tCRefName = $crefFormatting.CRefTypeName($t)
	$tCRef = $crefFormatting.CRef($t)
	$tParseResult = parse-cref $tCRef

	Write-Host "  -> $tFile"
    $typeNode = type-member $tCRefName

    $out = [IO.File]::Open("$TargetPath\$tFile", [IO.FileMode]::Create, [IO.FileAccess]::Write)
    $writer = new System.IO.StreamWriter($out, (new System.Text.UTF8Encoding ($false)))

	#$writer.WriteLine("<!--")
	#$writer.WriteLine("Type: $($t.FullName)")
	#$writer.WriteLine("FileName: $tFile")
	#$writer.WriteLine("CRef: $tCRef")
	#$writer.WriteLine("-->")
	$writer.WriteLine()

	$typeVariation = type-variation $t
	$writer.WriteLine("# $($crefFormatting.EscapeMarkdown($crefFormatting.Label($tCref))) $typeVariation")

	if ($typeNode)
	{
		transform $writer $typeStyle $($typeNode.SelectSingleNode("summary"))
	}

	[Reflection.AssemblyName]$aName = $t.Assembly.GetName()
	$writer.WriteLine("**Absolute Name:** ``$($crefFormatting.FullLabel($tCRef))``  ")
	$writer.WriteLine("**Namespace:** $($t.Namespace)  ")
	$writer.WriteLine("**Assembly:** $($aName.Name), Version $($aName.Version)")
	$writer.WriteLine()

    [array]$ctorNodes = ctor-member $tCRefName
    [array]$fieldNodes = field-member $tCRefName
    [array]$eventNodes = event-member $tCRefName
    [array]$propertyNodes = property-member $tCRefName
    [array]$methodNodes = method-member $tCRefName

	if ($typeNode)
	{
		transform $writer $typeStyle $typeNode
		$writer.WriteLine()
	}

	if ($ctorNodes -or $fieldNodes -or $eventNodes -or $propertyNodes -or $methodNodes)
	{
		$writer.WriteLine("## Overview")
		write-indexblock $writer $ctorNodes "Constructors"
		write-indexblock $writer $fieldNodes "Fields"
		write-indexblock $writer $eventNodes "Events"
		write-indexblock $writer $propertyNodes "Properties"
		write-indexblock $writer $methodNodes "Methods"
	}

	write-memberblock $writer $ctorNodes "Constructors" "ctors"
	write-memberblock $writer $fieldNodes "Fields" "fields"
	write-memberblock $writer $eventNodes "Events" "events"
	write-memberblock $writer $propertyNodes "Properties" "properties"
	write-memberblock $writer $methodNodes "Methods" "methods"

    $writer.Close()
    $out.Close()
}

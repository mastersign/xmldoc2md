param (
	[array]$Assemblies,
	[string]$TargetPath
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

Add-Type -Path "$myDir\xmldoc2md.cs" -ReferencedAssemblies "System.Xml.dll"
$crefParsing = new Mastersign.XmlDoc.CRefParsing
$crefFormatting = new Mastersign.XmlDoc.CRefFormatting

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
function ctor-member($typeName) { all-member | ? { $_.name -like "M:${typeName}.#ctor*" } }
function method-member($typeName)
{
	all-member | ? { ($_.name -like "M:${typeName}.*") -and !($_.name -like "*#ctor*") }
}

function transform($writer, $style, [System.Xml.XmlElement]$m)
{
    $sr = new System.IO.StringReader ("<partial>" + [string]$m.OuterXml + "</partial>")
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
        Write-Warning $_.Exception.Message
    }
    $xr.Close()
    $sr.Close()
}

[array]$types = public-types | sort { $_.FullName }

Write-Host "Types"
foreach ($t in $types)
{
	$tFile = $crefFormatting.FileName($t)
	$tCRef = $crefFormatting.CRef($t)
	Write-Host "  - $($t.FullName)"
    $typeNode = type-member $tCRef

    $out = [IO.File]::Open("$TargetPath\$tFile", [IO.FileMode]::Create, [IO.FileAccess]::Write)
    $writer = new System.IO.StreamWriter($out, (new System.Text.UTF8Encoding ($false)))

	$writer.WriteLine("<!--")
	$writer.WriteLine("Type: $($t.FullName)")
	$writer.WriteLine("FileName: $tFile")
	$writer.WriteLine("CRef: $tCRef")
	$writer.WriteLine("-->")
	$writer.WriteLine()

    transform $writer $typeStyle $typeNode

	# Constructors
    [array]$ctorNodes = ctor-member $tCRef
    if ($ctorNodes)
    {
        $writer.WriteLine("## Constructors {#ctors}")
		$writer.WriteLine()
        foreach ($ctorNode in $ctorNodes)
        {
            transform $writer $memberStyle $ctorNode
        }
    }

	# Methods
    [array]$methodNodes = method-member $tCRef
    if ($methodNodes)
    {
        $writer.WriteLine("## Methods {#methods}")
		$writer.WriteLine()
        foreach ($methodNode in $methodNodes)
        {
            transform $writer $memberStyle $methodNode
        }
    }

    $writer.Close()
    $out.Close()
}

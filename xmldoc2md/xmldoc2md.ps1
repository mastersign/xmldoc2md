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

Add-Type -Path "$myDir\xmldoc2md.cs"
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

function parse-cref($cref)
{
    return [Mastersign.XmlDoc.CRefParsing]::Parse($cref)
}

function public-types()
{
	foreach ($a in $assemblyRefs)
	{
		[System.Reflection.Assembly]$a = $a
		$a.GetTypes() | ? { $_.IsPublic }
	}
}

function all-member()
{
	foreach ($doc in $xmlDocs)
	{
		$doc.SelectNodes("/doc/members/member")
	}
}

function type-member($typeName) { all-member | ? { $_.name -like "T:${typeName}" } }
function field-member($typeName) { all-member | ? { $_.name -like "F:${typeName}.*" } }
function event-member($typeName) { all-member | ? { $_.name -like "E:${typeName}.*" } }
function property-member($typeName) { all-member | ? { $_.name -like "P:${typeName}.*" } }
function method-member($typeName) { all-member | ? { $_.name -like "M:${typeName}.*" } }

function transform($style, [System.Xml.XmlElement]$m)
{
    $w = new System.IO.StringWriter
    $sr = new System.IO.StringReader ("<partial>" + [string]$m.OuterXml + "</partial>")
    $xr = [System.Xml.XmlReader]::Create($sr)
    $xmlArgs = new System.Xml.Xsl.XsltArgumentList
	$xmlArgs.AddExtensionObject("urn:CRefParsing", $crefParsing)
	$xmlArgs.AddExtensionObject("urn:CRefFormatting", $crefFormatting)
    try
    {
        $style.Transform($xr, $xmlArgs, $w)
    }
    catch
    {
        Write-Warning $_.Exception.Message
    }
    $xr.Close()
    $sr.Close()
    return $w.ToString()
}

[array]$types = public-types | sort { $_.FullName }

foreach ($t in $types)
{
	$tn = $t.FullName
    $tm = type-member $tn

    $out = [IO.File]::OpenWrite("$TargetPath\${tn}.md")
    $writer = new System.IO.StreamWriter($out, (new System.Text.UTF8Encoding ($false)))

    $writer.WriteLine((transform $typeStyle $tm))
    [array]$methods = method-member $tn
    if ($methods)
    {
        $writer.WriteLine("## Methods {#methods}")
        foreach ($m in $methods)
        {
            $parsedMethod = parse-cref $m.name
            $writer.WriteLine("### " + $parsedMethod.Name)
            $writer.WriteLine((transform $memberStyle $m))
        }
    }

    $writer.Close()
    $out.Close()
}

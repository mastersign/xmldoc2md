<?xml version="1.0"?>

<stylesheet version="1.0" xmlns="http://www.w3.org/1999/XSL/Transform"
            xmlns:cf="urn:CRefFormatting">

  <include href="xmldoc2md.xslt"/>

  <output encoding="utf-8" method="text" omit-xml-declaration="yes" standalone="yes" />

  <template match="member">
    <text>### </text>
    <value-of select="cf:Label(@name)"/>
    <text>
</text>
    <apply-templates select="summary"/>
    <apply-templates select="remarks"/>
    <text>
</text>
  </template>

  <template match="summary">
    <apply-templates />
    <text>
</text>
  </template>

  <template match="remarks">
    <text>### Remarks
</text>
    <apply-templates />
    <text>
</text>
  </template>

</stylesheet>

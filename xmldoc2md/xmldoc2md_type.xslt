<?xml version="1.0"?>

<stylesheet version="1.0" xmlns="http://www.w3.org/1999/XSL/Transform"
            xmlns:cp="urn:CRefParsing"
            xmlns:cf="urn:CRefFormatting">

  <include href="xmldoc2md.xslt"/>

  <output encoding="utf-8" method="text" omit-xml-declaration="yes" standalone="yes" />

  <template match="member">
    <text># </text>
    <value-of select="cf:EscapeMarkdown(cf:Label(@name))"/>
    <text>
Full Name:
`</text>
    <value-of select="cf:FullLabel(@name)"/>
    <text>`

</text>
    <choose>
      <when test="typeparam">

      </when>
    </choose>
    <apply-templates select="summary"/>
    <apply-templates select="remarks"/>
    <if test="typeparam">
      <text>## Type Parameter {#type-parameter}

</text>
      <apply-templates select="typeparam" />
    </if>
  </template>

  <template match="summary">
    <text>## Summary
</text>
    <apply-templates />
    <text>

</text>
  </template>

  <template match="remarks">
    <text>## Remarks
</text>
    <apply-templates />
    <text>

</text>
  </template>

</stylesheet>

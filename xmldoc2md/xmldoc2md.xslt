<?xml version="1.0"?>

<stylesheet version="1.0" xmlns="http://www.w3.org/1999/XSL/Transform"
            xmlns:cp="urn:CRefParsing"
            xmlns:cf="urn:CRefFormatting">

  <template match="text()">
    <value-of select="normalize-space(.)"/>
  </template>

  <template match="c">
    <text> `</text>
    <value-of select="."/>
    <text>` </text>
  </template>

  <template match="strong|b">
    <text> **</text>
    <value-of select="."/>
    <text>** </text>
  </template>

  <template match="em|i">
    <text> _</text>
    <value-of select="."/>
    <text>_ </text>
  </template>

  <template match="para">
    <apply-templates />
    <text>
</text>
  </template>

  <template match="code">
    <text>```
</text>
    <value-of select="text()" />
    <text>```
</text>
  </template>

  <template match="list">
    <apply-templates select="item" />
  </template>

  <template match="item">
    <text>* </text>
    <apply-templates />
    <text>
</text>
  </template>

  <template match="term">
    <text>**</text>
    <value-of select="."/>
    <text>**</text>
  </template>

  <template match="see">
    <text>[`</text>
    <value-of select="cf:Label(@cref)"/>
    <text>`][]
</text>
  </template>

  <template match="typeparam">
    <text>* **</text>
    <value-of select="@name"/>
    <text>**
  </text>
    <apply-templates />
    <text>
</text>
  </template>

</stylesheet>

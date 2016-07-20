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
    <text>
```
</text>
    <value-of select="cf:RemoveIndentation(text())" />
    <text>
```
</text>
  </template>

  <template match="list">
    <text>

</text>
    <choose>
      <when test="@type='table'">
        <apply-templates select="." mode="table" />
      </when>
      <when test="@type='number'">
        <apply-templates select="." mode="number" />
      </when>
      <otherwise>
        <apply-templates select="." mode="bullet" />
      </otherwise>
    </choose>
    <text>
</text>
  </template>
  <template match="list" mode="bullet">
    <apply-templates select="item" mode="bullet" />
  </template>
  <template match="list" mode="number">
    <apply-templates select="item" mode="number" />
  </template>
  <template match="list" mode="table">
    <text>| </text>
    <apply-templates select="listheader/term"/>
    <text> | </text>
    <apply-templates select="listheader/description"/>
    <text> |
|---|---|
</text>
    <apply-templates select="item" mode="table" />
  </template>

  <template match="item" mode="bullet">
    <text>* </text>
    <apply-templates />
    <text>
</text>
  </template>
  <template match="item" mode="number">
    <text>1. </text>
    <apply-templates />
    <text>
</text>
  </template>
  <template match="item" mode="table">
    <text>| </text>
    <apply-templates select="term"/>
    <text> | </text>
    <apply-templates select="description"/>
    <text> |
</text>
  </template>

  <template match="list[@type='bullet']/item/term">
    <text>**</text>
    <value-of select="."/>
    <text>**</text>
    <text>  </text>
    <text>
  </text>
  </template>

  <template match="see">
    <text> [`</text>
    <value-of select="cf:Label(@cref)"/>
    <text>`](#)
</text>
  </template>

  <template match="seealso">
    <text>* [</text>
    <value-of select="cf:FullLabel(@cref)"/>
    <text>](#)
</text>
  </template>

  <template match="param">
    <text>* **</text>
    <value-of select="@name"/>
    <text>**
  </text>
    <apply-templates />
    <text>
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

  <template match="paramref">
    <text> `</text>
    <value-of select="@name"/>
    <text>` </text>
  </template>

  <template match="typeparamref">
    <text> `</text>
    <value-of select="@name"/>
    <text>` </text>
  </template>

  <template match="summary|remarks">
    <apply-templates />
    <text>

</text>
  </template>

</stylesheet>

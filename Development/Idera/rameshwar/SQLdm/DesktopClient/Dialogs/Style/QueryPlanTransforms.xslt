<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:msxsl="urn:schemas-microsoft-com:xslt"
  xmlns:s="http://schemas.microsoft.com/sqlserver/2004/07/showplan"
  exclude-result-prefixes="msxsl s xsl">
  <xsl:output method="html" indent="no" omit-xml-declaration="yes" />

  <!-- Disable built-in recursive processing templates -->
  <xsl:template match="*|/|text()|@*" mode="NodeLabel2" />
  <xsl:template match="*|/|text()|@*" mode="ToolTipDescription" />
  <xsl:template match="*|/|text()|@*" mode="ToolTipDetails" />

  <!-- Default template -->
  <xsl:template match="/">
    <xsl:apply-templates select="s:ShowPlanXML" />
  </xsl:template>

  <!-- Outermost div that contains all statement plans. 

   Vineet - Added or condition to include s:BatchSequence/s:Batch/s:Statements/s:StmtSimple/s:StoredProc/s:Statements/s:StmtSimple as a outermost div-->
  <xsl:template match="s:ShowPlanXML">
    <div class="qp-root">
      <xsl:apply-templates select="s:BatchSequence/s:Batch/s:Statements/s:StmtSimple | s:BatchSequence/s:Batch/s:Statements/s:StmtSimple/s:StoredProc/s:Statements/s:StmtSimple"/>
    </div>
  </xsl:template>
 
  <!-- Matches a branch in the query plan (either an operation or a statement) -->
  <xsl:template match="s:RelOp|s:StmtSimple|s:StmtCond">
    <div class="qp-tr">
      <div>
        <div class="qp-node">
          <xsl:apply-templates select="." mode="NodeIcon" />
          <xsl:apply-templates select="." mode="NodeLabel" />
          <xsl:apply-templates select="." mode="NodeLabel2" />
          <xsl:apply-templates select="." mode="NodeCostLabel" />
          <xsl:call-template name="ToolTip" />
        </div>
      </div>
      <div><xsl:apply-templates select="*/s:RelOp" /></div>
    </div>
  </xsl:template>

  <!-- Writes the tool tip -->
  <xsl:template name="ToolTip">
    <div class="qp-tt">
      <div class="qp-tt-header"><xsl:value-of select="@PhysicalOp | @StatementType" /></div>
      <div><xsl:apply-templates select="." mode="ToolTipDescription" /></div>
      <xsl:call-template name="ToolTipGrid" />
      <xsl:apply-templates select="* | @* | */* | */@*" mode="ToolTipDetails" />
    </div>
  </xsl:template>

  <!-- Writes the grid of node properties to the tool tip -->
  <xsl:template name="ToolTipGrid">
    <table>
      <xsl:call-template name="ToolTipRow">
        <xsl:with-param name="Condition" select="s:QueryPlan/@CachedPlanSize" />
        <xsl:with-param name="Label">Cached plan size</xsl:with-param>
        <xsl:with-param name="Value" select="concat(s:QueryPlan/@CachedPlanSize, ' B')" />
      </xsl:call-template>
      <xsl:call-template name="ToolTipRow">
        <xsl:with-param name="Label">Physical Operation</xsl:with-param>
        <xsl:with-param name="Value" select="@PhysicalOp" />
      </xsl:call-template>
      <xsl:call-template name="ToolTipRow">
        <xsl:with-param name="Label">Logical Operation</xsl:with-param>
        <xsl:with-param name="Value" select="@LogicalOp" />
      </xsl:call-template>
      <xsl:call-template name="ToolTipRow">
        <xsl:with-param name="Label">Actual Number of Rows</xsl:with-param>
        <xsl:with-param name="Value" select="s:RunTimeInformation/s:RunTimeCountersPerThread/@ActualRows" />
      </xsl:call-template>
      <xsl:call-template name="ToolTipRow">
        <xsl:with-param name="Condition" select="@EstimateIO" />
        <xsl:with-param name="Label">Estimated I/O Cost</xsl:with-param>
        <xsl:with-param name="Value">
          <xsl:call-template name="round">
            <xsl:with-param name="value" select="@EstimateIO" />
          </xsl:call-template>
        </xsl:with-param>
      </xsl:call-template>
      <xsl:call-template name="ToolTipRow">
        <xsl:with-param name="Condition" select="@EstimateCPU" />
        <xsl:with-param name="Label">Estimated CPU Cost</xsl:with-param>
        <xsl:with-param name="Value">
          <xsl:call-template name="round">
            <xsl:with-param name="value" select="@EstimateCPU" />
          </xsl:call-template>
        </xsl:with-param>
      </xsl:call-template>
      <!-- TODO: Estimated Number of Executions -->
      <xsl:call-template name="ToolTipRow">
        <xsl:with-param name="Label">Number of Executions</xsl:with-param>
        <xsl:with-param name="Value" select="s:RunTimeInformation/s:RunTimeCountersPerThread/@ActualExecutions" />
      </xsl:call-template>
      <xsl:call-template name="ToolTipRow">
        <xsl:with-param name="Label">Degree of Parallelism</xsl:with-param>
        <xsl:with-param name="Value" select="s:QueryPlan/@DegreeOfParallelism" />
      </xsl:call-template>
      <xsl:call-template name="ToolTipRow">
        <xsl:with-param name="Label">Memory Grant</xsl:with-param>
        <xsl:with-param name="Value" select="s:QueryPlan/@MemoryGrant" />
      </xsl:call-template>
      <xsl:call-template name="ToolTipRow">
        <xsl:with-param name="Condition" select="@EstimateIO | @EstimateCPU" />
        <xsl:with-param name="Label">Estimated Operator Cost</xsl:with-param>
        <xsl:with-param name="Value">
          <xsl:variable name="EstimatedOperatorCost">
            <xsl:call-template name="EstimatedOperatorCost" />
          </xsl:variable>
          <xsl:variable name="TotalCost">
            <xsl:value-of select="ancestor::s:StmtSimple/@StatementSubTreeCost" />
          </xsl:variable>
          
          <xsl:call-template name="round">
            <xsl:with-param name="value" select="$EstimatedOperatorCost" />
          </xsl:call-template>
          (<xsl:value-of select="format-number(number($EstimatedOperatorCost) div number($TotalCost), '0%')" />)
        </xsl:with-param>
      </xsl:call-template>
      <xsl:call-template name="ToolTipRow">
        <xsl:with-param name="Condition" select="@StatementSubTreeCost | @EstimatedTotalSubtreeCost" />
        <xsl:with-param name="Label">Estimated Subtree Cost</xsl:with-param>
        <xsl:with-param name="Value">
          <xsl:call-template name="round">
            <xsl:with-param name="value" select="@StatementSubTreeCost | @EstimatedTotalSubtreeCost" />
          </xsl:call-template>
        </xsl:with-param>
      </xsl:call-template>
      <xsl:call-template name="ToolTipRow">
        <xsl:with-param name="Label">Estimated Number of Rows</xsl:with-param>
        <xsl:with-param name="Value" select="@StatementEstRows | @EstimateRows" />
      </xsl:call-template>
      <xsl:call-template name="ToolTipRow">
        <xsl:with-param name="Condition" select="@AvgRowSize" />
        <xsl:with-param name="Label">Estimated Row Size</xsl:with-param>
        <xsl:with-param name="Value" select="concat(@AvgRowSize, ' B')" />
      </xsl:call-template>
      <!-- TODO: Actual Rebinds
           TODO: Actual Rewinds -->
      <xsl:call-template name="ToolTipRow">
        <xsl:with-param name="Condition" select="s:IndexScan/@Ordered" />
        <xsl:with-param name="Label">Ordered</xsl:with-param>
        <xsl:with-param name="Value">
          <xsl:choose>
            <xsl:when test="s:IndexScan/@Ordered = 1">True</xsl:when>
            <xsl:otherwise>False</xsl:otherwise>
          </xsl:choose>
        </xsl:with-param>
      </xsl:call-template>
      <xsl:call-template name="ToolTipRow">
        <xsl:with-param name="Label">Node ID</xsl:with-param>
        <xsl:with-param name="Value" select="@NodeId" />
      </xsl:call-template>
    </table>
  </xsl:template>

  <!-- Calculates the estimated operator cost. -->
  <xsl:template name="EstimatedOperatorCost">
    <xsl:variable name="EstimateIO">
      <xsl:call-template name="convertSciToNumString">
        <xsl:with-param name="inputVal" select="@EstimateIO" />
      </xsl:call-template>
    </xsl:variable>
    <xsl:variable name="EstimateCPU">
      <xsl:call-template name="convertSciToNumString">
        <xsl:with-param name="inputVal" select="@EstimateCPU" />
      </xsl:call-template>
    </xsl:variable>
    <xsl:value-of select="number($EstimateIO) + number($EstimateCPU)" />
  </xsl:template>

  <!-- Renders a row in the tool tip details table. -->
  <xsl:template name="ToolTipRow">
    <xsl:param name="Label" />
    <xsl:param name="Value" />
    <xsl:param name="Condition" select="$Value" />
    <xsl:if test="$Condition">
      <tr>
        <th><xsl:value-of select="$Label" /></th>
        <td><xsl:value-of select="$Value" /></td>
      </tr>      
    </xsl:if>
  </xsl:template>

  <!-- Prints the name of an object. -->
  <xsl:template match="s:Object | s:ColumnReference" mode="ObjectName">
    <xsl:param name="ExcludeDatabaseName" select="false()" />
    <xsl:choose>
      <xsl:when test="$ExcludeDatabaseName">
        <xsl:for-each select="@Table | @Index | @Column | @Alias">
          <xsl:value-of select="." />
          <xsl:if test="position() != last()">.</xsl:if>
        </xsl:for-each>
      </xsl:when>
      <xsl:otherwise>
        <xsl:for-each select="@Database | @Schema | @Table | @Index | @Column | @Alias">
          <xsl:value-of select="." />
          <xsl:if test="position() != last()">.</xsl:if>
        </xsl:for-each>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <!-- Displays the node cost label. -->
  <xsl:template match="s:RelOp" mode="NodeCostLabel">
    <xsl:variable name="EstimatedOperatorCost"><xsl:call-template name="EstimatedOperatorCost" /></xsl:variable>
    <xsl:variable name="TotalCost"><xsl:value-of select="ancestor::s:StmtSimple/@StatementSubTreeCost" /></xsl:variable>
    <div>Cost: <xsl:value-of select="format-number(number($EstimatedOperatorCost) div number($TotalCost), '0%')" /></div>
  </xsl:template>

  <!-- Dont show the node cost for statements. -->
  <xsl:template match="s:StmtSimple" mode="NodeCostLabel" />

  <!-- 
  ================================
  Tool tip detail sections
  ================================
  The following section contains templates used for writing the detail sections at the bottom of the tool tip,
  for example listing outputs, or information about the object to which an operator applies.
  -->

  <xsl:template match="*/s:Object" mode="ToolTipDetails">
    <!-- TODO: Make sure this works all the time -->
    <div class="qp-bold">Object</div>
    <div><xsl:apply-templates select="." mode="ObjectName" /></div>
  </xsl:template>

  <xsl:template match="s:SetPredicate[s:ScalarOperator/@ScalarString]" mode="ToolTipDetails">
    <div class="qp-bold">Predicate</div>
    <div><xsl:value-of select="s:ScalarOperator/@ScalarString" /></div>
  </xsl:template>

  <xsl:template match="s:OutputList[count(s:ColumnReference) > 0]" mode="ToolTipDetails">
    <div class="qp-bold">Output List</div>
    <xsl:for-each select="s:ColumnReference">
      <div><xsl:apply-templates select="." mode="ObjectName" /></div>
    </xsl:for-each>
  </xsl:template>

  <xsl:template match="s:NestedLoops/s:OuterReferences[count(s:ColumnReference) > 0]" mode="ToolTipDetails">
    <div class="qp-bold">Outer References</div>
    <xsl:for-each select="s:ColumnReference">
      <div><xsl:apply-templates select="." mode="ObjectName" /></div>
    </xsl:for-each>
  </xsl:template>

  <xsl:template match="@StatementText" mode="ToolTipDetails">
    <div class="qp-bold">Statement</div>
    <div><xsl:value-of select="." /></div>
  </xsl:template>

  <xsl:template match="s:Sort/s:OrderBy[count(s:OrderByColumn/s:ColumnReference) > 0]" mode="ToolTipDetails">
    <div class="qp-bold">Order By</div>
    <xsl:for-each select="s:OrderByColumn">
      <div>
        <xsl:apply-templates select="s:ColumnReference" mode="ObjectName" />
        <xsl:choose>
          <xsl:when test="@Ascending = 1"> Ascending</xsl:when>
          <xsl:otherwise> Descending</xsl:otherwise>
        </xsl:choose>
      </div>
    </xsl:for-each>
  </xsl:template>

  <!-- TODO: Seek Predicates -->

  <!-- 
  ================================
  Node icons
  ================================
  The following templates determine what icon should be shown for a given node
  -->

  <!-- Use the logical operation to determine the icon for the "Parallelism" operators. -->
  <xsl:template match="s:RelOp[@PhysicalOp = 'Parallelism']" mode="NodeIcon" priority="1">
    <xsl:element name="div">
      <xsl:attribute name="class">qp-icon-<xsl:value-of select="translate(@LogicalOp, ' ', '')" /></xsl:attribute>
    </xsl:element>
  </xsl:template>

  <!-- Use the physical operation to determine icon if it is present. -->
  <xsl:template match="*[@PhysicalOp]" mode="NodeIcon">
    <xsl:element name="div">
      <xsl:attribute name="class">qp-icon-<xsl:value-of select="translate(@PhysicalOp, ' ', '')" /></xsl:attribute>
    </xsl:element>
  </xsl:template>
  
  <!-- Matches all statements. -->
  <xsl:template match="s:StmtSimple" mode="NodeIcon">
    <div class="qp-icon-Statement"></div>
  </xsl:template>

  <!-- Fallback template - show the Bitmap icon. -->
  <xsl:template match="*" mode="NodeIcon">
    <div class="qp-icon-Catchall"></div>
  </xsl:template>

  <!-- 
  ================================
  Node labels
  ================================
  The following section contains templates used to determine the first (main) label for a node.
  -->

  <xsl:template match="s:RelOp" mode="NodeLabel">
    <div><xsl:value-of select="@PhysicalOp" /></div>
  </xsl:template>

  <xsl:template match="s:StmtSimple" mode="NodeLabel">
    <div><xsl:value-of select="@StatementType" /></div>
  </xsl:template>

  <!--
  ================================
  Node alternate labels
  ================================
  The following section contains templates used to determine the second label to be displayed for a node.
  -->

  <!-- Display the object for any node that has one -->
  <xsl:template match="*[*/s:Object]" mode="NodeLabel2">
    <xsl:variable name="ObjectName">
      <xsl:apply-templates select="*/s:Object" mode="ObjectName">
        <xsl:with-param name="ExcludeDatabaseName" select="true()" />
      </xsl:apply-templates>
    </xsl:variable>
    <div>
      <xsl:value-of select="substring($ObjectName, 0, 36)" />
      <xsl:if test="string-length($ObjectName) >= 36">…</xsl:if>
    </div>
  </xsl:template>

  <!-- Display the logical operation for any node where it is not the same as the physical operation. -->
  <xsl:template match="s:RelOp[@LogicalOp != @PhysicalOp]" mode="NodeLabel2">
    <div>(<xsl:value-of select="@LogicalOp" />)</div>
  </xsl:template>

  <!-- Disable the default template -->
  <xsl:template match="*" mode="NodeLabel2" />

  <!-- 
  ================================
  Tool tip descriptions
  ================================
  The following section contains templates used for writing the description shown in the tool tip.
  

  <xsl:template match="*[@PhysicalOp = 'Table Insert']" mode="ToolTipDescription">Insert input rows into the table specified in Argument field.</xsl:template>
  
  
  <xsl:template match="*[@PhysicalOp = 'Compute Scalar']" mode="ToolTipDescription">Compute new values from existing values in a row.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Sort']" mode="ToolTipDescription">Sort the input.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Clustered Index Scan']" mode="ToolTipDescription">Scanning a clustered index, entirely or only a range.</xsl:template>
  
  <xsl:template match="*[@PhysicalOp = 'Index Scan']" mode="ToolTipDescription">Scanning a non clustered index, entirely or only a range.</xsl:template>
  
  <xsl:template match="*[@PhysicalOp = 'Stream Aggregate']" mode="ToolTipDescription">Compute summary values for groups of rows in a suitably sorted stream.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Hash Match']" mode="ToolTipDescription">Use each row from the top input to build a hash table, and each row from the bottom input to probe into the hash table, outputting all matching rows.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Bitmap']" mode="ToolTipDescription">Bitmap.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Clustered Index Seek']" mode="ToolTipDescription">Scanning a particular range of rows from a clustered index.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Index Seek']" mode="ToolTipDescription">Scan a particular range of rows from a nonclustered index.</xsl:template>

  <xsl:template match="*[@PhysicalOp = 'Parallelism' and @LogicalOp='Repartition Streams']" mode="ToolTipDescription">Repartition Streams.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Parallelism']" mode="ToolTipDescription">An operation involving parallelism.</xsl:template>
  
  <xsl:template match="*[s:TableScan]" mode="ToolTipDescription">Scan rows from a table.</xsl:template>
  <xsl:template match="*[s:NestedLoops]" mode="ToolTipDescription">For each row in the top (outer) input, scan the bottom (inner) input, and output matching rows.</xsl:template>
  <xsl:template match="*[s:Top]" mode="ToolTipDescription">Select the first few rows based on a sort order.</xsl:template>

  -->

  
  <xsl:template match="*[@PhysicalOp = 'Aggregate']" mode="ToolTipDescription">The AGGREGATE operator is a physical or logical operator that computes a new value using the SQL functions MIN, MAX, SUM, COUNT or AVG.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Arithmetic Expression']" mode="ToolTipDescription">The ARITHMETIC EXPRESSION operator computes a new value from values in a row.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Assert']" mode="ToolTipDescription">The ASSERT operator verifies a condition.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Assign']" mode="ToolTipDescription">The ASSIGN operator assigns a value to a variable</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Asnyc Concat']" mode="ToolTipDescription">The ASYNCCONCAT operator is used in remote distributed queries to get output rows from remote child nodes to send to the parent node.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Bitmap']" mode="ToolTipDescription">The BITMAP operator is used to apply filtering to parallel query plans.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Bitmap Create']" mode="ToolTipDescription">The BITMAP CREATE operator is a logical operator that shows where bitmaps are built.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Bookmark Lookup']" mode="ToolTipDescription">The BOOKMARK LOOKUP operator uses a bookmark to lookup rows.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Branch Repartition']" mode="ToolTipDescription">The BRANCH  REPARTITION operator is a logical operator showing where iterators could be executed by parallel threads.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Broadcast']" mode="ToolTipDescription">The BROADCAST operator sends the set of input rows to multiple consumers.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Build Hash']" mode="ToolTipDescription">The BUILD HASH operator indicates that a batch has table has been built.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Cache']" mode="ToolTipDescription">The CACHE operator is a logical operator that caches a single row of data.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Clustered Index Delete']" mode="ToolTipDescription">The CLUSTERED INDEX DELETE operator deletes rows from a clustered index.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Clustered Index Insert']" mode="ToolTipDescription">The CLUSTERED INDEX INSERT operator inserts rows into a clustered index.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Clustered Index Merge']" mode="ToolTipDescription">The CLUSTERED INDEX MERGE operator merges a data stream and a clustered index.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Clustered Index Scan']" mode="ToolTipDescription">The CLUSTERED INDEX SCAN operator scans a clustered index. It returns only rows matching the WHERE clause and sorts the results based on the ORDERED clause.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Clustered Index Seek']" mode="ToolTipDescription">The CLUSTERED INDEX SEEK operator retrieves rows from a clustered index using the seeking ability of indexes.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Clustered Index Update']" mode="ToolTipDescription">The CLUSTERED INDEX UPDATE operator updates rows in the clustered index. It only updates rows matching the WHERE clause.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Collapse']" mode="ToolTipDescription">The COLLAPSE operator merges separate operations into a single more efficient operation.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Columnstore Index Scan']" mode="ToolTipDescription">The COMLUMNSTORE INDEX SCAN operator scans the columnstore index specified in the query execution plan argument.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Compute Scalar']" mode="ToolTipDescription">The COMPUTE SCALAR operator returns a computer scalar value from an evaluated expression.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Concatenation']" mode="ToolTipDescription">The CONCATENTATION operator returns rows scanned from multiple inputs. This is used for statements like UNION ALL.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Constant Scan']" mode="ToolTipDescription">The CONSTANT SCAN operator adds rows into a query.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Convert']" mode="ToolTipDescription">The CONVERT operator converts one scalar type to another.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Cross Join']" mode="ToolTipDescription">The CROSS JOIN operator is a logical operator that joins each row from input with each row from another input.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'catchall']" mode="ToolTipDescription">The CATCHALL operator is a placeholder icon when an operator does not match any other query operators.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Cursor']" mode="ToolTipDescription">The CURSOR operator describes the execution of a query or update that use cursor operations. </xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Declare']" mode="ToolTipDescription">The DECLARE operator allocates a local variable.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Delete']" mode="ToolTipDescription">The DELETE operator deletes rows from an object.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Deleted Scan']" mode="ToolTipDescription">The DELETED SCAN operator scans the deleted table within a trigger.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Distinct']" mode="ToolTipDescription">The DISTINCT operator removes duplicate rows.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Distinct Sort']" mode="ToolTipDescription">The DISTINCT SORT operator removes duplicate rows and sorts the resulting set of rows.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Distribute Streams']" mode="ToolTipDescription">The DISTRIBUTE STREAMS operator is used to break records from a single input to multiple output streams in a parallel query plan.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Dynamic']" mode="ToolTipDescription">The DYNAMIC operator uses a cursor to see all changes made by others.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Eager Spool']" mode="ToolTipDescription">The EAGER SPOOL operator stores each row in the input to tempdb so that this cached data can be used if the operator is rewound.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Fetch Query']" mode="ToolTipDescription">The FETCH QUERY operator returns the rows from a fetch against a cursor.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Filter']" mode="ToolTipDescription">The FILTER operator returns only rows matching a filter.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Flow Distinct']" mode="ToolTipDescription">The FLOW DISTINCT operator removes duplicate rows but returns each row as it is processed.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Full Outer Join']" mode="ToolTipDescription">The FULL OUTER JOIN operator is a logical operator that implements an OUTER JOIN. All matching rows from two streams plus rows for each row in the streams that do not have matches.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Gather Streams']" mode="ToolTipDescription">The GATHER STREAMS operator is used to consume multiple input streams and combine them into a single output stream in parallel query plans.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Hash Match']" mode="ToolTipDescription">The HASH MATCH operator creates a hash table from the rows in the build input.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Inner Join']" mode="ToolTipDescription">The INNER JOIN operator returns rows that satisfy the JOIN of the first input with the second input.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Insert']" mode="ToolTipDescription">The INSERT operator is a logical operator that inserts input rows into a specified object.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Inserted Scan']" mode="ToolTipDescription">The INSERTED SCAN operator scans the inserted table.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Intrinsic']" mode="ToolTipDescription">The INTRINSIC operator runs an internal SQL function.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Iterator']" mode="ToolTipDescription">The ITERATOR operator is a placeholder icon used when no match can be found for iterator operation.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Key Lookup']" mode="ToolTipDescription">The KEY LOOKUP operator signals a lookup using a bookmark on a table with a clustered index.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Keyset']" mode="ToolTipDescription">The KEYSET operator uses a cursor that can only see updates and not inserts.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Language Element']" mode="ToolTipDescription">The LANGUAGE ELEMENT operator is a placeholder icon when no matching language constructs can be found.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Lazy Spool']" mode="ToolTipDescription">The LAZY SPOOL operator stores each row in the input to tempdb so that this cached data can be used if the operator is rewound. The rows are only copied to tempdb as they are processed.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Left Anti Semi Join']" mode="ToolTipDescription">The LEFT ANTI SEMI JOIN operator returns rows from the first input where there is no match in the second input.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Left Outer Join']" mode="ToolTipDescription">The LEFT OUTER JOIN operator returns rows from the first input that satisfy the join with the second input.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Left Semi Join']" mode="ToolTipDescription">The LEFT SEMI JOIN operator returns rows from the first input that have a match with the second input.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Log Row Scan']" mode="ToolTipDescription">The LOG ROW SCAN operator scans the transaction log.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Merge Interval']" mode="ToolTipDescription">The MERGE INTERVAL operator merges multiple intervals to create a minimal set of non-overlapping intervals.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Merge Join']" mode="ToolTipDescription">The MERGE JOIN operator performs one of the many possible join operations.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Nested Loops']" mode="ToolTipDescription">The NESTED LOOPS operator perform the logical operations to satisfy many join operations that need a loop to search the inner table for rows in the outer table.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Nonclustered Index Delete']" mode="ToolTipDescription">The NONCLUSTERED INDEX DELETE operator deleted rows from a non-clustered index.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Index Insert']" mode="ToolTipDescription">The INDEX INSERT operator inserts rows into a non-clustered index.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Index Scan']" mode="ToolTipDescription">The INDEX SCAN operator scans a non-clustered index. It returns only rows matching the WHERE clause</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Index Seek']" mode="ToolTipDescription">The INDEX SEEK operator retrieves rows from a non-clustered index using the seeking ability of indexes.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Index Spool']" mode="ToolTipDescription">The INDEX SPOOL operator copies input rows to tempdb and builds a non-clustered index for these rows.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Nonclustered Index Update']" mode="ToolTipDescription">The NONCLUSTERED INDEX UPDATE operator updates rows from its input in the non-clustered index.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Online Index Insert']" mode="ToolTipDescription">The ONLINE INDEX INSERT operator is a physical operator indicating that an index create, alter or drop is performed online.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Parallelism']" mode="ToolTipDescription">The PARALLELISM operator the logical operations of distribute, gather or repartition streams.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Parameter Table Scan']" mode="ToolTipDescription">The PARAMETER TABLE SCAN operator scans a table that acts as a parameter of the query.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Partial Aggregate']" mode="ToolTipDescription">The PARTIAL AGGREGATE operator is a logical operator that aggregates input rows to prevent writing to disk in parallel plans.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Population Query']" mode="ToolTipDescription">The POPULATION QUERY operator populates a cursor's work table.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Refresh Query']" mode="ToolTipDescription">The REFRESH QUERY operator fetches current data for rows.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Remote Delete']" mode="ToolTipDescription">The REMOTE DELETE operator deletes rows from an remote object.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Remote Index Scan']" mode="ToolTipDescription">The REMOTE INDEX SCAN operator scans a remote index.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Remote Index Seek']" mode="ToolTipDescription">The REMOTE INDEX SEEK operator retrieves rows using a remote index.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Remote Insert']" mode="ToolTipDescription">The REMOTE INSERT operator inserts rows into a remote object.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Remote Query']" mode="ToolTipDescription">The REMOTE QUERY operator submits a query to a remote source.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Remote Scan']" mode="ToolTipDescription">The REMOTE SCAN operator scans a remote object.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Remote Update']" mode="ToolTipDescription">The REMOTE UPDATE operator updates a remote object.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Repartition Streams']" mode="ToolTipDescription">The REPARTITION STREAMS operator creates multiple out streams from multiple input streams while applying a bitmap filter.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Result']" mode="ToolTipDescription">The RESULT operator is the query plan return data.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'RID Lookup']" mode="ToolTipDescription">The RID LOOKUP operator is a bookmark lookup on a heap.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Right Anti Semi Join']" mode="ToolTipDescription">The RIGHT ANTI SEMI JOIN operator returns rows from the second input where the is no match in the first input.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Right Outer Join']" mode="ToolTipDescription">The RIGHT OUTER JOIN operator returns rows from the second input that satisfy the join with the first input.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Right Semi Join']" mode="ToolTipDescription">The RIGHT SEMI JOIN operator returns rows from the second input that have a match with the first input.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Row Count Spool']" mode="ToolTipDescription">The ROW COUNT SPOOL operator  returns empty rows  for each row in the input stream.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Segment']" mode="ToolTipDescription">The SEGMENT operator  uses the value of columns to divide the input set into segments.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Segment Repartition']" mode="ToolTipDescription">The SEGMENT REPARTITION operator  marks the boundaries of regions whose iterators can be run in parallel threads.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Sequence']" mode="ToolTipDescription">The SEQUENCE operator executes each input in sequence.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Sequence Project']" mode="ToolTipDescription">The SEQUENCE PROJECT operator adds columns to the input set, divides the input set into segments and outputs one segment at a time.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Snapshot']" mode="ToolTipDescription">The SNAPSHOT operator creates a cursor that cant see changes by others.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Sort']" mode="ToolTipDescription">The SORT operator sorts incoming rows.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Split']" mode="ToolTipDescription">The SPLIT operator creates a delete and insert operation out of each update operation.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Spool']" mode="ToolTipDescription">The SPOOL operator saves an intermediate query to tempdb.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Stream Aggregate']" mode="ToolTipDescription">The STREAM AGGREGATE operator  groups rows by columns and calculates aggregate expressions.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Switch']" mode="ToolTipDescription">The SWITCH operator  copies  the appropriate input stream to the output stream by evaluating an expression. </xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Table Delete']" mode="ToolTipDescription">The TABLE DELETE operator deletes rows from a table.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Table Insert']" mode="ToolTipDescription">The TABLE INSERT operator inserts rows into a table.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Table Merge']" mode="ToolTipDescription">The TABLE MERGE operator applies a merge data stream to a heap.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Table Scan']" mode="ToolTipDescription">The TABLE SCAN operator retrieves rows from a table.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Table Spool']" mode="ToolTipDescription">The TABLE SPOOL operator scans the input and places the rows into tempdb.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Table Update']" mode="ToolTipDescription">The TABLE UPDATE operator updates rows in a table.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Table-valued function']" mode="ToolTipDescription">The TABLE-VALUED FUNCTION operator evaluates a table-valued function and stores the resulting rows in tempdb.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Top']" mode="ToolTipDescription">The TOP operator  returns only the specified number of rows from the input.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Top N Sort']" mode="ToolTipDescription">The TOP N SORT operator  returns only the specified number of rows from the input and sorts them.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'UDX']" mode="ToolTipDescription">The UDX operator implements XQuery and XPath operations.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Union']" mode="ToolTipDescription">The UNION operator combines multiple inputs and removes duplicates.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Update']" mode="ToolTipDescription">The UPDATE operator updates a specified object from the rows in its input.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'While']" mode="ToolTipDescription">The WHILE operator represents a SQL while loop.</xsl:template>
  <xsl:template match="*[@PhysicalOp = 'Window Spool']" mode="ToolTipDescription">The WINDOW SPOOL operator expands the input rows into sets of rows that represent the window associated with the row.</xsl:template>
  
  <!--
  ================================
  Number handling
  ================================
  The following section contains templates used for handling numbers (scientific notation, rounding etc...)

   Outputs a number rounded to 7 decimal places - to be used for displaying all numbers.
  This template accepts numbers in scientific notation. -->
  <xsl:template name="round">
    <xsl:param name="value" select="0" />
    <xsl:variable name="number">
      <xsl:call-template name="convertSciToNumString">
        <xsl:with-param name="inputVal" select="$value" />
      </xsl:call-template>
    </xsl:variable>
    <xsl:value-of select="round(number($number) * 10000000) div 10000000" />
  </xsl:template>
  
  <!-- Template for handling of scientific numbers
  See: http://www.orm-designer.com/article/xslt-convert-scientific-notation-to-decimal-number -->
  <xsl:variable name="max-exp">
    <xsl:value-of select="'0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000'" />
  </xsl:variable>

  <xsl:template name="convertSciToNumString">
    <xsl:param name="inputVal" select="0" />

    <xsl:variable name="numInput">
      <xsl:value-of select="translate(string($inputVal),'e','E')" />
    </xsl:variable>

    <xsl:choose>
      <xsl:when test="number($numInput) = $numInput">
        <xsl:value-of select="$numInput" />
      </xsl:when> 
      <xsl:otherwise>
        <!-- ==== Mantisa ==== -->
        <xsl:variable name="numMantisa">
          <xsl:value-of select="number(substring-before($numInput,'E'))" />
        </xsl:variable>

        <!-- ==== Exponent ==== -->
        <xsl:variable name="numExponent">
          <xsl:choose>
            <xsl:when test="contains($numInput,'E+')">
              <xsl:value-of select="substring-after($numInput,'E+')" />
            </xsl:when>
            <xsl:otherwise>
              <xsl:value-of select="substring-after($numInput,'E')" />
            </xsl:otherwise>
          </xsl:choose>
        </xsl:variable>

        <!-- ==== Coefficient ==== -->
        <xsl:variable name="numCoefficient">
          <xsl:choose>
            <xsl:when test="$numExponent > 0">
              <xsl:text>1</xsl:text>
              <xsl:value-of select="substring($max-exp, 1, number($numExponent))" />
            </xsl:when>
            <xsl:when test="$numExponent &lt; 0">
              <xsl:text>0.</xsl:text>
              <xsl:value-of select="substring($max-exp, 1, -number($numExponent)-1)" />
              <xsl:text>1</xsl:text>
            </xsl:when>
            <xsl:otherwise>1</xsl:otherwise>
          </xsl:choose>
        </xsl:variable>
        <xsl:value-of select="number($numCoefficient) * number($numMantisa)" />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
</xsl:stylesheet>

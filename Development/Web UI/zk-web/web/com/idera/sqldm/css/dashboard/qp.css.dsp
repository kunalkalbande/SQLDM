div.qp-node
{
	background-color: #FFFFFF;
	margin: 2px;
	padding: 2px;
	border: 0px solid gray;
	font-size: 12px;
	line-height:normal;
	font-family:Arial;
}
.qp-node > div
{
	font-family: Verdana;
	text-align: center;
}
div[class|='qp-icon']
{
	height: 32px;
	width: 32px;
	margin-left: auto;
	margin-right: auto;
	background-repeat: no-repeat;
}
.qp-tt
{
	top: 4em; 
	left: 2em;
	border: 1px solid black;
	background-color: #CCFFFF;
	padding: 2px;
}
.qp-tt div, .qp-tt table
{
	font-family: Sans-Serif;
	text-align: left;
}
.qp-tt table
{
	border-width: 0px;
	border-spacing: 0px;
	margin-top: 10px;
	margin-bottom: 10px;
	width: 100%;
}
.qp-tt td, .qp-tt th
{
	font-size: 11px;
	border-bottom: solid 1px Black;
	padding: 1px;
}
.qp-tt td
{
	text-align: right;
	padding-left: 10px;
}
.qp-tt th
{
	text-align: left;
}
.qp-bold, .qp-tt-header
{
	font-weight: bold;
}
.qp-tt-header
{
	text-align: center;
}

/* Icons */
/*Previous CSSs
.qp-icon-Catchall{background-image:url('../../images/qp/bitmap.gif')}
.qp-icon-ArithmeticExpression{background-image:url('../../images/qp/arithmetic_expression.gif')}
.qp-icon-Assert{background-image:url('../../images/qp/assert.gif')}
.qp-icon-Assign{background-image:url('../../images/qp/assign.gif')}
.qp-icon-Bitmap{background-image:url('../../images/qp/bitmap.gif')}
.qp-icon-BookmarkLookup{background-image:url('../../images/qp/bookmark_lookup.gif')}
.qp-icon-ClusteredIndexDelete{background-image:url('../../images/qp/clustered_index_delete.gif')}
.qp-icon-ClusteredIndexInsert{background-image:url('../../images/qp/clustered_index_insert.gif')}
.qp-icon-ClusteredIndexScan{background-image:url('../../images/qp/clustered_index_scan.gif')}
.qp-icon-ClusteredIndexSeek{background-image:url('../../images/qp/clustered_index_seek.gif')}
.qp-icon-ClusteredIndexUpdate{background-image:url('../../images/qp/clustered_index_update.gif')}
.qp-icon-Collapse{background-image:url('../../images/qp/collapse.gif')}
.qp-icon-ComputeScalar{background-image:url('../../images/qp/compute_scalar.gif')}
.qp-icon-Concatenation{background-image:url('../../images/qp/concatenation.gif')}
.qp-icon-ConstantScan{background-image:url('../../images/qp/constant_scan.gif')}
.qp-icon-Convert{background-image:url('../../images/qp/convert.gif')}
.qp-icon-CursorCatchall{background-image:url('../../images/qp/bitmap.gif')}
.qp-icon-Declare{background-image:url('../../images/qp/declare.gif')}
.qp-icon-Delete{background-image:url('../../images/qp/table_delete.gif')}
.qp-icon-DistributeStreams{background-image:url('../../images/qp/distribute_streams.gif')}
.qp-icon-Dynamic{background-image:url('../../images/qp/dynamic.gif')}
.qp-icon-EagerSpool{background-image:url('../../images/qp/spool.gif')}
.qp-icon-FetchQuery{background-image:url('../../images/qp/fetch_query.gif')}
.qp-icon-Filter{background-image:url('../../images/qp/filter.gif')}
.qp-icon-GatherStreams{background-image:url('../../images/qp/gather_streams.gif')}
.qp-icon-HashMatch{background-image:url('../../images/qp/hash_match.gif')}
.qp-icon-HashMatchRoot{background-image:url('../../images/qp/hash_match.gif')}
.qp-icon-HashMatchTeam{background-image:url('../../images/qp/hash_match.gif')}
.qp-icon-If{background-image:url('../../images/qp/if.gif')}
.qp-icon-Insert{background-image:url('../../images/qp/table_insert.gif')}
.qp-icon-InsertedScan{background-image:url('../../images/qp/inserted_scan.gif')}
.qp-icon-Intrinsic{background-image:url('../../images/qp/intrinsic.gif')}
.qp-icon-IteratorCatchall{background-image:url('../../images/qp/bitmap.gif')}
.qp-icon-Keyset{background-image:url('../../images/qp/keyset.gif')}
.qp-icon-LanguageElementCatchall{background-image:url('../../images/qp/bitmap.gif')}
.qp-icon-LazySpool{background-image:url('../../images/qp/spool.gif')}
.qp-icon-LogRowScan{background-image:url('../../images/qp/log_row_scan.gif')}
.qp-icon-MergeInterval{background-image:url('../../images/qp/merge_interval.gif')}
.qp-icon-MergeJoin{background-image:url('../../images/qp/merge_join.gif')}
.qp-icon-NestedLoops{background-image:url('../../images/qp/nested_loops.gif')}
.qp-icon-NonclusteredIndexDelete{background-image:url('../../images/qp/nonclustered_index_delete.gif')}
.qp-icon-NonclusteredIndexInsert{background-image:url('../../images/qp/nonclustered_index_insert.gif')}
.qp-icon-NonclusteredIndexScan{background-image:url('../../images/qp/nonclustered_index_scan.gif')}
.qp-icon-IndexSeek{background-image:url('../../images/qp/nonclustered_index_seek.gif')}
.qp-icon-NonclusteredIndexSpool{background-image:url('../../images/qp/nonclustered_index_spool.gif')}
.qp-icon-NonclusteredIndexUpdate{background-image:url('../../images/qp/nonclustered_index_update.gif')}
.qp-icon-OnlineIndexInsert{background-image:url('../../images/qp/online_index_insert.gif')}
.qp-icon-ParameterTableScan{background-image:url('../../images/qp/parameter_table_scan.gif')}
.qp-icon-PopulationQuery{background-image:url('../../images/qp/population_query.gif')}
.qp-icon-RdiLookup{background-image:url('../../images/qp/rdi_lookup.gif')}
.qp-icon-RefreshQuery{background-image:url('../../images/qp/refresh_query.gif')}
.qp-icon-RemoteDelete{background-image:url('../../images/qp/remote_delete.gif')}
.qp-icon-RemoteInsert{background-image:url('../../images/qp/remote_insert.gif')}
.qp-icon-RemoteQuery{background-image:url('../../images/qp/remote_query.gif')}
.qp-icon-RemoteScan{background-image:url('../../images/qp/remote_scan.gif')}
.qp-icon-RemoteUpdate{background-image:url('../../images/qp/remote_update.gif')}
.qp-icon-RepartitionStreams{background-image:url('../../images/qp/repartition_streams.gif')}
.qp-icon-Result{background-image:url('../../images/qp/result.gif')}
.qp-icon-RowCountSpool{background-image:url('../../images/qp/row_count_spool.gif')}
.qp-icon-Segment{background-image:url('../../images/qp/segment.gif')}
.qp-icon-Sequence{background-image:url('../../images/qp/sequence.gif')}
.qp-icon-Sequenceproject{background-image:url('../../images/qp/sequenceproject.gif')}
.qp-icon-Snapshot{background-image:url('../../images/qp/snapshot.gif')}
.qp-icon-Sort{background-image:url('../../images/qp/sort.gif')}
.qp-icon-Split{background-image:url('../../images/qp/split.gif')}
.qp-icon-Spool{background-image:url('../../images/qp/spool.gif')}
.qp-icon-Statement{background-image:url('../../images/qp/result.gif')}
.qp-icon-StreamAggregate{background-image:url('../../images/qp/stream_aggregate.gif')}
.qp-icon-Switch{background-image:url('../../images/qp/switch.gif')}
.qp-icon-TableDelete{background-image:url('../../images/qp/table_delete.gif')}
.qp-icon-TableInsert{background-image:url('../../images/qp/table_insert.gif')}
.qp-icon-TableScan{background-image:url('../../images/qp/table_scan.gif')}
.qp-icon-TableSpool{background-image:url('../../images/qp/table_spool.gif')}
.qp-icon-TableUpdate{background-image:url('../../images/qp/table_update.gif')}
.qp-icon-TableValuedFunction{background-image:url('../../images/qp/table_valued_function.gif')}
.qp-icon-Top{background-image:url('../../images/qp/top.gif')}
.qp-icon-Udx{background-image:url('../../images/qp/udx.gif')}
.qp-icon-Update{background-image:url('../../images/qp/table_update.gif.gif')}
.qp-icon-While{background-image:url('../../images/qp/while.gif')}
*/

.qp-icon-Aggregate{background-image:url('../../images/qp/Aggregate.gif')}
.qp-icon-ArithmeticExpression{background-image:url('../../images/qp/ArithmeticExpression.gif')}
.qp-icon-Assert{background-image:url('../../images/qp/Assert.gif')}
.qp-icon-Assign{background-image:url('../../images/qp/Assign.gif')}
.qp-icon-AsnycConcat{background-image:url('../../images/qp/AsnycConcat.gif')}
.qp-icon-Bitmap{background-image:url('../../images/qp/Bitmap.gif')}
.qp-icon-BitmapCreate{background-image:url('../../images/qp/BitmapCreate.gif')}
.qp-icon-BookmarkLookup{background-image:url('../../images/qp/BookmarkLookup.gif')}
.qp-icon-BranchRepartition{background-image:url('../../images/qp/BranchRepartition.gif')}
.qp-icon-Broadcast{background-image:url('../../images/qp/Broadcast.gif')}
.qp-icon-BuildHash{background-image:url('../../images/qp/BuildHash.gif')}
.qp-icon-Cache{background-image:url('../../images/qp/Cache.gif')}
.qp-icon-ClusteredIndexDelete{background-image:url('../../images/qp/ClusteredIndexDelete.gif')}
.qp-icon-ClusteredIndexInsert{background-image:url('../../images/qp/ClusteredIndexInsert.gif')}
.qp-icon-ClusteredIndexMerge{background-image:url('../../images/qp/ClusteredIndexMerge.gif')}
.qp-icon-ClusteredIndexScan{background-image:url('../../images/qp/ClusteredIndexScan.gif')}
.qp-icon-ClusteredIndexSeek{background-image:url('../../images/qp/ClusteredIndexSeek.gif')}
.qp-icon-ClusteredIndexUpdate{background-image:url('../../images/qp/ClusteredIndexUpdate.gif')}
.qp-icon-Collapse{background-image:url('../../images/qp/Collapse.gif')}
.qp-icon-ColumnstoreIndexScan{background-image:url('../../images/qp/ColumnstoreIndexScan.gif')}
.qp-icon-ComputeScalar{background-image:url('../../images/qp/ComputeScalar.gif')}
.qp-icon-Concatenation{background-image:url('../../images/qp/Concatenation.gif')}
.qp-icon-ConstantScan{background-image:url('../../images/qp/ConstantScan.gif')}
.qp-icon-Convert{background-image:url('../../images/qp/Convert.gif')}
.qp-icon-CrossJoin{background-image:url('../../images/qp/CrossJoin.gif')}
.qp-icon-catchall{background-image:url('../../images/qp/catchall.gif')}
.qp-icon-Cursor{background-image:url('../../images/qp/Cursor.gif')}
.qp-icon-Declare{background-image:url('../../images/qp/Declare.gif')}
.qp-icon-Delete{background-image:url('../../images/qp/Delete.gif')}
.qp-icon-DeletedScan{background-image:url('../../images/qp/DeletedScan.gif')}
.qp-icon-Distinct{background-image:url('../../images/qp/Distinct.gif')}
.qp-icon-DistinctSort{background-image:url('../../images/qp/DistinctSort.gif')}
.qp-icon-DistributeStreams{background-image:url('../../images/qp/DistributeStreams.gif')}
.qp-icon-Dynamic{background-image:url('../../images/qp/Dynamic.gif')}
.qp-icon-EagerSpool{background-image:url('../../images/qp/EagerSpool.gif')}
.qp-icon-FetchQuery{background-image:url('../../images/qp/FetchQuery.gif')}
.qp-icon-Filter{background-image:url('../../images/qp/Filter.gif')}
.qp-icon-FlowDistinct{background-image:url('../../images/qp/FlowDistinct.gif')}
.qp-icon-FullOuterJoin{background-image:url('../../images/qp/FullOuterJoin.gif')}
.qp-icon-GatherStreams{background-image:url('../../images/qp/GatherStreams.gif')}
.qp-icon-HashMatch{background-image:url('../../images/qp/HashMatch.gif')}
.qp-icon-InnerJoin{background-image:url('../../images/qp/InnerJoin.gif')}
.qp-icon-Insert{background-image:url('../../images/qp/Insert.gif')}
.qp-icon-InsertedScan{background-image:url('../../images/qp/InsertedScan.gif')}
.qp-icon-Intrinsic{background-image:url('../../images/qp/Intrinsic.gif')}
.qp-icon-Iterator{background-image:url('../../images/qp/Iterator.gif')}
.qp-icon-KeyLookup{background-image:url('../../images/qp/KeyLookup.gif')}
.qp-icon-Keyset{background-image:url('../../images/qp/Keyset.gif')}
.qp-icon-LanguageElement{background-image:url('../../images/qp/LanguageElement.gif')}
.qp-icon-LazySpool{background-image:url('../../images/qp/LazySpool.gif')}
.qp-icon-LeftAntiSemiJoin{background-image:url('../../images/qp/LeftAntiSemiJoin.gif')}
.qp-icon-LeftOuterJoin{background-image:url('../../images/qp/LeftOuterJoin.gif')}
.qp-icon-LeftSemiJoin{background-image:url('../../images/qp/LeftSemiJoin.gif')}
.qp-icon-LogRowScan{background-image:url('../../images/qp/LogRowScan.gif')}
.qp-icon-MergeInterval{background-image:url('../../images/qp/MergeInterval.gif')}
.qp-icon-MergeJoin{background-image:url('../../images/qp/MergeJoin.gif')}
.qp-icon-NestedLoops{background-image:url('../../images/qp/NestedLoops.gif')}
.qp-icon-NonclusteredIndexDelete{background-image:url('../../images/qp/NonclusteredIndexDelete.gif')}
.qp-icon-IndexInsert{background-image:url('../../images/qp/IndexInsert.gif')}
.qp-icon-IndexScan{background-image:url('../../images/qp/IndexScan.gif')}
.qp-icon-IndexSeek{background-image:url('../../images/qp/IndexSeek.gif')}
.qp-icon-IndexSpool{background-image:url('../../images/qp/IndexSpool.gif')}
.qp-icon-NonclusteredIndexUpdate{background-image:url('../../images/qp/NonclusteredIndexUpdate.gif')}
.qp-icon-OnlineIndexInsert{background-image:url('../../images/qp/OnlineIndexInsert.gif')}
.qp-icon-Parallelism{background-image:url('../../images/qp/Parallelism.gif')}
.qp-icon-ParameterTableScan{background-image:url('../../images/qp/ParameterTableScan.gif')}
.qp-icon-PartialAggregate{background-image:url('../../images/qp/PartialAggregate.gif')}
.qp-icon-PopulationQuery{background-image:url('../../images/qp/PopulationQuery.gif')}
.qp-icon-RefreshQuery{background-image:url('../../images/qp/RefreshQuery.gif')}
.qp-icon-RemoteDelete{background-image:url('../../images/qp/RemoteDelete.gif')}
.qp-icon-RemoteIndexScan{background-image:url('../../images/qp/RemoteIndexScan.gif')}
.qp-icon-RemoteIndexSeek{background-image:url('../../images/qp/RemoteIndexSeek.gif')}
.qp-icon-RemoteInsert{background-image:url('../../images/qp/RemoteInsert.gif')}
.qp-icon-RemoteQuery{background-image:url('../../images/qp/RemoteQuery.gif')}
.qp-icon-RemoteScan{background-image:url('../../images/qp/RemoteScan.gif')}
.qp-icon-RemoteUpdate{background-image:url('../../images/qp/RemoteUpdate.gif')}
.qp-icon-RepartitionStreams{background-image:url('../../images/qp/RepartitionStreams.gif')}
.qp-icon-Result{background-image:url('../../images/qp/Result.gif')}
.qp-icon-RIDLookup{background-image:url('../../images/qp/RIDLookup.gif')}
.qp-icon-RightAntiSemiJoin{background-image:url('../../images/qp/RightAntiSemiJoin.gif')}
.qp-icon-RightOuterJoin{background-image:url('../../images/qp/RightOuterJoin.gif')}
.qp-icon-RightSemiJoin{background-image:url('../../images/qp/RightSemiJoin.gif')}
.qp-icon-RowCountSpool{background-image:url('../../images/qp/RowCountSpool.gif')}
.qp-icon-Segment{background-image:url('../../images/qp/Segment.gif')}
.qp-icon-SegmentRepartition{background-image:url('../../images/qp/SegmentRepartition.gif')}
.qp-icon-Sequence{background-image:url('../../images/qp/Sequence.gif')}
.qp-icon-SequenceProject{background-image:url('../../images/qp/SequenceProject.gif')}
.qp-icon-Snapshot{background-image:url('../../images/qp/Snapshot.gif')}
.qp-icon-Sort{background-image:url('../../images/qp/Sort.gif')}
.qp-icon-Split{background-image:url('../../images/qp/Split.gif')}
.qp-icon-Spool{background-image:url('../../images/qp/Spool.gif')}
.qp-icon-StreamAggregate{background-image:url('../../images/qp/StreamAggregate.gif')}
.qp-icon-Switch{background-image:url('../../images/qp/Switch.gif')}
.qp-icon-TableDelete{background-image:url('../../images/qp/TableDelete.gif')}
.qp-icon-TableInsert{background-image:url('../../images/qp/TableInsert.gif')}
.qp-icon-TableMerge{background-image:url('../../images/qp/TableMerge.gif')}
.qp-icon-TableScan{background-image:url('../../images/qp/TableScan.gif')}
.qp-icon-TableSpool{background-image:url('../../images/qp/TableSpool.gif')}
.qp-icon-TableUpdate{background-image:url('../../images/qp/TableUpdate.gif')}
.qp-icon-Table-valuedfunction{background-image:url('../../images/qp/Table_valuedFunction.gif')}
.qp-icon-Top{background-image:url('../../images/qp/Top.gif')}
.qp-icon-TopNSort{background-image:url('../../images/qp/TopNSort.gif')}
.qp-icon-UDX{background-image:url('../../images/qp/UDX.gif')}
.qp-icon-Union{background-image:url('../../images/qp/Union.gif')}
.qp-icon-Update{background-image:url('../../images/qp/Update.gif')}
.qp-icon-While{background-image:url('../../images/qp/While.gif')}
.qp-icon-WindowSpool{background-image:url('../../images/qp/WindowSpool.gif')}


/*Addtional Icons*/
.qp-icon-Statement{background-image:url('../../images/qp/Statement.gif')}

/* Layout - can't touch this */
.qp-tt
{
	position: absolute;
	display: none;
	z-index: 1;
	white-space: normal;
}
div.qp-node:hover .qp-tt
{
	display: block;
}
.qp-tt table
{
	white-space: nowrap;
}
.qp-node
{
	position: relative;
	white-space: nowrap;
}
.qp-tr
{
	display: table;
	margin:5px;
}
.qp-tr > div
{
	display: table-cell;
	padding-left: 15px;
}
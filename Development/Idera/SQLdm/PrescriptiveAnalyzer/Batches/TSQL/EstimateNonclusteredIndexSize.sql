--  Based on the 'Estimating the Size of a Nonclustered Index' from BOL
--  http://msdn.microsoft.com/en-us/library/ms190620.aspx
--
-- Variable replacement:
--	Database       - Database name
--	Table          - Table name
--  NonUniqueIndex - Is the new index unique? (1=true 0=false)
--  FillFactor     - The fill factor for the new index
--  KeyColumns     - The key columns of the new index
--  IncludeColumns - The include columns of the new index

use {Database};

declare @key_columns table (col int);
declare @cluster_columns table (col int);
declare @include_columns table (col int);
declare @colid int;
declare @loop int;
declare @tblid int;

declare @NonUnique bit;
declare @HasNullableColumns bit;
declare @ClusterHasNullableColumns bit;

declare @Num_Rows bigint; -- number of rows in the table
declare @Num_Key_Cols int; -- total number of key columns
declare @Fixed_Key_Size int; -- total byte size of all fixed-length key columns
declare @Num_Variable_Key_Cols int; -- number of variable-length key columns
declare @Max_Var_Key_Size int; -- maximum byte size of all variable-length key columns
declare @Index_Null_Bitmap smallint;
declare @Leaf_Null_Bitmap smallint;
declare @Variable_Key_Size int;
declare @Index_Row_Size int;
declare @Index_Rows_Per_Page int;
declare @Num_Leaf_Cols int;
declare @Fixed_Leaf_Size int;
declare @Num_Variable_Leaf_Cols int;
declare @Max_Var_Leaf_Size int;
declare @Variable_Leaf_Size int;
declare @Leaf_Row_Size int;
declare @Leaf_Rows_Per_Page int;
declare @Fill_Factor int;
declare @Free_Rows_Per_Page int;
declare @Leaf_Space_Used bigint;
declare @Num_Leaf_Pages bigint;

set @tblid = object_id({SchemaTable});
set @NonUnique = {NonUniqueIndex};
set @Fill_Factor = {FillFactor};

-- build out list of key columns and include columns that will be used in creating the
-- non-clustered index
{KeyColumns}
{IncludeColumns}

-- determine the columns on the clustered index that are not key columns on this index.
-- this will be used later to determine what additional information we need to add
-- in to each row for linking back to the clustered index.
insert into @cluster_columns(col) select column_id from sys.index_columns 
		where object_id = @tblid
		and index_id = 1
		and column_id not in (select col from @key_columns);

select @Num_Key_Cols = count(*) from @key_columns;

select @Num_Rows = sum(row_count) from sys.dm_db_partition_stats
	where object_id = @tblid
	and index_id < 2;

set @HasNullableColumns = 0;
set @ClusterHasNullableColumns = 0;
set @Fixed_Key_Size = 0;
set @Num_Variable_Key_Cols = 0;
set @Max_Var_Key_Size = 0;
set @Index_Null_Bitmap = 0;
set @Leaf_Null_Bitmap = 0;
set @Variable_Key_Size = 0;
set @Index_Row_Size = 0;
set @Leaf_Row_Size = 0;
set @Leaf_Rows_Per_Page = 0;
set @Variable_Leaf_Size = 0;
set @Free_Rows_Per_Page = 0;
set @Leaf_Space_Used = 0;
set @Num_Leaf_Pages = 0;

-- Step 1. Calculate Variables for Use in Steps 2 and 3
set @loop = 0;
while (select count(*) from @key_columns) > 0
begin
    set @loop = @loop + 1;
    if @loop > 100 break; -- sanity check to prevent infinite looping which should never happen
    select top 1 @colid = col from @key_columns;
    delete from @key_columns where col = @colid;

	select 
		@Fixed_Key_Size = @Fixed_Key_Size + (case when sc.max_length = -1 then 8000 else sc.max_length end * (1 - (case when st.system_type_id in (165, 167, 173, 175, 231, 239, 240) then 1 else 0 end))),
		@Num_Variable_Key_Cols = @Num_Variable_Key_Cols + (case when st.system_type_id in (165, 167, 173, 175, 231, 239, 240) then 1 else 0 end),
		@Max_Var_Key_Size = @Max_Var_Key_Size + (case when sc.max_length = -1 then 8000 else sc.max_length end * (case when st.system_type_id in (165, 167, 173, 175, 231, 239, 240) then 1 else 0 end)),
		@HasNullableColumns = (case when sc.is_nullable = 1 then 1 else @HasNullableColumns end)
	from sys.objects as so
	join sys.columns as sc
		on (so.object_id = sc.object_id)
	join sys.types as st
		on (sc.system_type_id = st.system_type_id)
	where sc.object_id = @tblid
	and sc.column_id = @colid;
end;

-- if the index is nonunique, account for the data row locator that is required
if @NonUnique = 1
begin
	-- If the nonclustered index is over a heap, the data row locator is the heap RID. 
	-- This is a size of 8 bytes
	if objectpropertyex(@tblid, 'TableHasClustIndex') = 0
	begin
		set @Num_Key_Cols = @Num_Key_Cols + 1;
		set @Num_Variable_Key_Cols = @Num_Variable_Key_Cols + 1;
		set @Max_Var_Key_Size = @Max_Var_Key_Size + 8;
	end
	else
	begin
		-- If the nonclustered index is over a clustered index, the data row locator is the clustering key. 
		-- The columns that must be combined with the nonclustered index key are those columns in the 
		-- clustering key that are not already present in the set of nonclustered index key columns.

		--Add in the number of clustering key columns not in the set of nonclustered index key columns (+ 1 if the clustered index is nonunique)
		select @Num_Key_Cols = @Num_Key_Cols + count(*) from @cluster_columns;

		--loop through the clustered index columns for the table and adjust our counts as needed.
		set @loop = 0;
		while (select count(*) from @cluster_columns) > 0
		begin
			set @loop = @loop + 1;
			if @loop > 100 break; -- sanity check to prevent infinite looping which should never happen
			select top 1 @colid = col from @cluster_columns;
			delete from @cluster_columns where col = @colid;

			select 
				--Add in the total byte size of fixed-length clustering key columns not in the set of nonclustered index key columns
				@Fixed_Key_Size = @Fixed_Key_Size + (case when sc.max_length = -1 then 8000 else sc.max_length end * (1 - (case when st.system_type_id in (165, 167, 173, 175, 231, 239, 240) then 1 else 0 end))),
				--Add in the number of variable-length clustering key columns not in the set of nonclustered index key columns (+ 1 if the clustered index is nonunique)
				@Num_Variable_Key_Cols = @Num_Variable_Key_Cols + (case when st.system_type_id in (165, 167, 173, 175, 231, 239, 240) then 1 else 0 end),
				--Add the maximum byte size of variable-length clustering key columns not in the set of nonclustered index key columns (+ 4 if the clustered index is nonunique)
				@Max_Var_Key_Size = @Max_Var_Key_Size + (case when sc.max_length = -1 then 8000 else sc.max_length end * (case when st.system_type_id in (165, 167, 173, 175, 231, 239, 240) then 1 else 0 end)),
				@ClusterHasNullableColumns = (case when sc.is_nullable = 1 then 1 else @ClusterHasNullableColumns end)
			from sys.objects as so
			join sys.columns as sc
				on (so.object_id = sc.object_id)
			join sys.types as st
				on (sc.system_type_id = st.system_type_id)
			where sc.object_id = @tblid
			and sc.column_id = @colid;
		end;
		if (select is_unique from sys.indexes where object_id = @tblid and index_id = 1) = 0
		begin
			set @Num_Key_Cols = @Num_Key_Cols + 1; -- (+ 1 if the clustered index is nonunique)
			set @Num_Variable_Key_Cols = @Num_Variable_Key_Cols + 1; -- (+ 1 if the clustered index is nonunique)
			set @Max_Var_Key_Size = @Max_Var_Key_Size + 4; -- (4 1 if the clustered index is nonunique)
		end
	end
end

--4.Part of the row, known as the null bitmap, may be reserved to manage column nullability. Calculate its size:
--If there are nullable columns in the index key, including any necessary clustering key columns as described in Step 1.3, part of the index row is reserved for the null bitmap.
--Index_Null_Bitmap = 2 + ((number of columns in the index row + 7) / 8) 
--Only the integer part of the previous expression should be used. Discard any remainder. 
--If there are no nullable key columns, set Index_Null_Bitmap to 0.
if @HasNullableColumns = 1 or @ClusterHasNullableColumns = 1
begin 
	set @Index_Null_Bitmap = (2 + (( @Num_Key_Cols + 7) / 8 ) );
end;

--5.Calculate the variable length data size:
--If there are variable-length columns in the index key, including any necessary clustered index key columns, determine how much space is used to store the columns within the index row: 
--Variable_Key_Size = 2 + (Num_Variable_Key_Cols x 2) + Max_Var_Key_Size 
--The bytes added to Max_Var_Key_Size are for tracking each variable column. This formula assumes that all variable-length columns are 100 percent full. If you anticipate that a smaller percentage of the variable-length column storage space will be used, you can adjust the Max_Var_Key_Size value by that percentage to yield a more accurate estimate of the overall table size. 
--If there are no variable-length columns, set Variable_Key_Size to 0. 
if @Num_Variable_Key_Cols <> 0
begin
	set @Variable_Key_Size = 2 + @Num_Variable_Key_Cols + @Num_Variable_Key_Cols + @Max_Var_Key_Size
end

--6.Calculate the index row size: 
--Index_Row_Size = Fixed_Key_Size + Variable_Key_Size + Index_Null_Bitmap 
set @Index_Row_Size = @Fixed_Key_Size + @Variable_Key_Size + @Index_Null_Bitmap;
set @Index_Row_Size = @Index_Row_Size + 1; -- + 1 (for row header overhead of an index row) 
set @Index_Row_Size = @Index_Row_Size + 6; -- + 6 (for the child page ID pointer)

--7.Calculate the number of index rows per page (8096 free bytes per page): 
--Index_Rows_Per_Page = 8096 / (Index_Row_Size + 2) 
--Because index rows do not span pages, the number of index rows per page should be rounded down to the nearest whole row. The 2 in the formula is for the row's entry in the page's slot array.
set @Index_Rows_Per_Page = 8096 / (@Index_Row_Size + 2)

-- Step 2. Calculate the Space Used to Store Index Information in the Leaf Level 
--You can use the following steps to estimate the amount of space that is required to store the leaf level of the index. You will need the values preserved from Step 1 to complete this step. 
--
--1.Specify the number of fixed-length and variable-length columns at the leaf level and calculate the space that is required for their storage:
--
--If the nonclustered index does not have any included columns, use the values from Step 1, including any modifications determined in Step 1.3:
if (select count(*) from @include_columns) = 0
begin
	set @Num_Leaf_Cols = @Num_Key_Cols;
	set @Fixed_Leaf_Size = @Fixed_Key_Size;
	set @Num_Variable_Leaf_Cols = @Num_Variable_Key_Cols;
	set @Max_Var_Leaf_Size = @Max_Var_Key_Size;
end
else
begin
	--If the nonclustered index does have included columns, add the appropriate values to the values from Step 1, including any modifications in Step 1.3. The size of a column depends on the data type and length specification. For more information, see Data Types (Database Engine).
	select @Num_Leaf_Cols = @Num_Key_Cols + count(*) from @include_columns;
	set @Fixed_Leaf_Size = @Fixed_Key_Size;
	set @Num_Variable_Leaf_Cols = @Num_Variable_Key_Cols;
	set @Max_Var_Leaf_Size = @Max_Var_Key_Size;

	--loop through the include columns for the index and adjust our counts as needed.
	set @loop = 0;
	while (select count(*) from @include_columns) > 0
	begin
		set @loop = @loop + 1;
		if @loop > 100 break; -- sanity check to prevent infinite looping which should never happen
		select top 1 @colid = col from @include_columns;
		delete from @include_columns where col = @colid;

		select 
			@Fixed_Leaf_Size = @Fixed_Leaf_Size + (case when sc.max_length = -1 then 8000 else sc.max_length end * (1 - (case when st.system_type_id in (165, 167, 173, 175, 231, 239, 240) then 1 else 0 end))),
			@Num_Variable_Leaf_Cols = @Num_Variable_Leaf_Cols + (case when st.system_type_id in (165, 167, 173, 175, 231, 239, 240) then 1 else 0 end),
			@Max_Var_Leaf_Size = @Max_Var_Leaf_Size + (case when sc.max_length = -1 then 8000 else sc.max_length end * (case when st.system_type_id in (165, 167, 173, 175, 231, 239, 240) then 1 else 0 end))
		from sys.objects as so
		join sys.columns as sc
			on (so.object_id = sc.object_id)
		join sys.types as st
			on (sc.system_type_id = st.system_type_id)
		where sc.object_id = @tblid
		and sc.column_id = @colid;
	end;
end;

--2.Account for the data row locator:
--If the nonclustered index is nonunique, the overhead for the data row locator has already been considered in Step 1.3 and no additional modifications are required. Go to the next step.
--If the nonclustered index is unique, the data row locator must be accounted for in all rows at the leaf level.
if @NonUnique = 0
begin

	-- If the nonclustered index is over a heap, the data row locator is the heap RID (size 8 bytes).
	if objectpropertyex(@tblid, 'TableHasClustIndex') = 0
	begin
		set @Num_Leaf_Cols = @Num_Leaf_Cols + 1;
		set @Num_Variable_Leaf_Cols = @Num_Variable_Leaf_Cols + 1;
		set @Max_Var_Leaf_Size = @Max_Var_Leaf_Size + 8;
	end
	else
	begin
		--If the nonclustered index is over a clustered index, the data row locator is the clustering key. 
		--The columns that must be combined with the nonclustered index key are those columns in the 
		--clustering key that are not already present in the set of nonclustered index key columns.

		--Add in number of clustering key columns not in the set of nonclustered index key columns (+ 1 if the clustered index is nonunique)
		select @Num_Leaf_Cols = @Num_Leaf_Cols + count(*) from @cluster_columns;

		--loop through the clustered index columns for the table and adjust our counts as needed.
		set @loop = 0;
		while (select count(*) from @cluster_columns) > 0
		begin
			set @loop = @loop + 1;
			if @loop > 100 break; -- sanity check to prevent infinite looping which should never happen
			select top 1 @colid = col from @cluster_columns;
			delete from @cluster_columns where col = @colid;

			select 
				--Add in the total byte size of fixed-length clustering key columns not in the set of nonclustered index key columns
				@Fixed_Leaf_Size = @Fixed_Leaf_Size + (case when sc.max_length = -1 then 8000 else sc.max_length end * (1 - (case when st.system_type_id in (165, 167, 173, 175, 231, 239, 240) then 1 else 0 end))),
				--Add in the number of variable-length clustering key columns not in the set of nonclustered index key columns (+ 1 if the clustered index is nonunique)
				@Num_Variable_Leaf_Cols = @Num_Variable_Leaf_Cols + (case when st.system_type_id in (165, 167, 173, 175, 231, 239, 240) then 1 else 0 end),
				--Add the maximum byte size of variable-length clustering key columns not in the set of nonclustered index key columns (+ 4 if the clustered index is nonunique)
				@Max_Var_Leaf_Size = @Max_Var_Leaf_Size + (case when sc.max_length = -1 then 8000 else sc.max_length end * (case when st.system_type_id in (165, 167, 173, 175, 231, 239, 240) then 1 else 0 end)),
				@ClusterHasNullableColumns = (case when sc.is_nullable = 1 then 1 else @ClusterHasNullableColumns end)
			from sys.objects as so
			join sys.columns as sc
				on (so.object_id = sc.object_id)
			join sys.types as st
				on (sc.system_type_id = st.system_type_id)
			where sc.object_id = @tblid
			and sc.column_id = @colid;
		end;
		if (select is_unique from sys.indexes where object_id = @tblid and index_id = 1) = 0
		begin
			set @Num_Leaf_Cols = @Num_Leaf_Cols + 1; -- (+ 1 if the clustered index is nonunique)
			set @Num_Variable_Leaf_Cols = @Num_Variable_Leaf_Cols + 1; -- (+ 1 if the clustered index is nonunique)
			set @Max_Var_Leaf_Size = @Max_Var_Leaf_Size + 4; -- (4 if the clustered index is nonunique)
		end
	end
end;

--3.Calculate the null bitmap size:
--Leaf_Null_Bitmap = 2 + ((Num_Leaf_Cols + 7) / 8) 
--Only the integer part of the previous expression should be used. Discard any remainder. 
if @HasNullableColumns = 1 or @ClusterHasNullableColumns = 1
begin 
	set @Leaf_Null_Bitmap = (2 + (( @Num_Leaf_Cols + 7) / 8 ) );
end;

--4.Calculate the variable length data size:
--If there are variable-length columns in the index key, including any necessary clustering key columns as described previously in Step 2.2, determine how much space is used to store the columns within the index row: 
--Variable_Leaf_Size = 2 + (Num_Variable_Leaf_Cols x 2) + Max_Var_Leaf_Size 
--The bytes added to Max_Var_Key_Size are for tracking each variable column. This formula assumes that all variable-length columns are 100 percent full. If you anticipate that a smaller percentage of the variable-length column storage space will be used, you can adjust the Max_Var_Leaf_Size value by that percentage to yield a more accurate estimate of the overall table size. 
--If there are no variable-length columns, set Variable_Leaf_Size to 0. 
if @Num_Variable_Leaf_Cols <> 0
begin
	set @Variable_Leaf_Size = 2 + @Num_Variable_Leaf_Cols + @Num_Variable_Leaf_Cols + @Max_Var_Leaf_Size
end

--5.Calculate the index row size: 
--Leaf_Row_Size = Fixed_Leaf_Size + Variable_Leaf_Size + Leaf_Null_Bitmap + 1 (for row header overhead of an index row) + 6 (for the child page ID pointer)
set @Leaf_Row_Size = @Fixed_Leaf_Size + @Variable_Leaf_Size + @Leaf_Null_Bitmap; 
set @Leaf_Row_Size = @Leaf_Row_Size + 1; --(for row header overhead of an index row) 
set @Leaf_Row_Size = @Leaf_Row_Size + 6; --(for the child page ID pointer)

--6.Calculate the number of index rows per page (8096 free bytes per page): 
--Because index rows do not span pages, the number of index rows per page should be rounded down to the nearest whole row. The 2 in the formula is for the row's entry in the page's slot array.
set @Leaf_Rows_Per_Page = 8096 / (@Leaf_Row_Size + 2);

--7.Calculate the number of reserved free rows per page, based on the fill factor specified:
--The fill factor used in the calculation is an integer value instead of a percentage. Because rows do not span pages, the number of rows per page should be rounded down to the nearest whole row. As the fill factor grows, more data will be stored on each page and there will be fewer pages. The 2 in the formula is for the row's entry in the page's slot array.
set @Free_Rows_Per_Page = 8096 * ((100 - @Fill_Factor) / 100) / (@Leaf_Row_Size + 2);

--8.Calculate the number of pages required to store all the rows: 
--The number of pages estimated should be rounded up to the nearest whole page. 
set @Num_Leaf_Pages = @Num_Rows / (@Leaf_Rows_Per_Page - @Free_Rows_Per_Page);

--9.Calculate the size of the index (8192 total bytes per page): 
set @Leaf_Space_Used = 8192 * @Num_Leaf_Pages;

-- Step 3. Calculate the Space Used to Store Index Information in the Non-leaf Levels 
--Follow these steps to estimate the amount of space that is required to store the intermediate 
--and root levels of the index. You will need the values preserved from steps 2 and 3 to complete 
--this step. 
--
-- The caller will handle the remainder of the calculation...

select 
	[Index_Rows_Per_Page]=@Index_Rows_Per_Page,
	[Num_Leaf_Pages ]=@Num_Leaf_Pages,
	[Leaf_Space_Used]=@Leaf_Space_Used;

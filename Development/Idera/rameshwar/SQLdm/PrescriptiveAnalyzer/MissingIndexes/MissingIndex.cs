using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Globalization;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ShowPlan;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;

namespace Idera.SQLdm.PrescriptiveAnalyzer.MissingIndexes
{
    public enum MissingIndexColumnType
    {
        Equality,
        Inequality,
        Include,
    }

    internal class MissingIndex
    {
        private static Int64 _staticCount = 0;
        private static ColumnTypeEqualityComparer _columnTypeEqualityComparer = new ColumnTypeEqualityComparer();
        private List<ColumnType> _equal = new List<ColumnType>();
        private List<ColumnType> _notequal = new List<ColumnType>();
        private List<ColumnType> _include = new List<ColumnType>();
        private readonly Int64 _id = 0;
        private MissingIndex() { _id = System.Threading.Interlocked.Increment(ref _staticCount); }
        public MissingIndex(ColumnGroupType[] cgts) : this()
        {
            foreach (ColumnGroupType cgt in cgts)
            {
                switch (cgt.Usage)
                {
                    case (ColumnGroupTypeUsage.EQUALITY): { _equal.AddRange(cgt.Column); break; }
                    case (ColumnGroupTypeUsage.INEQUALITY): { _notequal.AddRange(cgt.Column); break; }
                    case (ColumnGroupTypeUsage.INCLUDE): { _include.AddRange(cgt.Column); break; }
                }
            }
            ColumnTypeComparer c = new ColumnTypeComparer();
            _equal.Sort(c);
            _notequal.Sort(c);
            _include.Sort(c);
        }
        public MissingIndex(IEnumerable<ColumnType> equal, IEnumerable<ColumnType> notequal, IEnumerable<ColumnType> include) : this()
        {
            if (null != equal) _equal.AddRange(equal);
            if (null != notequal) _notequal.AddRange(notequal);
            if (null != include) _include.AddRange(include);
        }

        public Int64 ID { get { return (_id); } }

        public static IEnumerable<MissingIndex> CreateConsolidatedIndexes(IEnumerable<MissingIndex> mis)
        {
            MissingIndex mi = new MissingIndex();
            List<ColumnType> allColumns = new List<ColumnType>();
            List<ColumnType> cts;
            List<ColumnType> tempAll;
            foreach (MissingIndex i in mis)
            {
                mi._equal.AddRange(i._equal);
                mi._notequal.AddRange(i._notequal);
                mi._include.AddRange(i._include);
                yield return (i);
                allColumns.AddRange(i._equal);
                allColumns.AddRange(i._notequal);
                allColumns.AddRange(i._include);
            }
            RemoveDuplicates(allColumns);
            //----------------------------------------------------------------------------
            // At this point we have all of the equal columns that were used in all of the
            // indexes.  Sort this by the column used the most and return missing index
            // information for the various combinations of these columns.
            // 
            //cts = mi._equal.GroupBy(x => x.Name).OrderByDescending(g => g.Count()).Select(a => a.ElementAt(0)).ToList();
            Dictionary<string, List<ColumnType>> dict = new Dictionary<string, List<ColumnType>>();
            foreach (ColumnType ct in mi._equal)
            {
                if (!dict.ContainsKey(ct.Name))
                {
                    List<ColumnType> l = new List<ColumnType>();
                    l.Add(ct);
                    dict[ct.Name] = l;
                }
                else
                {
                    dict[ct.Name].Add(ct);
                }
            }
            string maxKey = "";
            foreach (string str in dict.Keys)
            {
                int max = 0;
                if (dict[str].Count > max)
                {
                    max = dict[str].Count;
                    maxKey = str;
                }
            }
            cts = dict[maxKey];

            if (null != cts)
            {
                while (cts.Count > 0)
                {
                    yield return (new MissingIndex(cts, null, null));
                    tempAll = UniqueColumns(allColumns, cts);
                    if (tempAll.Count > 0) yield return (new MissingIndex(cts, null, tempAll));
                    cts.RemoveAt(cts.Count - 1);
                }
            }
            //----------------------------------------------------------------------------
            // now do the same as above but for the excluded columns.
            // 
            //cts = mi._notequal.GroupBy(x => x.Name).OrderByDescending(g => g.Count()).Select(a => a.ElementAt(0)).ToList();
            Dictionary<string, List<ColumnType>> dictNotEqual = new Dictionary<string, List<ColumnType>>();
            foreach (ColumnType ct in mi._notequal)
            {
                if (!dictNotEqual.ContainsKey(ct.Name))
                {
                    List<ColumnType> l = new List<ColumnType>();
                    l.Add(ct);
                    dictNotEqual[ct.Name] = l;
                }
                else
                {
                    dictNotEqual[ct.Name].Add(ct);
                }
            }
            string maxKeyNotEqual = "";
            foreach (string str in dictNotEqual.Keys)
            {
                int max = 0;
                if (dictNotEqual[str].Count > max)
                {
                    max = dictNotEqual[str].Count;
                    maxKeyNotEqual = str;
                }
            }
            cts = dict[maxKeyNotEqual];
            if (null != cts)
            {
                while (cts.Count > 0)
                {
                    yield return (new MissingIndex(null, cts, null));
                    tempAll = UniqueColumns(allColumns, cts);
                    if (tempAll.Count > 0) yield return (new MissingIndex(null, cts, tempAll));
                    cts.RemoveAt(cts.Count - 1);
                }
            }
            //----------------------------------------------------------------------------
            // Be sure to remove any duplicates before using it as a new index option.
            // 
            RemoveDuplicates(mi._equal);
            RemoveDuplicates(mi._notequal);
            RemoveDuplicates(mi._include);
            //----------------------------------------------------------------------------
            // If a column is in both the equal and notequal list, remove it from the not equal list.
            // 
            //mi._notequal.RemoveAll(delegate(ColumnType ct) { return (mi._equal.Contains(ct, _columnTypeEqualityComparer)); });
            mi._notequal.RemoveAll(delegate(ColumnType ct) 
            {
                foreach (ColumnType c in mi._equal)
                {
                    if (_columnTypeEqualityComparer.Equals(ct, c))
                    {
                        return true;
                    }
                }
                return false; 
            });
            //----------------------------------------------------------------------------
            // If a column is in the include column that was already defined in either the equal or notequal list, 
            // remove it from the include list.
            // 
            //mi._include.RemoveAll(delegate(ColumnType ct) { return (mi._equal.Contains(ct, _columnTypeEqualityComparer) || mi._notequal.Contains(ct, _columnTypeEqualityComparer)); });

            mi._include.RemoveAll(delegate(ColumnType ct) 
            {
                bool res1 = false;
                bool res2 = false;
                foreach (ColumnType c in mi._equal)
                {
                    if (_columnTypeEqualityComparer.Equals(ct, c))
                    {
                        res1 = true;
                    }
                }
                foreach (ColumnType c in mi._notequal)
                {
                    if (_columnTypeEqualityComparer.Equals(ct, c))
                    {
                        res2 = true;
                    }
                }
                return (res1 || res2);
            });
            yield return (mi);
        }

        private static List<ColumnType> UniqueColumns(List<ColumnType> allColumns, List<ColumnType> cts)
        {
            //return (allColumns.FindAll(delegate(ColumnType ct) { return (!cts.Contains(ct, _columnTypeEqualityComparer)); }));
            return (allColumns.FindAll(delegate(ColumnType ct) 
            {
                foreach (ColumnType c in cts)
                {
                    if (_columnTypeEqualityComparer.Equals(ct, c))
                    {
                        return true;
                    }
                }
                return false;
            }));
        }

        private static void RemoveDuplicates(List<ColumnType> cols)
        {
            if (cols.Count > 1)
            {
                //List<ColumnType> li = cols.Distinct(_columnTypeEqualityComparer).ToList();
                List<ColumnType> li = new List<ColumnType>();
                foreach (ColumnType ct in cols)
                {
                    bool contains = false;
                    foreach (ColumnType c in li)
                    {
                        if (_columnTypeEqualityComparer.Equals(c, ct))
                        {
                            contains = true;
                        }
                    }
                    if (!contains)
                    {
                        li.Add(ct);
                    }
                }
                cols.Clear();
                cols.AddRange(li);
            }
        }

        /// <summary>
        /// This logic does not try to get all of the possible permutations of the columns.  This uses a very
        /// simple algorithm to get the possible permutations of 2 or 3 equal/notequal columns.  The algorithm
        /// will not work for 4 or more columns so it is capped at a max of 12 permutations since anything more
        /// than that ends up being way too many permutations along with a more complex algorithm.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MissingIndex> GetPermutations()
        {
            yield return (new MissingIndex(_equal, _notequal, _include));
            if (_equal.Count > 1)
            {
                int maxPermutations = GetMaxPermutations(_equal.Count);
                List<ColumnType> c = new List<ColumnType>(_equal);
                int count = _equal.Count - 1;
                int i = 0;
                while (--maxPermutations > 0)
                {
                    c.Reverse(i++ % count, 2);
                    yield return (new MissingIndex(c, _notequal, _include));
                }
            }
            if ((_equal.Count <= 0) && (_notequal.Count > 1))
            {
                int maxPermutations = GetMaxPermutations(_notequal.Count);
                List<ColumnType> c = new List<ColumnType>(_notequal);
                int count = _notequal.Count - 1;
                int i = 0;
                while (--maxPermutations > 0)
                {
                    c.Reverse(i++ % count, 2);
                    yield return (new MissingIndex(_equal, c, _include));
                }
            }
        }

        /// <summary>
        /// The number of permutations is a factorial count but that is an
        /// unrealistic number based on the combinations we want to process.
        /// We will hard code the number of permutations based on a realistic limit.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private int GetMaxPermutations(int p)
        {
            switch (p)
            {
                case (1):{return (1);}
                case (2):{return (2);}
                case (3):{return (6);}
                default: { return (12); } // there are 24 permutations of 4 but we will cap at 12
            }
        }

        public string GetColumnListForCreate()
        {
            StringBuilder sb = new StringBuilder();
            string eq = ConcatColumnNames(_equal);
            string neq = ConcatColumnNames(_notequal);
            string inc = ConcatColumnNames(_include);
            sb.Append("(");
            if (!string.IsNullOrEmpty(eq)) sb.Append(eq);
            if (!string.IsNullOrEmpty(neq)) sb.Append(string.IsNullOrEmpty(eq) ? neq : ", " + neq);
            sb.Append(")");
            if (!string.IsNullOrEmpty(inc))
            {
                sb.Append(" include (" + inc + ")");
            }
            return (sb.ToString());
        }

        private string ConcatColumnNames(List<ColumnType> cts)
        {
            if (null == cts) return string.Empty;
            if (cts.Count <= 0) return string.Empty;
            StringBuilder sb = new StringBuilder();
            foreach (ColumnType ct in cts)
            {
                if (sb.Length > 0) sb.Append(", ");
                sb.Append(ct.Name);
            }
            return (sb.ToString());
        }

        public override bool Equals(object obj)
        {
            MissingIndex mi = obj as MissingIndex;
            if (null == mi) return (false);
            var ec = new ColumnTypeEqualityComparer();
            //if (!Enumerable.SequenceEqual(_equal, mi._equal, ec)) return (false);
            //if (!Enumerable.SequenceEqual(_notequal, mi._notequal, ec)) return (false);
            //if (!Enumerable.SequenceEqual(_include, mi._include, ec)) return (false);
            if (!isSequenceEqual(_equal, mi._equal, ec)) return (false);
            if (!isSequenceEqual(_notequal, mi._notequal, ec)) return (false);
            if (!isSequenceEqual(_include, mi._include, ec)) return (false);
            return (true);
        }

        private bool isSequenceEqual(List<ColumnType> s1, List<ColumnType> s2, ColumnTypeEqualityComparer c)
        {
            bool isEqual = false;
            for (int i = 0; i < s1.Count; i++)
            {
                for (int j = 0; j < s1.Count; j++)
                {
                    if (i == j)
                    {
                        isEqual = c.Equals(s1[i], s2[j]);
                        if (isEqual == false) { break; }
                    }
                }
                if (isEqual == false) { break; }
            }
            return isEqual;
        }


        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override string ToString()
        {
            string eq = ConcatColumnNames(_equal);
            string neq = ConcatColumnNames(_notequal);
            string inc = ConcatColumnNames(_include);
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(eq)) sb.Append(string.Format("Equal ({0})", eq));
            if (!string.IsNullOrEmpty(neq)) sb.Append(string.Format("NotEqual ({0})", neq));
            if (!string.IsNullOrEmpty(inc)) sb.Append(string.Format("Include ({0})", inc));
            return (sb.ToString());
        }
        public IEnumerable<string> GetColumnNames(MissingIndexColumnType t)
        {
            List<ColumnType> cols = null;
            switch (t)
            {
                case (MissingIndexColumnType.Equality): { cols = _equal; break; }
                case (MissingIndexColumnType.Inequality): { cols = _notequal; break; }
                case (MissingIndexColumnType.Include): { cols = _include; break; }
            }
            if (null == cols) return (null);
            if (cols.Count > 0)
            {
                List<string> names = new List<string>(cols.Count);
                foreach (ColumnType ct in cols) { names.Add(ct.Name); }
                return (names);
            }
            return (null);
        }
        public int GetColumnCount()
        {
            int colCount = 0;
            if (null != _equal) colCount += _equal.Count;
            if (null != _notequal) colCount += _notequal.Count;
            if (null != _include) colCount += _include.Count;
            return (colCount);
        }

        internal IEnumerable<RedundantIndex> GetRedundantIndexes(TableIndexes indexes)
        {
            if (null == indexes) return (null);
            List<string> columns = new List<string>();
            List<string> includeColumns = new List<string>();
            IEnumerable<string> temp = GetColumnNames(MissingIndexColumnType.Equality);
            if (null != temp) columns.AddRange(temp);
            temp = GetColumnNames(MissingIndexColumnType.Inequality);
            if (null != temp) columns.AddRange(temp);
            temp = GetColumnNames(MissingIndexColumnType.Include);
            if (null != temp) includeColumns.AddRange(temp);

            return (indexes.GetRedundantIndexes(columns, includeColumns));
        }
        private bool IsPartialColumnMatch(string full, string partial)
        {
            if (0 == string.Compare(full, partial, true)) return (true);
            if (full.StartsWith(partial, true, CultureInfo.InvariantCulture)) return (true);
            return (false);
        }
        internal bool IsPartialMatch(DMVMissingIndex dmv)
        {
            string eq = ConcatColumnNames(_equal);
            string neq = ConcatColumnNames(_notequal);
            string inc = ConcatColumnNames(_include);
            if (IsPartialColumnMatch(eq, dmv.EqualityColumns))
            {
                if (IsPartialColumnMatch(neq, dmv.InequalityColumns))
                {
                    if (IsPartialColumnMatch(inc, dmv.IncludedColumns))
                    {
                        return (true);
                    }
                }
            }
            return (false);
        }
    }

}

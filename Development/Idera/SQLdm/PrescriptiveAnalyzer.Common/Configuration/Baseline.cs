using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdoctor.Common.Configuration
{
    [Serializable]
    public enum StretchRange { Up, Down, None }

    [Serializable]
    public class Baseline
    {
        public bool Ascending = true;
        public List<Range> Ranges = new List<Range>();

        public Baseline(string name, long lower, long upper, bool ascending)
        {
            Ascending = ascending;
            AddRange(name, lower, upper);
        }

        public Baseline(bool ascending)
        {
            Ascending = ascending;
        }

        public Baseline() { }

        public void AddRange(string name, long lower, long upper)
        {
            if (RangeCount > 0)
            {
                if (FindRange(name) != null) throw new ApplicationException(string.Format("Baseline.AddRange failed: a range named {0} already exists",name));

                long? min = GetMin();
                if (upper == min-1)
                {
                    Ranges.Add(new Range(name, lower, upper, Ascending));
                    return;
                }
                long? max = GetMax();
                if (lower == max+1)
                {
                    Ranges.Add(new Range(name, lower, upper, Ascending));
                    return;
                }
                if ((lower >= min) && (lower <= max))
                    throw new ApplicationException(string.Format("Baseline.AddRange failed: new range {0} - {1} overlaps existing ranges {2} - {3}", 
                                                                lower, upper, min, max));

                if ((upper >= min) && (upper <= max))
                    throw new ApplicationException(string.Format("Baseline.AddRange failed: new range {0} - {1} overlaps existing ranges {2} - {3}",
                                                                lower, upper, min, max));

                if ((lower > max+1) ||(upper < min-1))
                    throw new ApplicationException(string.Format("Baseline.AddRange failed: new range {0} - {1} is not adjacent to existing ranges {2} - {3}",
                                                                lower, upper, min, max));
            }
            Ranges.Add(new Range(name, lower, upper, Ascending));
        }

        public void RemoveRange(string name, StretchRange stretch)
        {
            Range victim = FindRange(name);
            if (null == victim) return;
            if (StretchRange.Up == stretch)
            {
                Range previous = FindRangeBefore(victim);
                if (null != previous)
                    previous.UpperBound = victim.UpperBound;
                Ranges.Remove(victim);
            }
            else if (StretchRange.Down == stretch)
            {
                Range next = FindRangeAfter(victim);
                if (null != next)
                    next.LowerBound = victim.LowerBound;
                Ranges.Remove(victim);
            }
            else if (StretchRange.None == stretch)
            {
                if (victim.LowerBound == GetMin())
                    Ranges.Remove(victim);
                else if (victim.UpperBound == GetMax())
                    Ranges.Remove(victim);
                else
                    throw new ApplicationException(string.Format("Baseline.RemoveRange failed: remove range {0} would break range continuity",
                                                                victim.ToString()));
            }
        }

        public void EditRange(string name, long lower, long upper)
        {
            Range target = FindRange(name);
            if (null == target) throw new ApplicationException(string.Format("Baseline.EditRange failed: No range named {0} exists", name));

            Range previous = FindRangeBefore(target);
            bool lowerBoundEditValidated = false;
            if (null == previous)
                lowerBoundEditValidated = true;
            else
            {
                lowerBoundEditValidated = (previous.LowerBound < lower); // valid if the previous range will still exist after edit
            }

            Range next = FindRangeAfter(target);
            bool upperBoundEditValidated = false;
            if (null == next)
                upperBoundEditValidated = true;
            else
                upperBoundEditValidated = (next.UpperBound > upper); // valid if the next range will still exist after edit

            if (lowerBoundEditValidated && upperBoundEditValidated)
            {
                target.LowerBound = lower;
                target.UpperBound = upper;

                if (previous != null)
                    previous.UpperBound = target.LowerBound-1;
                if (next != null)
                    next.LowerBound = target.UpperBound+1;
            }
            else
                throw new ApplicationException("Baseline.EditRange failed: edit would destroy an existing range");
        }

        public string GetRange(long value)
        {
            foreach (Range r in Ranges)
                if (0 == r.Compare(value)) return r.Name;

            return null;
        }

        public int RangeCount
        {
            get { return Ranges.Count; }
        }
        public long? GetMin()
        {
            if (0 == RangeCount) return null;
            long min = long.MaxValue;
            foreach (Range r in Ranges)
                if (min > r.LowerBound)
                    min = r.LowerBound;
            return min;
        }
        public long? GetMax()
        {
            if (0 == RangeCount) return null;
            long max = long.MinValue;
            foreach (Range r in Ranges)
                if (max < r.UpperBound)
                    max = r.UpperBound;
            return max;
        }
        public Range FindRange(string name)
        {
            foreach (Range r in Ranges)
                if (r.Name == name) return r;
            return null;
        }
        public Range FindRangeBefore(Range target)
        {
            foreach (Range r in Ranges)
                if (r.UpperBound == target.LowerBound-1)
                    return r;
            return null;
        }
        public Range FindRangeAfter(Range target)
        {
            foreach (Range r in Ranges)
                if (r.LowerBound == target.UpperBound+1)
                    return r;
            return null;
        }
    }

    [Serializable]
    public class Range
    {
        public string Name;
        public long LowerBound;
        public long UpperBound;
        public bool Ascending = true;

        public Range(string name, long lowerBound, long upperBound, bool ascending)
        {
            Name = name;
            LowerBound = lowerBound;
            UpperBound = upperBound;
            Ascending = ascending;
        }

        public Range() { }

        public int Compare(long value)
        {
            if (Ascending)
            {
                if (value < LowerBound)
                    return -1;
                if (value > UpperBound)
                    return 1;
            }
            else
            {
                if (value < LowerBound)
                    return 1;
                if (value > UpperBound)
                    return -1;
            }
            return 0;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1} - {2}", Name, LowerBound, UpperBound);
        }
    }
}

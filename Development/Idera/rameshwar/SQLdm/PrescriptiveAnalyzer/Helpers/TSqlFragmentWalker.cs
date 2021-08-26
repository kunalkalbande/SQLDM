//------------------------------------------------------------------------------
// <copyright file="TSqlFragmentWalker.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using Microsoft.Data.Schema.ScriptDom.Sql;
using System.Collections.Generic;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Helpers
{
    public class TSqlEndFragment : TSqlFragment
    {
        public readonly TSqlFragment StartFragment;

        public TSqlEndFragment(TSqlFragment startFragment)
        {
            ScriptTokenStream = startFragment.ScriptTokenStream;
            FirstTokenIndex = startFragment.FirstTokenIndex;
            LastTokenIndex = startFragment.LastTokenIndex;
            StartFragment = startFragment;
        }
    }

    public class TSqlFragmentWalker
    {
        public delegate bool TSqlFilterDelegate(TSqlFragment fragment);

        private TSqlFilterDelegate descendFilter;
        private TSqlFilterDelegate pushEndFilter;

        public TSqlFragmentWalker()
        {
        }

        public IEnumerator<TSqlFragment> GetEnumerator(TSqlFragment fragment, bool returnFragment)
        {
            if (returnFragment)
                yield return fragment;

            if (descendFilter == null || descendFilter.Invoke(fragment))
            {
                IList<TSqlFragment> children = TSqlParsingHelpers.GetChildren(fragment);
                if (children != null && children.Count > 0)
                {
                    foreach (TSqlFragment child in children)
                    {
                        if (child == null)
                            continue;
            
                        IEnumerator<TSqlFragment> childEnum = GetEnumerator(child, true);
                        while (childEnum.MoveNext())
                            yield return childEnum.Current;
                    }
                }
            }

            if (returnFragment && pushEndFilter != null && pushEndFilter.Invoke(fragment))
            {
                yield return new TSqlEndFragment(fragment);
            }
        }

        public TSqlFilterDelegate DescendFilter
        {
            get { return descendFilter; }
            set { descendFilter = value; }
        }

        public TSqlFilterDelegate PushEndFilter
        {
            get { return pushEndFilter; }
            set { pushEndFilter = value; }
        }
    }
}

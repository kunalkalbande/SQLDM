using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdm.Service.Web
{
    public class AlertsGridAdvancedFilterParam
    {
        public List<AlertsGridAdvancedFilterParamFilter> value { get; set; }
        public string property { get; set; }
    }

    public class AlertsGridAdvancedFilterParamFilter
    {
        public string property { get; set; }
        public string op { get; set; }
        public string logicOperator { get; set; }
        public List<string> value { get; set; }
    }

    public class AlertsGridAdvancedFilterParams : List<AlertsGridAdvancedFilterParam>
    {
        public AlertsGridAdvancedFilterParams() { }
        public AlertsGridAdvancedFilterParams(IEnumerable<AlertsGridAdvancedFilterParam> alertsGridAdvancedFilterParams) : base(alertsGridAdvancedFilterParams) { }
    }

    public class AdvanceFilter
    {
        public string filterName { get; set; }
        public List<AlertsGridAdvancedFilterParamFilter> filterConfig { get; set; }
    }

    public class ExtSortParam
    {
        public string property { get; set; }
        public string direction { get; set; }
    }

    public class ExtSortParams : List<ExtSortParam>
    {
        public ExtSortParams() { }
        public ExtSortParams(IEnumerable<ExtSortParam> extSortParams) : base(extSortParams) { }
    }

}
